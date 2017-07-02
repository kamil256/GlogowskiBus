function Point(route, model, engine)
{
    var self = this;
    
    self.id = model ? model.Id : null;
    self.position = ko.observable(new google.maps.LatLng(model ? model.Latitude : 0, model ? model.Longitude : 0));
    self.timeOffset = ko.observable(model ? model.TimeOffset : 0);
    self.busStopId = ko.observable(model ? model.BusStopId : null);
    self.busLine = route.busLine;
    self.route = route;

    self.getModel = function()
    {
        var pointModel =
        {
            Id: self.id,
            Latitude: self.position().lat(),
            Longitude: self.position().lng(),
            TimeOffset: self.timeOffset(),
            BusStopId: self.busStopId()
        }
        return pointModel;
    };

    self.busStop = ko.computed(function() 
    { 
        if (self.busStopId())
            for (var i = 0; i < engine.busStops().length; i++)
                if (engine.busStops()[i].id === self.busStopId())
                    return engine.busStops()[i];
        return null;
    });

    self.timeOffsetInMinutes = ko.observable(self.busStop() ? (self.timeOffset() / 60000).toFixed(0) : null);

    self.timeOffsetInMinutes.subscribe(function(newValue)
    {
        self.timeOffset(newValue * 60000);
    });

    var marker = new google.maps.Marker(
    {
        draggable: true,
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
                    self.busStopId(engine.busStops()[i].id);
                    self.position(engine.busStops()[i].position());
                    return;
                }
            }
        self.busStopId(null);
        self.position(marker.position);
    });

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

    var updateMarkerMap = function()
    {
        if (self.route.isEditable() && self.route === engine.selectedRoute())
            marker.setMap(map);
        else
            marker.setMap(null);
    };

    var updateMarkerPosition = function()
    {
        marker.setPosition(self.position());
    };

    updateMarkerIcon();
    updateMarkerMap();
    updateMarkerPosition();

    var selfBusStopSubscription = self.busStop.subscribe(function(newValue)
    {
        updateMarkerIcon();
        self.timeOffsetInMinutes((self.timeOffset() / 60000).toFixed(0));
    });

    var engineSelectedRouteSubscription = engine.selectedRoute.subscribe(function(newValue)
    {
        updateMarkerMap();
    });

    var selfRouteIsEditableSubscription = self.route.isEditable.subscribe(function()
    {
        updateMarkerMap();
    });

    var selfPositionSubscription = self.position.subscribe(function()
    {
        updateMarkerPosition();
        self.route.updatePolylines();
    });

    self.dispose = function()
    {
        
        selfBusStopSubscription.dispose();
        engineSelectedRouteSubscription.dispose();
        selfRouteIsEditableSubscription.dispose();
        selfPositionSubscription.dispose();
        marker.setMap(null);
    };
}