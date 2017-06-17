function AdministrationContext(busStopsFromModel, busLinesFromModel)
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
        new View('DeleteBusStop', ['USUWANIE PRZYSTANKU'])
    ];

    self.selectedView = ko.observable(self.views[0]);

    self.busStops = new Collection();
    for (var i = 0; i < busStopsFromModel.length; i++)
        self.busStops.add(new BusStop(busStopsFromModel[i].Id, busStopsFromModel[i].Name, busStopsFromModel[i].Latitude, busStopsFromModel[i].Longitude));

    self.busLines = new Collection();
    for (var i = 0; i < busLinesFromModel.length; i++)
        self.busLines.add(new BusLine(busLinesFromModel[i].Id, busLinesFromModel[i].BusNumber, busLinesFromModel[i].Routes, self.busStops));
    for (var i = 0; i < busLinesFromModel.length; i++)
        self.busLines.add(new BusLine(busLinesFromModel[i].Id, busLinesFromModel[i].BusNumber, busLinesFromModel[i].Routes, self.busStops));
    for (var i = 0; i < busLinesFromModel.length; i++)
        self.busLines.add(new BusLine(busLinesFromModel[i].Id, busLinesFromModel[i].BusNumber, busLinesFromModel[i].Routes, self.busStops));
    for (var i = 0; i < busLinesFromModel.length; i++)
        self.busLines.add(new BusLine(busLinesFromModel[i].Id, busLinesFromModel[i].BusNumber, busLinesFromModel[i].Routes, self.busStops));

    self.routes = new Collection();
    for (var i = 0; i < self.busLines.count(); i++)
        self.routes.addMany(self.busLines.getAt(i).routes.toArray());

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
    for (var i = 0; i < self.busStops.count() ; i++)
        self.busStops.getAt(i).selectBusStopEvent = selectBusStopEvent;

    self.selectedBusStop.subscribe(function(newBusStop)
    {
        for (var i = 0; i < self.busStops.count() ; i++)
            self.busStops.getAt(i).deselect();
        if (newBusStop)
            newBusStop.select();
    });

    self.selectedRoute = ko.observable();

    self.selectedRoute.subscribe(function(newRoute)
    {
        for (var i = 0; i < self.routes.count(); i++)
            self.routes.getAt(i).deselect();
        if (newRoute)
            newRoute.select();
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
                self.selectedBusStop().dispose();
                self.selectedBusStop(new BusStop(null, self.selectedBusStop().name, e.latLng.lat(), e.latLng.lng()));
                break;
        }
    });

    self.busLinesNumbersListForBusStop = function(busStop)
    {
        var busLinesNumbers = [];
        for (var i = 0; i < busStop.points.count() ; i++)
            if (busLinesNumbers.indexOf(busStop.points.getAt(i).route.busLine.busNumber) == -1)
                busLinesNumbers.push(busStop.points.getAt(i).route.busLine.busNumber);
        busLinesNumbers.sort(function(a, b)
        {
            if (a > b)
                return 1;
            else if (a < b)
                return -1;
            else
                return 0;
        });
        if (busLinesNumbers.length > 0)
            return busLinesNumbers.join(', ');
        else
            return '-';
    };

    self.busStopsListAddBtnClick = function()
    {
        self.selectedView(self.views[1]);
        self.selectedBusStop(new BusStop(null, 'Nowy przystanek', 0, 0));
    };

    self.busStopsListDeleteBtnClick = function()
    {
        if (self.selectedBusStop())
            self.selectedView(self.views[3]);
    };

    self.addBusStopAddBtnClick = function()
    {
        sendAjaxRequest('/api/BusStop', "POST",
        {
            Name: self.selectedBusStop().name,
            Latitude: self.selectedBusStop().latitude,
            Longitude: self.selectedBusStop().longitude
        }, function(response)
        {
            self.selectedBusStop().dispose();

            var newBusStop = new BusStop(response.Id, response.Name, response.Latitude, response.Longitude);
            newBusStop.selectBusStopEvent = selectBusStopEvent;
            self.busStops.add(newBusStop);
            self.selectedBusStop(newBusStop);

            self.selectedView(self.views[0]);
        });
    };

    self.addBusStopCancelBtnClick = function()
    {
        self.selectedBusStop().dispose();
        self.selectedBusStop(null);

        self.selectedView(self.views[0]);
    };

    self.deleteBusStopDeleteBtnClick = function()
    {
        sendAjaxRequest('/api/BusStop/' + self.selectedBusStop().id, "DELETE", null, function(response)
        {
            self.busStops.remove(self.selectedBusStop());
            self.selectedBusStop().dispose();
            self.selectedBusStop(null);

            self.selectedView(self.views[0]);
        });
    };

    self.deleteBusStopCancelBtnClick = function()
    {
        self.selectedView(self.views[0]);
    };
}