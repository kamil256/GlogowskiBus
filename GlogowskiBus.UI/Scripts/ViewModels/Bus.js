function Bus(departureTime, engine)
{
    var self = this;

    self.busNumber = departureTime.route.busLine.busNumber();
    self.departureTime = departureTime;

    var points = departureTime.route.points;

    var marker = new google.maps.Marker(
    {
        icon: markerIcons.redBus,
        label:
        {
            color: '#FF0000',
            fontSize: '9px',
            fontWeight: 'bold',
            text: self.busNumber
        },
        map: map,
        position: points()[0].position()
    });

    var updateMarkerColor = function()
    {
        if (engine.selectedDepartureTime() === self.departureTime)
        {
            marker.setIcon(markerIcons.activeRedBus);
            var label = marker.getLabel();
            label.color = '#FF0000';
            marker.setLabel(label);
            marker.setZIndex(12);
        }
        else if (engine.selectedBusLine() === self.departureTime.route.busLine)
        {
            marker.setIcon(markerIcons.redBus);
            var label = marker.getLabel();
            label.color = '#FF0000';
            marker.setLabel(label);
            marker.setZIndex(11);
        }
        else
        {
            marker.setIcon(markerIcons.grayBus);
            var label = marker.getLabel();
            label.color = '#404040';
            marker.setLabel(label);
            marker.setZIndex(10);
        }
    };

    updateMarkerColor();

    var engineSelectedBusLineSubscription = engine.selectedBusLine.subscribe(function()
    {
        updateMarkerColor();
    });

    var engineSelectedDepartureTimeSubscription = engine.selectedDepartureTime.subscribe(function()
    {
        updateMarkerColor();
    });

    marker.addListener('click', function()
    {
        engine.selectDepartureTime(self.departureTime);
    });

    var departureMillisecondsSinceMidnight = 60 * 60 * 1000 * self.departureTime.hours() + 60 * 1000 * self.departureTime.minutes();

    self.update = function()
    {
        var now = serverTime.now();
        var currentMillisecondsSinceMidnight = 60 * 60 * 1000 * now.getHours() + 60 * 1000 * now.getMinutes() + 1000 * now.getSeconds() + now.getMilliseconds();
        if (currentMillisecondsSinceMidnight < departureMillisecondsSinceMidnight)
            departureMillisecondsSinceMidnight -= 24 * 60 * 60 * 1000;
        var currentTimeOffset = currentMillisecondsSinceMidnight - departureMillisecondsSinceMidnight;
        if (currentTimeOffset > points()[points().length - 1].timeOffset())
        {
            engine.buses.remove(self);
            self.dispose();
        }
        else
        {
            for (var i = 0; i < points().length - 1; i++)
                if ((currentTimeOffset >= points()[i].timeOffset() && currentTimeOffset < points()[i + 1].timeOffset()))
                {
                    var newPosition = getPositionBetweenTwoPoints(points()[i], points()[i+1], currentTimeOffset);
                    marker.setPosition(newPosition);
                    break;
                }
        }
    }

    self.dispose = function()
    {
        marker.setMap(null);
        engineSelectedBusLineSubscription.dispose();
        engineSelectedDepartureTimeSubscription.dispose();
    };
}