function AdministrationContext()
{
    var self = this;

    function View(name, tabs)
    {
        var self = this;

        self.name = name;
        self.tabs = tabs;
        self.selectedTab = ko.observable(self.tabs[0])
    }

    self.views = [
        new View('List', ['PRZYSTANKI', 'LINIE']),
        new View('AddBusStop', ['DODAWANIE PRZYSTANKU']),
        new View('EditBusStop', ['EDYTOWANIE PRZYSTANKU']),
        new View('DeleteBusStop', ['USUWANIE PRZYSTANKU']),
        new View('AddBusLine', ['DODAWANIE LINII']),
        new View('EditBusLine', ['EDYTOWANIE LINII']),
        new View('DeleteBusLine', ['USUWANIE LINII'])
    ];

    self.selectedView = ko.observable(self.views[0]);

    self.busStops = new Collection();
    self.busLines = new Collection();
    self.directions = ko.observable();

    self.selectedBusStop = ko.observable();
    var selectBusStopEvent = function(busStop)
    {
        switch (self.selectedView().name)
        {
            case 'List':
                self.selectedBusStop(busStop);
                break;
        }
    };

    self.selectedDayOfWeek = ko.observable(serverTime.daysOfWeek[0]);

    sendAjaxRequest('/api/BusStop', "GET", null, function(response)
    {
        for (var i = 0; i < response.length; i++)
        {
            var busStop = new BusStop(response[i].Id, response[i].Name, response[i].Latitude, response[i].Longitude, self.busLines);
            busStop.selectBusStopEvent = selectBusStopEvent;
            self.busStops.add(busStop);
        }

        sendAjaxRequest('/api/BusLine', "GET", null, function(response)
        {
            for (var i = 0; i < response.length; i++)
                self.busLines.add(new BusLine(response[i].Id, response[i].BusNumber, response[i].Routes, self.busStops));
        });
    });

    self.getAllRoutes = ko.computed(function()
    {
        var allRoutes = new Collection();
        for (var i = 0; i < self.busLines.count(); i++)
            allRoutes.addMany(self.busLines.getAt(i).routes.toArray());
        return allRoutes;
    });

    
    
    

    self.selectedBusStop.subscribe(function(newBusStop)
    {
        for (var i = 0; i < self.busStops.count() ; i++)
            self.busStops.getAt(i).deselect();
        if (newBusStop)
            newBusStop.select();
        if (self.selectedRoute())
            self.selectedRoute().selectBusStop(newBusStop);
    });

    self.selectedBusLine = ko.observable();

    self.selectedRoute = ko.observable();

    self.selectedRoute.subscribe(function(newRoute)
    {
        for (var i = 0; i < self.getAllRoutes().count() ; i++)
            self.getAllRoutes().getAt(i).deselect();
        if (self.selectedBusLine())
            for (var i = 0; i < self.selectedBusLine().routes.count() ; i++)
                self.selectedBusLine().routes.getAt(i).deselect();
        if (newRoute)
        {
            newRoute.select();
            newRoute.selectBusStopEvent = selectBusStopEvent;
        }

        if (newRoute && self.selectedBusStop())
            newRoute.selectBusStop(self.selectedBusStop());
    });

    map.addListener('click', function(e)
    {
        switch (self.selectedView().name)
        {
            case 'List':
                self.selectedBusStop(null);
                self.selectedRoute(null);
                break;
            case 'AddBusStop':
            case 'EditBusStop':
                self.selectedBusStop().latitude(e.latLng.lat());
                self.selectedBusStop().longitude(e.latLng.lng());
                break;
            case 'AddBusLine':
                if (self.directions())
                    self.directions().addNewPoint(e.latLng);
        }
    });

    self.selectedDepartureTimes = ko.computed(function()
    {
        var departureTimes = [];
        if (self.selectedRoute())
        {
            for (var i = 0; i < 24; i++)
                departureTimes[i] = [];
            for (var i = 0; i < self.selectedRoute().departureTimes.count() ; i++)
            {
                var minutes = self.selectedRoute().departureTimes.getAt(i).minutes();
                var hours = self.selectedRoute().departureTimes.getAt(i).hours();
                var dayOfWeek = self.selectedRoute().departureTimes.getAt(i).dayOfWeek();
                if (dayOfWeek == self.selectedDayOfWeek())
                {
                    departureTimes[hours].push(
                    {
                        minutes: minutes
                    });
                }
            }
        }
        return departureTimes;
    });

    self.newDepartureTime =
    {
        hours: ko.observable(0),
        minutes: ko.observable(0),
    };

    self.busStopsListAddBtnClick = function()
    {
        self.selectedBusStop(new BusStop(null, 'Nowy przystanek', 0, 0, self.busLines));
        self.selectedView(self.views[1]);
    };

    self.busStopsListEditBtnClick = function(busStop)
    {
        self.selectedBusStop(new BusStop(busStop.id, busStop.name(), busStop.latitude(), busStop.longitude(), self.busLines));
        self.selectedView(self.views[2]);
    };

    self.busStopsListDeleteBtnClick = function(busStop)
    {
        self.selectedBusStop(busStop);
        self.selectedView(self.views[3]);
    };

    self.addBusStopAddBtnClick = function()
    {
        sendAjaxRequest('/api/BusStop', "POST",
        {
            Name: self.selectedBusStop().name(),
            Latitude: self.selectedBusStop().latitude(),
            Longitude: self.selectedBusStop().longitude()
        }, function(response)
        {
            self.selectedView(self.views[0]);

            self.selectedBusStop().dispose();

            var newBusStop = new BusStop(response.Id, response.Name, response.Latitude, response.Longitude, self.busLines);
            newBusStop.selectBusStopEvent = selectBusStopEvent;
            self.busStops.add(newBusStop);
            self.selectedBusStop(newBusStop);
        });
    };

    self.addBusStopCancelBtnClick = function()
    {
        self.selectedView(self.views[0]);

        self.selectedBusStop().dispose();
        self.selectedBusStop(null);
    };

    self.editBusStopEditBtnClick = function()
    {
        sendAjaxRequest('/api/BusStop', "PUT",
        {
            Id: self.selectedBusStop().id,
            Name: self.selectedBusStop().name(),
            Latitude: self.selectedBusStop().latitude(),
            Longitude: self.selectedBusStop().longitude()
        }, function(response)
        {
            self.selectedView(self.views[0]);

            self.selectedBusStop().dispose();

            var existingBusStop = self.busStops.getSingle(function(busStop) { return busStop.id == response.Id; });
            existingBusStop.name(response.Name);
            existingBusStop.latitude(response.Latitude);
            existingBusStop.longitude(response.Longitude);

            self.selectedBusStop(existingBusStop);
        });
    };

    self.editBusStopCancelBtnClick = function()
    {
        self.selectedView(self.views[0]);

        self.selectedBusStop().dispose();
        self.selectedBusStop(self.busStops.getSingle(function(busStop) { return busStop.id == self.selectedBusStop().id; }));
    };

    self.deleteBusStopDeleteBtnClick = function()
    {
        sendAjaxRequest('/api/BusStop/' + self.selectedBusStop().id, "DELETE", null, function(response)
        {
            self.selectedView(self.views[0]);

            self.busStops.remove(self.selectedBusStop());
            self.selectedBusStop().dispose();
            self.selectedBusStop(null);
        });
    };

    self.deleteBusStopCancelBtnClick = function()
    {
        self.selectedView(self.views[0]);
    };

    self.busLinesListAddBtnClick = function()
    {
        self.selectedBusLine(new BusLine(null, 'Nowa linia', [], self.busStops));
        self.selectedRoute(null);
        self.selectedView(self.views[4]);
    };

    self.addBusLineAddRouteBtnClick = function()
    {
        self.selectedRoute(null);
        self.directions(new GoogleMapsDirections());
    };

    self.addBusLineFinishAddingRouteBtnClick = function()
    {
        self.selectedBusLine().routes.add(new Route(self.selectedBusLine(), 0, '', '', [], self.directions().points()/*points*//*Model*/, self.busStops, true));
        self.selectedRoute(self.selectedBusLine().routes.getLast());

        self.directions().dispose();
        self.directions(null);
    };

    self.addBusLineCancelAddingRouteBtnClick = function()
    {
        self.directions().dispose();
        self.directions(null);
    };

    self.addBusLineAddBtnClick = function()
    {
        var calculateTimes = function(route)
        {
            console.log(route);
            if (route.Points[0].BusStopId && route.Points[route.Points.length - 1].BusStopId)
            {
                
                var start = 0;
                var end = 0;
                while (end < route.Points.length - 1)
                {
                    var totalDistance = 0;
                    var distances = [];

                    do
                    {
                        end++;
                        var diff_X = route.Points[end].Latitude - route.Points[end - 1].Latitude;
                        var diff_Y = route.Points[end].Longitude - route.Points[end - 1].Longitude;
                        var distance = Math.sqrt(diff_X * diff_X + diff_Y * diff_Y);
                        totalDistance += distance;
                        distances.push(distance);
                    }
                    while (!route.Points[end].BusStopId);

                    var totalTime = Number(route.Points[end].TimeOffset) - Number(route.Points[start].TimeOffset);
                    console.log(totalTime);
                    while (++start != end)
                    {
                        route.Points[start].TimeOffset = Number(route.Points[start - 1].TimeOffset) + Math.round(totalTime * distances.shift() / totalDistance);
                    }
                }
            }
        };


        var newBusLine =
        {
            BusNumber: self.selectedBusLine().busNumber(),
            Routes: []
        };
        for (var i = 0; i < self.selectedBusLine().routes.count() ; i++)
        {
            newBusLine.Routes.push(
            {
                IndexMark: self.selectedBusLine().routes.getAt(i).indexMark() || 'Ind',
                Details: self.selectedBusLine().routes.getAt(i).details() || 'Det',
                DepartureTimes: [],
                Points: []
            });
            for (var j = 0; j < self.selectedBusLine().routes.getAt(i).points.count() ; j++)
                newBusLine.Routes[i].Points.push(
                {
                    Latitude: self.selectedBusLine().routes.getAt(i).points.getAt(j).latitude(),
                    Longitude: self.selectedBusLine().routes.getAt(i).points.getAt(j).longitude(),
                    TimeOffset: self.selectedBusLine().routes.getAt(i).points.getAt(j).timeOffset(),
                    BusStopId: self.selectedBusLine().routes.getAt(i).points.getAt(j).busStop() ? self.selectedBusLine().routes.getAt(i).points.getAt(j).busStop().id : null
                });
            calculateTimes(newBusLine.Routes[i]);
        }

        

        console.log(newBusLine);

        sendAjaxRequest('/api/BusLine', "POST", newBusLine, function(response)
        {
            self.selectedView(self.views[0]);
            for (var i = 0; i < self.selectedBusLine().routes.count() ; i++)
                self.selectedBusLine().routes.getAt(i).dispose();
            //self.selectedBusStop().dispose();

            var newBusLine = new BusLine(response.Id, response.BusNumber, response.Routes, self.busStops);
            //var newBusStop = new BusStop(response.Id, response.Name, response.Latitude, response.Longitude, self.busLines);
            //newBusStop.selectBusStopEvent = selectBusStopEvent;
            self.busLines.add(newBusLine);
            self.selectedBusLine(newBusLine);
        });
    };

    self.addBusLineCancelBtnClick = function()
    {
        self.selectedView(self.views[0]);
        self.selectedBusLine(null);
        self.selectedRoute(null);
    };
}