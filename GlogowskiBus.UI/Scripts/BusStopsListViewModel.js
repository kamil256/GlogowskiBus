function BusStopsListViewModel(engine)
{
    var self = this;

    self.title = 'PRZYSTANKI';

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

    self.busStops = engine.busStops;

    self.selectedBusStop = engine.selectedBusStop;

    self.addBusStopBtnClick = function()
    {
        navigationViewModel.selectView('DODAWANIE PRZYSTANKU');
    };

    self.editBusStopBtnClick = function(busStop)
    {
        engine.selectedBusStop(busStop);

        navigationViewModel.selectView('EDYTOWANIE PRZYSTANKU');
    };

    self.deleteBusStopBtnClick = function(busStop)
    {
        engine.selectedBusStop(busStop);

        navigationViewModel.selectView('USUWANIE PRZYSTANKU');
    };
}