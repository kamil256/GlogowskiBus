function AddBusStopViewModel(engine)
{
    var self = this;

    self.title = 'DODAWANIE PRZYSTANKU';

    self.isSelected = ko.observable(false);

    self.isSelected.subscribe(function(newValue)
    {
        if (newValue === true)
        {
            self.newBusStop(new BusStop(null, engine));

            engine.selectBusStop(self.newBusStop());
            engine.selectBusLine(null);

            engine.mapClickListener = function(e)
            {
                self.newBusStop().position(e.latLng);
            };

            engine.busStopClickListener = null;
        }
    });

    self.newBusStop = ko.observable();

    self.changeLatitudeOfNewBusStop = function(value)
    {
        self.newBusStop().position(new google.maps.LatLng(value, self.newBusStop().position().lng()));
    };

    self.changeLongitudeOfNewBusStop = function(value)
    {
        self.newBusStop().position(new google.maps.LatLng(self.newBusStop().position().lat(), value));
    };

    self.finishAddingBusStop = function()
    {
        sendAjaxRequest('/api/BusStop', 'POST', self.newBusStop().getModel(), function(model)
        {
            var newBusStop = new BusStop(model, engine);
            engine.busStops.push(newBusStop);
            engine.selectBusStop(newBusStop);
            self.newBusStop().dispose();
            navigationViewModel.selectView('PRZYSTANKI');
        });
    };

    self.cancelAddingBusStop = function()
    {
        engine.selectBusStop(null);
        self.newBusStop().dispose();
        navigationViewModel.selectView('PRZYSTANKI');
    };
}