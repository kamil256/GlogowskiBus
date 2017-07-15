function AddBusLineViewModel(engine)
{
    var self = this;

    self.title = 'DODAWANIE LINII';

    self.isSelected = ko.observable(false);

    self.isSelected.subscribe(function(newValue)
    {
        if (newValue === true)
        {
            self.newBusLine(new BusLine(null, engine));

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
                    self.directions().addNewPoint(busStop.position());
            };
        }
    });

    self.newBusLine = ko.observable();
    self.directions = ko.observable();
    self.selectedRoute = engine.selectedRoute;
    self.selectRoute = engine.selectRoute;
    self.selectedDayOfWeek = engine.selectedDayOfWeek;
    self.selectedBusStops = engine.selectedBusStops;
    self.departureTimesOfSelectedRoute = engine.departureTimesOfSelectedRoute;
    self.newDepartureTimeHours = ko.observable(0);
    self.newDepartureTimeMinutes = ko.observable(0);

    self.addRouteToNewBusLine = function()
    {
        engine.selectRoute(null);
        self.directions(new GoogleMapsDirections());
    };

    self.finishAddingRouteForNewBusLine = function()
    {
        var newRoute = new Route(self.newBusLine(), null, engine);
        var pointsModel = self.directions().points();
        for (var i = 0; i < pointsModel.length; i++)
            newRoute.points.push(new Point(newRoute, pointsModel[i], engine));
        newRoute.isEditable(true);
        self.newBusLine().routes.push(newRoute);
        engine.selectRoute(newRoute);
        self.directions().dispose();
        self.directions(null);
    };

    self.cancelAddingRouteForNewBusLine = function()
    {
        self.directions().dispose();
        self.directions(null);
    };

    self.copyRouteOfNewBusLine = function(route)
    {
        var copiedRoute = new Route(self.newBusLine(), route.getModel(), engine);
        copiedRoute.isEditable(true);
        self.newBusLine().routes.push(copiedRoute);
    };

    self.deleteRouteOfNewBusLine = function(route)
    {
        self.newBusLine().routes.remove(route);
        route.dispose();
        engine.selectBusLine(self.newBusLine());
    };

    self.addDepartureTimeForNewBusLine = function()
    {
        var hours = Number(self.newDepartureTimeHours());
        var minutes = Number(self.newDepartureTimeMinutes());
        if (hours >= 0 && hours < 24 && minutes >= 0 && minutes < 60 && !self.selectedRoute().getDepartureTime(hours, minutes, engine.selectedDayOfWeek()))
        {
            var newDepartureTime = new DepartureTime(self.selectedRoute(), null, engine.selectedDayOfWeek(), engine);
            newDepartureTime.hours(hours);
            newDepartureTime.minutes(minutes);
            newDepartureTime.dayOfWeek(engine.selectedDayOfWeek());
            self.selectedRoute().departureTimes.push(newDepartureTime);
            self.selectedRoute().sortDepartureTimes();
        }
    }

    self.removeDepartureTimeOfNewBusLine = function(departureTime)
    {
        self.selectedRoute().departureTimes.remove(departureTime);
    };

    self.finishAddingBusLine = function()
    {
        sendAjaxRequest('/api/BusLine', 'POST', self.newBusLine().getModel(), function(model)
        {
            if (model.Message)
            {
                alert(model.Message);
            }
            else
            {
                var newBusLine = new BusLine(model, engine);
                engine.busLines.push(newBusLine);
                engine.selectBusLine(newBusLine);
                self.newBusLine().dispose();
                navigationViewModel.selectView('LINIE');
            }
        });
    };

    self.cancelAddingBusLine = function()
    {
        engine.selectBusLine(null);
        self.newBusLine().dispose();
        navigationViewModel.selectView('LINIE');
    };
}