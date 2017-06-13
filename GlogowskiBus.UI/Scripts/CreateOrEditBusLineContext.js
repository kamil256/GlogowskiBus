function CreateOrEditBusLineContext(busLineId)
{
    var self = this;

    self.busStops = new Collection();
    sendAjaxRequest('/api/BusStop', "GET", null, function(response)
    {
        for (var i = 0; i < response.length; i++)
            self.busStops.add(new BusStop(response[i].Name, response[i].Latitude, response[i].Longitude));
    });

    if (busLineId)
    {
        sendAjaxRequest('/api/BusLine', "GET", { id: busLineId }, function(response)
        {
            console.log(response);
            self.busLine = new BusLine(response.BusNumber, response.Routes, self.busStops);
        });
    }
    else
    {
        self.busLine = new BusLine('New bus number', [], self.busStops);
    }

    self.tabs = [busLineId ? 'NOWA LINIA' : 'EDYTOWANA LINIA', 'PRZYSTANKI', 'ROZKŁAD JAZDY'];

    self.selectedTab = ko.observable(self.tabs[0]);

    self.selectedRoute = ko.observable();
}