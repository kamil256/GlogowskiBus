function BusStops()
{
    var self = this;

    var busStops = new Collection();

    self.count = busStops.count;
    self.add = busStops.add;
    self.addMany = busStops.addMany;
    self.getAt = busStops.getAt;
    self.getFirst = busStops.getFirst;
    self.getLast = busStops.getLast;
    self.getSingle = busStops.getSingle;
    self.get = busStops.get;
    self.toArray = busStops.toArray;
    self.sort = busStops.sort;
}

function BusStop(name, latitude, longitude)
{
    var self = this;

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