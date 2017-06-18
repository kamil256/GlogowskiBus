function BusStop(id, name, latitude, longitude)
{
    var self = this;

    self.id = id;
    self.name = ko.observable(name);
    self.latitude = ko.observable(latitude);
    self.longitude = ko.observable(longitude);
    self.points = new Collection();

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