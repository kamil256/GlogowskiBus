function CreateOrEditBusLineContext(busLineId)
{
    var self = this;

    self.busStops = new Collection();
    self.busLine = ko.observable();
    self.directions = ko.observable();

    sendAjaxRequest('/api/BusStop', "GET", null, function(response)
    {
        for (var i = 0; i < response.length; i++)
            self.busStops.add(new BusStop(response[i].Name, response[i].Latitude, response[i].Longitude));

        for (var i = 0; i < self.busStops.count(); i++)
        {
            self.busStops.getAt(i).selectBusStopEvent = function(busStop)
            {
                if (self.directions())
                    self.directions().addNewPoint(new google.maps.LatLng(busStop.latitude, busStop.longitude));
            };
        }

        if (busLineId)
        {
            sendAjaxRequest('/api/BusLine', "GET", { id: busLineId }, function(response)
            {
                self.busLine(new BusLine(response.BusNumber, response.Routes, self.busStops));
            });
        }
        else
        {
            self.busLine(new BusLine('New bus number', [], self.busStops));
        }
    });

    self.tabs = [busLineId ? 'NOWA LINIA' : 'EDYTOWANA LINIA', 'PRZYSTANKI', 'ROZKŁAD JAZDY'];

    self.selectedTab = ko.observable(self.tabs[0]);

    self.selectedRoute = ko.observable();

    self.addNewRoute = function(points)
    {
        var pointsModel = [];
        for (var i = 0; i < points.length; i++)
        {
            pointsModel.push(
            {
                Latitude: points[i].latitude,
                Longitude: points[i].longitude,
                TimeOffset: 0,
                BusStop: null
            });
        }
        self.busLine().routes.add(new Route(self.busLine(), '', '', [], points/*Model*/, self.busStops));
    };

    self.selectedRoute.subscribe(function(newRoute)
    {
        for (var i = 0; i < self.busLine().routes.count() ; i++)
            if (newRoute && self.busLine().routes.getAt(i) == newRoute)
                self.busLine().routes.getAt(i).select();
            else
                self.busLine().routes.getAt(i).deselect();
    });
}