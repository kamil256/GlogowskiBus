function BusStop2(model, engine)
{
    var self = this;

    self.id = model ? model.Id : null;
    self.name = ko.observable(model ? model.Name : 'Nowy przystanek');
    self.latitude = ko.observable(model ? model.Latitude : 0);
    self.longitude = ko.observable(model ? model.Longitude : 0);

    self.getModel = function()
    {
        var busStopModel =
        {
            Id: self.id,
            Name: self.name(),
            Latitude: self.latitude(),
            Longitude: self.longitude()
        }
        return busStopModel;
    };

    self.points = ko.computed(function()
    {
        var result = [];
        for (var i = 0; i < engine.points().length; i++)
            if (engine.points()[i].busStopId() === self.id && result.indexOf(engine.points()[i]) === -1)
                result.push(engine.points()[i]);
        return result;
    });

    self.routes = ko.computed(function()
    {
        var result = [];
        for (var i = 0; i < self.points().length; i++)
            if (result.indexOf(self.points()[i].route) === -1)
                result.push(self.points()[i].route);
        return result;
    });

    self.busLines = ko.computed(function()
    {
        var result = [];
        for (var i = 0; i < self.routes().length; i++)
            if (result.indexOf(self.routes()[i].busLine) === -1)
                result.push(self.routes()[i].busLine);
        return result;
    });

    self.busNumbers = ko.computed(function()
    {
        var result = [];
        for (var i = 0; i < self.busLines().length; i++)
            if (result.indexOf(self.busLines()[i].busNumber()) === -1)
                result.push(self.busLines()[i].busNumber());
        result.sort(function(busNumber1, busNumber2)
        {
            if (busNumber1 > busNumber2)
                return 1;
            else if (busNumber1 < busNumber2)
                return -1;
            else
                return 0;
        });
        return result;
    });

    self.isVisible = ko.observable(true);

    var marker = new google.maps.Marker(
    {
        icon: markerIcons.redBusStop,
        map: map,
        //optimized: false,
        position:
        {
            lat: self.latitude(),
            lng: self.longitude()
        },
        title: self.name(),
        zIndex: 0
    });

    var nameSubscription = self.name.subscribe(function(newValue)
    {
        marker.setTitle(newValue);
    });

    var latitudeSubscription = self.latitude.subscribe(function(newValue)
    {
        marker.setPosition(new google.maps.LatLng(newValue, self.longitude()));
    });

    var longitudeSubscription = self.longitude.subscribe(function(newValue)
    {
        marker.setPosition(new google.maps.LatLng(self.latitude(), newValue));
    });

    var updateMarkerIcon = function()
    {
        if (engine.selectedBusStop() == self)
            marker.setIcon(markerIcons.activeRedBusStop);
        else if (engine.selectedRoute() && self.routes().indexOf(engine.selectedRoute()) !== -1)
            marker.setIcon(markerIcons.redBusStopOnRoute);
        else
            marker.setIcon(markerIcons.redBusStop);
    };

    var updateMarkerMap = function()
    {
        if (self.isVisible())
            marker.setMap(map);
        else
            marker.setMap(null);
    };

    var selectedBusStopSubscription = engine.selectedBusStop.subscribe(function()
    {
        updateMarkerIcon();
    });

    var selectedRouteSubscription = engine.selectedRoute.subscribe(function()
    {
        updateMarkerIcon();
    });

    var isVisibleSubscription = self.isVisible.subscribe(function()
    {
        updateMarkerMap();
    });

    marker.addListener('click', function()
    {
        if (engine.busStopClickListener)
            engine.busStopClickListener(self);
    });

    self.dispose = function()
    {
        marker.setMap(null);
        nameSubscription.dispose();
        latitudeSubscription.dispose();
        longitudeSubscription.dispose();
        selectedBusStopSubscription.dispose();
        selectedRouteSubscription.dispose();
        isVisibleSubscription.dispose();
    };
}