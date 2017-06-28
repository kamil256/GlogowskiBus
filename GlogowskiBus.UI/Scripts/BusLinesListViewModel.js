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

    self.selectedRoute = engine.selectedRoute;
    self.selectRoute = engine.selectRoute;
    self.selectedBusLine = engine.selectedBusLine;

    self.addBusLineBtnClick = function()
    {
        navigationViewModel.selectView('DODAWANIE LINII');
    };

    self.editBusLineBtnClick = function(busLine)
    {
        engine.selectBusLine(busLine);

        navigationViewModel.selectView('EDYTOWANIE LINII');
    };

    self.deleteBusLineBtnClick = function(busLine)
    {
        //engine.selectedBusStop(busStop);

        //navigationViewModel.selectView('USUWANIE LINII');
    };
}