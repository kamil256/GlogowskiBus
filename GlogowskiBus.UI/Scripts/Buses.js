function Bus(departureTime)
{
    var self = this;

    var route = departureTime.route;
    var points = route.points;
    var busLine = route.busLine;

    self.busNumber = busLine.busNumber;
    self.departureTime = departureTime;

    var marker = new google.maps.Marker(
    {
        icon: markerIcons.redBus2,
        label:
        {
            color: '#FF0000',
            fontSize: '9px',
            fontWeight: 'bold',
            text: self.busNumber
        },
        map: busLine.hidden() ? null : map,
        position:
        {
            lat: points.getFirst().latitude,
            lng: points.getFirst().longitude
        }
    });

    self.selectBusEvent = null;
    self.removeBusEvent = null;

    marker.addListener('click', function()
    {
        if (self.selectBusEvent)
            self.selectBusEvent(self.departureTime);
    });

    self.hide = function()
    {
        if (marker.getMap() != null)
            marker.setMap(null);
    };

    self.show = function()
    {
        if (marker.getMap() == null)
            marker.setMap(map);
    };

    busLine.hidden.subscribe(function(newValue)
    {
        if (newValue)
            self.hide();
        else
            self.show();
    });

    var departureMillisecondsSinceMidnight = 60 * 60 * 1000 * self.departureTime.hours + 60 * 1000 * self.departureTime.minutes;

    var update = function()
    {
        var now = serverTime.now();
        var currentMillisecondsSinceMidnight = 60 * 60 * 1000 * now.getHours() + 60 * 1000 * now.getMinutes() + 1000 * now.getSeconds() + now.getMilliseconds();
        if (currentMillisecondsSinceMidnight < departureMillisecondsSinceMidnight)
            departureMillisecondsSinceMidnight -= 24 * 60 * 60 * 1000;
        var currentTimeOffset = currentMillisecondsSinceMidnight - departureMillisecondsSinceMidnight;

        if (currentTimeOffset > points.getLast().timeOffset)
        {
            self.hide();
            if (self.removeBusEvent)
                self.removeBusEvent(self);
        }
        else
        {
            for (var i = 0; i < points.count() - 1; i++)
                if ((currentTimeOffset >= points.getAt(i).timeOffset && currentTimeOffset < points.getAt(i + 1).timeOffset))
                {
                    var newPosition = getPositionBetweenTwoPoints(points.getAt(i), points.getAt(i + 1), currentTimeOffset);
                    marker.setPosition(newPosition);
                    break;
                }
            setTimeout(update, 100);
        }
    }

    update();
}