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
            
            self.editedBusLine(new BusLine(self.originalBusLine.getModel(), engine));
            for (var i = 0; i < self.editedBusLine().routes().length; i++)
                self.editedBusLine().routes()[i].isEditable(true);

            engine.selectBusStop(null);
            engine.selectBusLine(self.editedBusLine());
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

    self.editedBusLine = ko.observable();
    self.directions = ko.observable();
    self.selectedRoute = engine.selectedRoute;
    self.selectRoute = engine.selectRoute;
    self.selectedDayOfWeek = engine.selectedDayOfWeek;
    self.selectedBusStops = engine.selectedBusStops;
    self.departureTimesOfSelectedRoute = engine.departureTimesOfSelectedRoute;
    self.newDepartureTimeHours = ko.observable(0);
    self.newDepartureTimeMinutes = ko.observable(0);

    self.addRouteToExistingBusLine = function()
    {
        engine.selectRoute(null);
        self.directions(new GoogleMapsDirections());
    };

    self.finishAddingRouteForExistingBusLine = function()
    {
        var newRoute = new Route(self.editedBusLine(), null, engine);
        var pointsModel = self.directions().points();
        for (var i = 0; i < pointsModel.length; i++)
            newRoute.points.push(new Point(newRoute, pointsModel[i], engine));
        newRoute.isEditable(true);
        self.editedBusLine().routes.push(newRoute);
        engine.selectRoute(newRoute);
        self.directions().dispose();
        self.directions(null);
    };

    self.cancelAddingRouteForExistingBusLine = function()
    {
        self.directions().dispose();
        self.directions(null);
    };

    self.copyRouteOfExistingBusLine = function(route)
    {
        var copiedRoute = new Route(self.editedBusLine(), route.getModel(), engine);
        copiedRoute.isEditable(true);
        self.editedBusLine().routes.push(copiedRoute);
    };

    self.deleteRouteOfExistingBusLine = function(route)
    {
        self.editedBusLine().routes.remove(route);
        route.dispose();
        engine.selectBusLine(self.editedBusLine());
    };

    self.addDepartureTimeForExistingBusLine = function()
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
        }
    }

    self.removeDepartureTimeOfExistingBusLine = function(departureTime)
    {
        self.selectedRoute().departureTimes.remove(departureTime);
    };

    self.finishEditingBusLine = function()
    {
        sendAjaxRequest('/api/BusLine', 'PUT', self.editedBusLine().getModel(), function(model)
        {
            if (model.Message)
            {
                alert(model.Message);
            }
            else
            {
                var editedBusLine = new BusLine(model, engine);
                engine.busLines.splice(engine.busLines.indexOf(self.originalBusLine), 1, editedBusLine);
                engine.selectBusLine(editedBusLine);
                self.originalBusLine.dispose();
                self.editedBusLine().dispose();
                navigationViewModel.selectView('LINIE');
            }
        });
    };

    self.cancelEditingBusLine = function()
    {
        engine.selectBusLine(self.originalBusLine);
        self.editedBusLine().dispose();
        navigationViewModel.selectView('LINIE');
    };
}