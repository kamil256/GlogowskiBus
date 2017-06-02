function BusStops(busStops)
{
    var self = this;

    var elements = [];
    for (var i = 0; i < busStops.length; i++)
        elements.push(busStops[i]);

    self.length = elements.length;

    self.count = elements.length;

    self.getElementAt = function(index)
    {
        return elements[index];
    };

    self.getFirst = function()
    {
        if (elements.length > 0)
            return elements[0];
        return null;
    };

    self.getLast = function()
    {
        if (elements.length > 0)
            return elements[elements.length - 1];
        return null;
    };

    self.getSingle = function(condition)
    {
        if (condition)
            for (var i = 0; i < elements.length; i++)
                if (condition(elements[i]))
                    return elements[i];
        return null;
    };

    self.get = function(condition)
    {
        var result = [];
        for (var i = 0; i < elements.length; i++)
            if (condition(elements[i]))
                result.push(elements[i]);
        return result;
    };

    self.getAll = function()
    {
        return elements;
    };
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