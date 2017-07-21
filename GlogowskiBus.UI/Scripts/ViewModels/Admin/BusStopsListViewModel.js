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
    self.busNumbersForBusStop = engine.busNumbersForBusStop;



    self.busStops = engine.busStops;

    self.addBusStop = function()
    {
        navigationViewModel.selectView('DODAWANIE PRZYSTANKU');
    };

    self.editBusStop = function(busStop)
    {
        engine.selectBusStop(busStop);
        navigationViewModel.selectView('EDYTOWANIE PRZYSTANKU');
    };

    self.deleteBusStop = function(busStop)
    {
        engine.selectBusStop(busStop);
        var deleteBusStop = confirm('Usunąć przystanek \"' + busStop.name() + '\"?');
        if (deleteBusStop)
        {
            sendAjaxRequest('/api/BusStop/' + busStop.id, 'DELETE', null, function(response)
            {
                if (response.Message)
                {
                    alert(response.Message);
                }
                else
                {
                    engine.selectBusStop(null);
                    engine.busStops.remove(busStop);
                    busStop.dispose();

                    navigationViewModel.selectView('PRZYSTANKI');
                }
            });
        }
    };
}