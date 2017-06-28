function DeleteBusStopViewModel(engine)
{
    var self = this;

    self.title = 'USUWANIE PRZYSTANKU';

    self.isSelected = ko.observable(false);

    self.isSelected.subscribe(function(newValue)
    {
        if (newValue === true)
        {
            self.busStopToDelete(engine.selectedBusStop());

            engine.selectedRoute(null);

            engine.mapClickListener = null;

            engine.busStopClickListener = null;
        }
    });

    self.busStopToDelete = ko.observable();

    self.deleteBtnClick = function()
    {
        sendAjaxRequest('/api/BusStop/' + self.busStopToDelete().id, "DELETE", null, function(response)
        {
            engine.selectedBusStop(null);
            engine.busStops.remove(self.busStopToDelete());
            self.busStopToDelete().dispose();

            navigationViewModel.selectView('PRZYSTANKI');
        });
    };

    self.cancelBtnClick = function()
    {
        navigationViewModel.selectView('PRZYSTANKI');
    };
}