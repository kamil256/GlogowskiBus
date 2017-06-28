﻿function AddBusStopViewModel(engine)
{
    var self = this;

    self.title = 'DODAWANIE PRZYSTANKU';

    self.isSelected = ko.observable(false);

    self.isSelected.subscribe(function(newValue)
    {
        if (newValue === true)
        {
            self.newBusStop(new BusStop2(null, engine));

            engine.selectedBusStop(self.newBusStop());
            engine.selectedBusLine(null);

            engine.mapClickListener = function(e)
            {
                self.newBusStop().latitude(e.latLng.lat());
                self.newBusStop().longitude(e.latLng.lng());
            };

            engine.busStopClickListener = null;
        }
    });

    self.newBusStop = ko.observable();

    self.finishAddingBusStop = function()
    {
        sendAjaxRequest('/api/BusStop', 'POST', self.newBusStop().getModel(), function(model)
        {
            var newBusStop = new BusStop2(model, engine);
            engine.busStops.push(newBusStop);
            engine.selectedBusStop(newBusStop);
            self.newBusStop().dispose();
            navigationViewModel.selectView('PRZYSTANKI');
        });
    };

    self.cancelAddingBusStop = function()
    {
        engine.selectedBusStop(null);
        self.newBusStop().dispose();
        navigationViewModel.selectView('PRZYSTANKI');
    };
}