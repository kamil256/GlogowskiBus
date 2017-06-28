﻿function EditBusLineViewModel(engine)
{
    var self = this;

    self.title = 'EDYTOWANIE LINII';

    self.isSelected = ko.observable(false);

    self.isSelected.subscribe(function(newValue)
    {
        if (newValue === true)
        {
            self.originalBusLine = engine.selectedBusLine();
            
            self.editedBusLine(new BusLine2(self.originalBusLine.getModel(), engine));
            for (var i = 0; i < self.editedBusLine().routes().length; i++)
                self.editedBusLine().routes()[i].isEditable(true);

            engine.selectedBusStop(null);
            engine.selectedBusLine(self.editedBusLine());
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

    self.editedBusLine = ko.observable();
    self.directions = ko.observable();
    self.selectedRoute = engine.selectedRoute;
    self.selectedDayOfWeek = engine.selectedDayOfWeek;
    self.selectedBusStops = engine.selectedBusStops;
    self.selectedDepartureTimes = engine.selectedDepartureTimes;
    self.newDepartureTimeHours = ko.observable(0);
    self.newDepartureTimeMinutes = ko.observable(0);

    self.addRoute = function()
    {
        engine.selectedRoute(null);
        self.directions(new GoogleMapsDirections());
    };

    self.finishAddingRoute = function()
    {
        var newRoute = new Route2(self.editedBusLine(), null, engine);
        var pointsModel = self.directions().getPoints();
        for (var i = 0; i < pointsModel.length; i++)
            newRoute.points.push(new Point2(newRoute, pointsModel[i], engine));
        newRoute.isEditable(true);
        self.editedBusLine().routes.push(newRoute);
        engine.selectedRoute(newRoute);
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

    self.finishEditingBusLine = function()
    {
        console.log(self.editedBusLine().departureTimes());
        sendAjaxRequest('/api/BusLine', 'PUT', self.editedBusLine().getModel(), function(model)
        {
            var editedBusLine = new BusLine2(model, engine);
            engine.busLines.splice(engine.busLines.indexOf(self.originalBusLine), 1, editedBusLine);
            engine.selectedBusLine(editedBusLine);
            self.originalBusLine.dispose();
            self.editedBusLine().dispose();
            navigationViewModel.selectView('LINIE');
        });
    };

    self.cancelEditingBusLine = function()
    {
        engine.selectedBusLine(self.originalBusLine);
        self.editedBusLine().dispose();
        navigationViewModel.selectView('LINIE');
    };
}