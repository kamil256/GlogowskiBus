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
                engine.selectBusStop(null);
                engine.selectRoute(null);
            };

            engine.busStopClickListener = function(busStop)
            {
                engine.selectBusStop(busStop);
            };
        }
    });

    self.busStops = engine.busStops;
    self.selectedBusStop = engine.selectedBusStop;
    self.selectBusStop = engine.selectBusStop;

    self.addBusStopBtnClick = function()
    {
        navigationViewModel.selectView('DODAWANIE PRZYSTANKU');
    };

    self.editBusStopBtnClick = function(busStop)
    {
        engine.selectBusStop(busStop);

        navigationViewModel.selectView('EDYTOWANIE PRZYSTANKU');
    };

    self.deleteBusStopBtnClick = function(busStop)
    {
        engine.selectBusStop(busStop);

        navigationViewModel.selectView('USUWANIE PRZYSTANKU');
    };
}