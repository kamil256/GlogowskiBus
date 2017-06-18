function BusStop(id, name, latitude, longitude, busLines)
{
    var self = this;

    self.id = id;
    self.name = ko.observable(name);
    self.latitude = ko.observable(latitude);
    self.longitude = ko.observable(longitude);
    self.getAllBusNumbers = function()
    {
        var allBusNumbers = [];
        for (var i = 0; i < busLines.count() ; i++)
            for (var j = 0; j < busLines.getAt(i).routes.count(); j++)
                for (var k = 0; k < busLines.getAt(i).routes.getAt(j).points.count(); k++)
                    if (busLines.getAt(i).routes.getAt(j).points.getAt(k).busStop == self && allBusNumbers.indexOf(busLines.getAt(i).busNumber()) == -1)
                        allBusNumbers.push(busLines.getAt(i).busNumber());
        allBusNumbers.sort(function(busNumber1, busNumber2)
        {
            if (busNumber1 > busNumber2)
                return 1;
            else if (busNumber1 < busNumber2)
                return -1;
            else
                return 0;
        });
        return allBusNumbers;
    };

    var marker = new google.maps.Marker(
    {
        icon: markerIcons.redBusStop,
        map: map,
        optimized: false,
        position:
        {
            lat: self.latitude(),
            lng: self.longitude()
        },
        title: self.name(),
        zIndex: 0
    });

    self.latitude.subscribe(function(newValue)
    {
        marker.setPosition(new google.maps.LatLng(newValue, self.longitude()));
    });

    self.longitude.subscribe(function(newValue)
    {
        marker.setPosition(new google.maps.LatLng(self.latitude(), newValue));
    });

    self.selectBusStopEvent = null;

    marker.addListener('click', function()
    {
        if (self.selectBusStopEvent)
            self.selectBusStopEvent(self);
    });

    self.select = function()
    {
        marker.setIcon(markerIcons.activeRedBusStop);
    };

    self.deselect = function()
    {
        marker.setIcon(markerIcons.redBusStop);
    };

    self.dispose = function()
    {
        marker.setMap(null);
    };
}