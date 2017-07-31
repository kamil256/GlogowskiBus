function BusStop(model, engine)
{
    var self = this;

    self.isVisible = ko.observable(true);

    self.id = model ? model.Id : null;
    self.name = ko.observable(model ? model.Name : 'Nowy przystanek');
    self.position = ko.observable(new google.maps.LatLng(model ? model.Latitude : 0, model ? model.Longitude : 0));

    self.getModel = function()
    {
        var busStopModel =
        {
            Id: self.id,
            Name: self.name(),
            Latitude: self.position().lat(),
            Longitude: self.position().lng()
        }
        return busStopModel;
    };

    var marker = new google.maps.Marker(
    {
        map: map,
        zIndex: 0
    });

    marker.addListener('click', function()
    {
        if (engine.busStopClickListener)
            engine.busStopClickListener(self);
    });

    var updateMarkerIcon = function()
    {
        if (engine.selectedBusStop() === self)
        {
            if (engine.selectedRoute())
            {
                marker.setIcon(markerIcons.redActiveBusStopOnRoute);
                marker.setZIndex(4);
            }
            else
            {
                marker.setIcon(markerIcons.activeRedBusStop);
                marker.setZIndex(1);
            }
        }
        else if (engine.selectedRoute() && engine.selectedRoute().busStops().indexOf(self) !== -1)//self.routes().indexOf(engine.selectedRoute()) !== -1)
        {
            marker.setIcon(markerIcons.redBusStopOnRoute);
            marker.setZIndex(3);
        }
        else if (engine.selectedBusLine() && engine.selectedBusLine().busStops().indexOf(self) !== -1)//self.busLines().indexOf(engine.selectedBusLine()) !== -1)
        {
            marker.setIcon(markerIcons.grayBusStopOnRoute);
            marker.setZIndex(2);
        }
        else
        {
            marker.setIcon(markerIcons.grayBusStop);
            marker.setZIndex(0);
        }
    };

    var updateMarkerVisibility = function()
    {
        if (self.isVisible() || engine.busStopsAlwaysVisible)
            marker.setVisible(true);
        else
            marker.setVisible(false);
    };

    var updateMarkerPosition = function()
    {
        marker.setPosition(self.position());
    };

    var updateMarkerTitle = function()
    {
        marker.setTitle(self.name());
    };

    updateMarkerIcon();
    updateMarkerVisibility();
    updateMarkerPosition();
    updateMarkerTitle();

    var engineSelectedBusStopSubscription = engine.selectedBusStop.subscribe(function()
    {
        updateMarkerIcon();
    });

    var engineSelectedRouteSubscription = engine.selectedRoute.subscribe(function()
    {
        updateMarkerIcon();
    });

    var engineSelectedBusLineSubscription = engine.selectedBusLine.subscribe(function()
    {
        updateMarkerIcon();
        if (!engine.selectedBusLine() || engine.selectedBusLine().busStops().indexOf(self) !== -1)
            self.isVisible(true);
        else
            self.isVisible(false);
    });

    var selfIsVisibleSubscription = self.isVisible.subscribe(function()
    {
        updateMarkerVisibility();
    });

    var selfPositionSubscription = self.position.subscribe(function()
    {
        updateMarkerPosition();
    });

    var selfNameSubscription = self.name.subscribe(function()
    {
        updateMarkerTitle();
    });

    self.dispose = function()
    {
        engineSelectedBusStopSubscription.dispose();
        engineSelectedRouteSubscription.dispose();
        engineSelectedBusLineSubscription.dispose();
        selfIsVisibleSubscription.dispose();
        selfPositionSubscription.dispose();
        selfNameSubscription.dispose();
        marker.setVisible(false);
        marker.setMap(null);
    };
}