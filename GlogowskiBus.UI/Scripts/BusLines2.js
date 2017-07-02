





function Point2(route, model, engine)
{
    var self = this;
    
    self.id = model ? model.Id : null;

    //self.latitude = ko.observable(model ? model.Latitude : 0);
    //self.longitude = ko.observable(model ? model.Longitude : 0);
    self.pointPosition = ko.observable(new google.maps.LatLng(model ? model.Latitude : 0, model ? model.Longitude : 0));
    self.timeOffset = ko.observable(model ? model.TimeOffset : 0);
    

    self.busStopId = ko.observable(model ? model.BusStopId : null);

    self.getModel = function()
    {
        var pointModel =
        {
            Id: self.id,
            Latitude: self.position().lat(),
            Longitude: self.position().lng(),
            TimeOffset: self.timeOffset(),
            BusStopId: self.busStop() ? self.busStop().id : null
        }
        return pointModel;
    };

    self.busLine = route.busLine;
    self.route = route;

    //self.busStop = ko.observable();
    //if (model.BusStopId)
    //    for (var i = 0; i < engine.busStops().length; i++)
    //        if (engine.busStops()[i].id === model.BusStopId)
    //        {
    //            self.busStop(engine.busStops()[i]);
    //            break;
    //        }

    self.busStop = ko.computed(function() 
    { 
        if (self.busStopId())
            for (var i = 0; i < engine.busStops().length; i++)
                if (engine.busStops()[i].id === self.busStopId())
                    return engine.busStops()[i];
        return null;
    });

    self.timeOffsetInMinutes = ko.observable(self.busStop() ? (self.timeOffset() / 60000).toFixed(0) : null);

    self.timeOffset.subscribe(function(newValue)
    {
    });

    self.timeOffsetInMinutes.subscribe(function(newValue)
    {

        self.timeOffset(newValue * 60000);
    });

    self.position = ko.computed(function()
    {
        if (self.busStop())
            return new self.busStop().position();
        else
            return new self.pointPosition();
    });

    var marker = new google.maps.Marker(
    {
        draggable: true,
        //optimized: false,
        position: self.position()
    });

    //self.busStop().latitude.subscribe(function(newValue)
    //{
    //    console.log(newValue);
    //    if (newValue)
    //        self.position(new google.maps.LatLng(newValue, self.position().lng()));
    //});

    var updateMarkerIcon = function()
    {
        if (self.busStop())
        {
            marker.setIcon(markerIcons.redBusStopOnRoute);
            marker.zIndex = 1;
        }
        else
        {
            marker.setIcon(markerIcons.point);
            marker.zIndex = 2;
        }
    };

    updateMarkerIcon();

    var updateMarkerMap = function()
    {
        if (self.route.isEditable() && self.route === engine.selectedRoute())
            marker.setMap(map);
        else
            marker.setMap(null);
    };

    updateMarkerMap();

    var positionSubscription = self.position.subscribe(function(newValue)
    {
        marker.setPosition(newValue);
        self.route.updatePolylines();
    });

    var selectedRouteSubscription = engine.selectedRoute.subscribe(function(newValue)
    {
        updateMarkerMap();
    });

    var busStopSubscription = self.busStop.subscribe(function(newValue)
    {
        updateMarkerIcon();
        self.timeOffsetInMinutes((self.timeOffset() / 60000).toFixed(0));
    });

    var routeIsEditableSubscription = self.route.isEditable.subscribe(function(newValue)
    {
        updateMarkerMap();
    });

    marker.addListener('click', function()
    {
        if (self.route.points().length > 2)
        {
            self.route.points.remove(self);
            self.dispose();
        }
    });

    marker.addListener('dragend', function(e)
    {
        var mousePositionPixels = overlay.getProjection().fromLatLngToContainerPixel(e.latLng);
        for (var i = 0; i < engine.busStops().length; i++)
            if (self.route.busStops().indexOf(engine.busStops()[i]) === -1)
            {
                var busStopPositionPixels = overlay.getProjection().fromLatLngToContainerPixel(engine.busStops()[i].position());
                if (Math.abs(mousePositionPixels.x - busStopPositionPixels.x) <= 9 && Math.abs(mousePositionPixels.y - busStopPositionPixels.y) <= 9)
                {
                    //self.position(new google.maps.LatLng(engine.busStops()[i].latitude(), engine.busStops()[i].longitude()));
                    //self.latitude(engine.busStops()[i].latitude());
                    //self.longitude(engine.busStops()[i].longitude());
                    self.busStopId(engine.busStops()[i].id);
                    return;
                }
            }
        self.pointPosition(marker.position);
        //self.latitude(marker.position.lat());
        //self.longitude(marker.position.lng());
        self.busStopId(null);
    });

    self.dispose = function()
    {
        marker.setMap(null);
        positionSubscription.dispose();
        selectedRouteSubscription.dispose();
        busStopSubscription.dispose();
        routeIsEditableSubscription.dispose();
    };
}