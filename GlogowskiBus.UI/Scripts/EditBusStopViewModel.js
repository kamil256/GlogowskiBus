function EditBusStopViewModel(engine)
{
    var self = this;

    self.title = 'EDYTOWANIE PRZYSTANKU';

    self.isSelected = ko.observable(false);

    self.isSelected.subscribe(function(newValue)
    {
        if (newValue === true)
        {
            self.originalBusStop = engine.selectedBusStop();
            self.originalBusStop.isVisible(false);

            self.editedBusStop(new BusStop2(self.originalBusStop.getModel(), engine));

            engine.selectBusStop(self.editedBusStop());
            engine.selectBusLine(null);

            engine.mapClickListener = function(e)
            {
                self.editedBusStop().latitude(e.latLng.lat());
                self.editedBusStop().longitude(e.latLng.lng());
            };

            engine.busStopClickListener = null;
        }
    });

    self.editedBusStop = ko.observable();

    self.finishEditingBusStop = function()
    {
        sendAjaxRequest('/api/BusStop', 'PUT', self.editedBusStop().getModel(), function(model)
        {
            var editedBusStop = new BusStop2(model, engine);
            engine.busStops.splice(engine.busStops.indexOf(self.originalBusStop), 1, editedBusStop);
            engine.selectBusStop(editedBusStop);
            self.originalBusStop.dispose();
            self.editedBusStop().dispose();
            navigationViewModel.selectView('PRZYSTANKI');
        });
    };

    self.cancelEditingBusStop = function()
    {
        self.originalBusStop.isVisible(true);
        engine.selectBusStop(self.originalBusStop);
        self.editedBusStop().dispose();
        navigationViewModel.selectView('PRZYSTANKI');
    };
}