function AdministrationContext(busStopsFromModel, busLinesFromModel)
{
    var self = this;

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

    self.tabs = ['PRZYSTANKI', 'LINIE'];

    self.selectedTab = ko.observable(self.tabs[0]);

    self.selectedBusStop = ko.observable();

    for (var i = 0; i < self.busStops.count(); i++)
        self.busStops.getAt(i).selectBusStopEvent = function(busStop)
        {
            self.selectedBusStop(busStop);
        };

    self.selectedBusStop.subscribe(function(newBusStop)
    {
        for (var i = 0; i < self.busStops.count() ; i++)
            if (self.busStops.getAt(i) != newBusStop)
                self.busStops.getAt(i).deselect();
            else
                self.busStops.getAt(i).select();
    });

    self.selectedRoute = ko.observable();

    self.selectedRoute.subscribe(function(newRoute)
    {
        for (var i = 0; i < self.routes.count() ; i++)
            if (newRoute && self.routes.getAt(i) == newRoute)
                self.routes.getAt(i).select();
            else
                self.routes.getAt(i).deselect();
    });

    map.addListener('click', function()
    {
        self.selectedBusStop(null);
        self.selectedRoute(null);
    });

    self.busLinesNumbersListForBusStop = function(busStop)
    {
        var busLinesNumbers = [];
        for (var i = 0; i < busStop.points.count() ; i++)
            if (busLinesNumbers.indexOf(busStop.points.getAt(i).busNumber) == -1)
                busLinesNumbers.push(busStop.points.getAt(i).busNumber);
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
}