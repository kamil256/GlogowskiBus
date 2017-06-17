function AdministrationContext(busStopsFromModel, busLinesFromModel)
{
    var self = this;

    self.busStops = new Collection();
    for (var i = 0; i < busStopsFromModel.length; i++)
        self.busStops.add(new BusStop(busStopsFromModel[i].Name, busStopsFromModel[i].Latitude, busStopsFromModel[i].Longitude));

    self.busLines = new Collection();
    for (var i = 0; i < busLinesFromModel.length; i++)
        self.busLines.add(new BusLine(busLinesFromModel[i].BusNumber, busLinesFromModel[i].Routes, self.busStops));
    for (var i = 0; i < busLinesFromModel.length; i++)
        self.busLines.add(new BusLine(busLinesFromModel[i].BusNumber, busLinesFromModel[i].Routes, self.busStops));
    for (var i = 0; i < busLinesFromModel.length; i++)
        self.busLines.add(new BusLine(busLinesFromModel[i].BusNumber, busLinesFromModel[i].Routes, self.busStops));
    for (var i = 0; i < busLinesFromModel.length; i++)
        self.busLines.add(new BusLine(busLinesFromModel[i].BusNumber, busLinesFromModel[i].Routes, self.busStops));

    self.routes = new Collection();
    for (var i = 0; i < self.busLines.count(); i++)
        self.routes.addMany(self.busLines.getAt(i).routes.toArray());

    self.tabs = ['PRZYSTANKI', 'LINIE'];

    self.selectedTab = ko.observable(self.tabs[0]);

    self.selectedRoute = ko.observable();

    map.addListener('click', function()
    {
        self.selectedRoute(null);
    });

    self.selectedRoute.subscribe(function(newRoute)
    {
        for (var i = 0; i < self.routes.count(); i++)
            if (newRoute && self.routes.getAt(i) == newRoute)
                self.routes.getAt(i).select();
            else
                self.routes.getAt(i).deselect();
    });
}