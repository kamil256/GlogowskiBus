function BusStop(id, name, latitude, longitude)
{
    var self = this;

    self.id = id;
    self.name = name;
    self.latitude = latitude;
    self.longitude = longitude;
    self.points = new Collection();

    var marker = new google.maps.Marker(
    {
        icon: markerIcons.redBusStop,
        map: map,
        optimized: false,
        position:
        {
            lat: latitude,
            lng: longitude
        },
        title: name,
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
}