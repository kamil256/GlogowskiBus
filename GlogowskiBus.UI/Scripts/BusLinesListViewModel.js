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
                engine.selectedBusStop(null);
                engine.selectedRoute(null);
            };

            engine.busStopClickListener = function(busStop)
            {
                engine.selectedBusStop(busStop);
            };
        }
    });

    self.busLines = engine.busLines;

    self.selectedRoute = engine.selectedRoute;
    self.selectedBusLine = engine.selectedBusLine;

    self.addBusLineBtnClick = function()
    {
        navigationViewModel.selectView('DODAWANIE LINII');
    };

    self.editBusLineBtnClick = function(busLine)
    {
        engine.selectedBusLine(busLine);

        navigationViewModel.selectView('EDYTOWANIE LINII');
    };

    self.deleteBusLineBtnClick = function(busLine)
    {
        //engine.selectedBusStop(busStop);

        //navigationViewModel.selectView('USUWANIE LINII');
    };
}