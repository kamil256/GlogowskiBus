function GoogleMapsDirections()
{
    var self = this;

    var origin = null;
    var waypoints = [];
    var destination = null;
    var service = new google.maps.DirectionsService();
    var display = new google.maps.DirectionsRenderer(
    {
        draggable: true,
        map: map,
        preserveViewport: true
    });
    display.addListener('directions_changed', function()
    {
        origin = display.getDirections().request.origin;
        waypoints = display.getDirections().request.waypoints;
        for (var i = 0; i < waypoints.length; i++)
            waypoints[i].stopover = true;
        destination = display.getDirections().request.destination;
        update();
    });

    self.points = ko.observableArray([]);

    var originMarker = new google.maps.Marker(
    {
        draggable: true,
        label: 'A',
        map: null,
    });
    originMarker.addListener('dragend', function(e)
    {
        origin = e.latLng;
    });

    self.addNewPoint = function(point)
    {
        if (!origin)
            origin = point;
        else
        {
            if (destination)
            {
                waypoints.push(
                {
                    location: destination,
                    stopover: true
                });
            }
            destination = point;
        }

        update();
    };
    
    var update = function()
    {
        if (origin && !destination)
        {
            originMarker.setMap(map);
            originMarker.setPosition(origin);
        }
        if (origin && destination)
        {
            originMarker.setMap(null);
            service.route(
            {
                origin: origin,
                waypoints: waypoints,
                destination: destination,
                travelMode: 'DRIVING'
            }, function(response, status)
            {
                if (status === 'OK')
                {
                    display.setDirections(response);
                    self.points.removeAll();
                    for (var i = 0; i < response.routes[0].overview_path.length; i++)
                    {
                        self.points.push(
                        {
                            Id: null,
                            Latitude: response.routes[0].overview_path[i].lat(),
                            Longitude: response.routes[0].overview_path[i].lng(),
                            TimeOffset: 0,
                            BusStopId: null
                        });
                    }
                }
            });
        }
    };

    self.dispose = function()
    {
        originMarker.setMap(null);
        display.setMap(null);
    };
}