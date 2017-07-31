function TimeTableViewModel(engine)
{
    var self = this;

    self.title = 'ROZKŁAD JAZDY';

    self.isSelected = ko.observable(false);

    self.isSelected.subscribe(function(newValue)
    {
        if (newValue === true)
        {
            self.selectedDayOfWeek(serverTime.dayOfWeekToday());

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
    self.busLines = engine.busLines;
    self.buses = engine.buses;
    self.selectedBusStop = engine.selectedBusStop;
    self.selectBusStop = engine.selectBusStop;
    self.selectedBusLine = engine.selectedBusLine;
    self.selectedRoute = engine.selectedRoute;
    self.selectedDepartureTime = engine.selectedDepartureTime;
    self.selectBusLine = engine.selectBusLine;
    self.selectDepartureTime = engine.selectDepartureTime;
    self.selectedDayOfWeek = engine.selectedDayOfWeek;
    self.selectedBusStops = engine.selectedBusStops
    self.departureTimesOfSelectedBusLine = engine.departureTimesOfSelectedBusLine;
}