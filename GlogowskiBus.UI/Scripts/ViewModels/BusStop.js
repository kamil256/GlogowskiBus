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

    self.points = ko.computed(function()
    {
        var result = [];
        if (engine.busStops.indexOf(self) !== -1)
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
        result.sort(function(busLine1, busLine2)
        {
            if (busLine1.busNumber() > busLine2.busNumber())
                return 1;
            else if (busLine1.busNumber() < busLine2.busNumber())
                return -1;
            else
                return 0;
        });
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

    var marker = new google.maps.Marker(
    {
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
                marker.setIcon(markerIcons.redActiveBusStopOnRoute);
            else
                marker.setIcon(markerIcons.activeRedBusStop);
        }
        else if (engine.selectedRoute() && self.routes().indexOf(engine.selectedRoute()) !== -1)
            marker.setIcon(markerIcons.redBusStopOnRoute);
        else if (engine.selectedBusLine() && self.busLines().indexOf(engine.selectedBusLine()) !== -1)
            marker.setIcon(markerIcons.grayBusStopOnRoute);
        else
            marker.setIcon(markerIcons.grayBusStop);
    };

    var updateMarkerMap = function()
    {
        if (self.isVisible())
            marker.setMap(map);
        else
            marker.setMap(null);
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
    updateMarkerMap();
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
        updateMarkerMap();
    });

    var selfPositionSubscription = self.position.subscribe(function()
    {
        updateMarkerPosition();
        for (var i = 0; i < self.points().length; i++)
            self.points()[i].position(self.position());
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
        marker.setMap(null);
    };
}