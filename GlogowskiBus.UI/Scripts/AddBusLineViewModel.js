function AddBusLineViewModel(engine)
{
    var self = this;

    self.title = 'DODAWANIE LINII';

    self.isSelected = ko.observable(false);

    self.isSelected.subscribe(function(newValue)
    {
        if (newValue === true)
        {
            self.newBusLine(new BusLine2(null, engine));

            engine.selectBusStop(null);
            engine.selectBusLine(self.newBusLine());
            engine.selectedDayOfWeek(serverTime.daysOfWeek[0]);

            engine.mapClickListener = function(e)
            {
                if (self.directions())
                    self.directions().addNewPoint(e.latLng);
            };

            engine.busStopClickListener = function(busStop)
            {
                if (self.directions())
                    self.directions().addNewPoint(new google.maps.LatLng(busStop.latitude(), busStop.longitude()));
            };
        }
    });

    self.newBusLine = ko.observable();
    self.directions = ko.observable();
    self.selectedRoute = engine.selectedRoute;
    self.selectRoute = engine.selectRoute;
    self.selectedDayOfWeek = engine.selectedDayOfWeek;
    self.selectedBusStops = engine.selectedBusStops;
    self.selectedDepartureTimes = engine.selectedDepartureTimes;
    self.newDepartureTimeHours = ko.observable(0);
    self.newDepartureTimeMinutes = ko.observable(0);

    self.addRoute = function()
    {
        engine.selectRoute(null);
        self.directions(new GoogleMapsDirections());
    };

    self.finishAddingRoute2 = function()
    {
        var newRoute = new Route2(self.newBusLine(), null, engine);
        var pointsModel = self.directions().getPoints();
        for (var i = 0; i < pointsModel.length; i++)
            newRoute.points.push(new Point2(newRoute, pointsModel[i], engine));
        newRoute.isEditable(true);
        self.newBusLine().routes.push(newRoute);
        engine.selectRoute(newRoute);
        self.directions().dispose();
        self.directions(null);
    };

    self.cancelAddingRoute = function()
    {
        self.directions().dispose();
        self.directions(null);
    };

    self.addDepartureTime = function()
    {
        if (self.newDepartureTimeHours() >= 0 && self.newDepartureTimeHours() < 24 && self.newDepartureTimeMinutes() >= 0 && self.newDepartureTimeMinutes() < 60 &&
            !self.selectedRoute().getDepartureTime(self.newDepartureTimeHours(), self.newDepartureTimeMinutes(), engine.selectedDayOfWeek()))
        {
            var newDepartureTime = new DepartureTime2(self.selectedRoute(), null, engine.selectedDayOfWeek(), engine);
            newDepartureTime.hours(self.newDepartureTimeHours());
            newDepartureTime.minutes(self.newDepartureTimeMinutes());
            newDepartureTime.dayOfWeek(engine.selectedDayOfWeek());
            self.selectedRoute().departureTimes.push(newDepartureTime);
            self.selectedRoute().sortDepartureTimes();
        }
    }

    self.removeDepartureTime = function(departureTime)
    {
        self.selectedRoute().departureTimes.remove(departureTime);
    };

    self.finishAddingBusLine = function()
    {
        sendAjaxRequest('/api/BusLine', 'POST', self.newBusLine().getModel(), function(model)
        {
            var newBusLine = new BusLine2(model, engine);
            engine.busLines.push(newBusLine);
            engine.selectBusLine(newBusLine);
            self.newBusLine().dispose();
            navigationViewModel.selectView('LINIE');
        });
    };

    self.cancelAddingBusLine = function()
    {
        engine.selectBusLine(null);
        self.newBusLine().dispose();
        navigationViewModel.selectView('LINIE');
    };
}