function GoogleMapsDirections()
{
    var self = this;

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

    var origin = null;
    var waypoints = [];
    var destination = null;

    var points = ko.observableArray([]);

    var originMarker = new google.maps.Marker(
    {
        draggable: true,
        label: 'A',
        map: null//,
        //position: origin
    });
    originMarker.addListener('dragend', function(e)
    {
        origin = e.latLng;
    });
    
    //var mapClickListener = map.addListener('click', function(e)
    //{
    //    self.addNewPoint(e.latLng);
    //});

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

    self.getPoints = function()
    {
        //var points = [];
        //if (origin)
        //    points.push(
        //    {
        //        latitude: origin.lat(),
        //        longitude: origin.lng()
        //    });
        //for (var i = 0; i < waypoints.length; i++)
        //    points.push(
        //    {
        //        latitude: waypoints[i].location.lat(),
        //        longitude: waypoints[i].location.lng()
        //    });
        //if (destination)
        //    points.push(
        //    {
        //        latitude: destination.lat(),
        //        longitude: destination.lng()
        //    });
        return points();
    };

    self.dispose = function()
    {
        originMarker.setMap(null);
        display.setMap(null);
        //google.maps.event.removeListener(mapClickListener);
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
                    points.removeAll();
                    for (var i = 0; i < response.routes[0].overview_path.length; i++)
                    {
                        points.push(
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
}