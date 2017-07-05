function BusLinesListViewModel(engine)
{
    var self = this;

    self.title = 'LINIE';

    self.isSelected = ko.observable(false);

    self.isSelected.subscribe(function(newValue)
    {
        if (newValue === true)
        {
            engine.mapClickListener = function()
            {
                engine.selectBusStop(null);
                engine.selectRoute(null);
            };

            engine.busStopClickListener = function(busStop)
            {
                engine.selectBusStop(busStop);
            };
        }
    });

    self.busLines = engine.busLines;
    self.selectedBusLine = engine.selectedBusLine;
    self.selectedRoute = engine.selectedRoute;
    self.selectRoute = engine.selectRoute;

    self.addBusLine = function()
    {
        navigationViewModel.selectView('DODAWANIE LINII');
    };

    self.editBusLine = function(busLine)
    {
        engine.selectBusLine(busLine);
        navigationViewModel.selectView('EDYTOWANIE LINII');
    };

    self.deleteBusLine = function(busLine)
    {
        engine.selectBusLine(busLine);
        var deleteBusLine = confirm('Usunąć linię \"' + busLine.busNumber() + '\"?');
        if (deleteBusLine)
        {
            sendAjaxRequest('/api/BusLine/' + busLine.id, 'DELETE', null, function(response)
            {
                engine.selectBusLine(null);
                engine.busLines.remove(busLine);
                busLine.dispose();

                navigationViewModel.selectView('LINIE');
            });
        }
    };
}