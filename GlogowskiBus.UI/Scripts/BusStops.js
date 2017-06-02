function BusStops(busStopsArray)
{
    var self = this;

    var elements = new Collection(busStopsArray);

    self.count = elements.count;
    self.getElementAt = elements.getElementAt;
    self.getFirst = elements.getFirst;
    self.getLast = elements.getLast;
    self.getSingle = elements.getSingle;
    self.get = elements.get;
    self.getAll = elements.getAll;
}

function BusStop(name, latitude, longitude)
{
    var self = this;

    self.name = name;
    self.latitude = latitude;
    self.longitude = longitude;

    self.points = [];

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

    self.click = null;
    marker.addListener('click', function()
    {
        if (self.click)
            self.click(self);
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