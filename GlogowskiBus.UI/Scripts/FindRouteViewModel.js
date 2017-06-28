function FindRouteViewModel(engine)
{
    var self = this;

    self.title = 'SZUKAJ POŁĄCZEŃ';

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

    /***************************** Below: old TimeTableContext ********************************/

    //self.buses = new Collection();//Buses();
    //function updateBuses()
    //{
    //    for (var i = 0; i < self.departureTimes.count() ; i++)
    //        if (self.departureTimes.getAt(i).isOnTour() && !self.buses.getSingle(function(bus) { return bus.departureTime == self.departureTimes.getAt(i); }))
    //        {
    //            var bus = new Bus(self.departureTimes.getAt(i));
    //            bus.selectBusEvent = function(departureTime)
    //            {
    //                self.selectedDepartureTime(departureTime);
    //            };
    //            bus.removeBusEvent = function(bus)
    //            {
    //                self.buses.remove(bus);
    //            };
    //            self.buses.add(bus);
    //        }
    //};
    //updateBuses();
    //setInterval(function()
    //{
    //    if (serverTime.now().getSeconds() == 0)
    //        updateBuses();
    //}, 1000);

    //self.tabs = ['ROZKŁAD JAZDY', 'SZUKAJ POŁĄCZEŃ'];

    //self.selectedTab = ko.observable(self.tabs[0]);
    
    //self.selectedBusStop = ko.observable();

    //for (var i = 0; i < self.busStops.count(); i++)
    //{
    //    self.busStops.getAt(i).selectBusStopEvent = function(busStop)
    //    {
    //        self.selectedBusStop(busStop);
    //    };
    //}

    //for (var i = 0; i < self.routes.count(); i++)
    //{
    //    self.routes.getAt(i).selectBusStopEvent = function(busStop)
    //    {
    //        self.selectedBusStop(busStop);
    //    };
    //}

    //self.selectedDepartureTime = ko.observable();

    //map.addListener('click', function()
    //{
    //    self.selectedBusStop(null);
    //    self.selectedDepartureTime(null);
    //});

    //self.selectedDayOfWeek = ko.observable(serverTime.dayOfWeekToday());

    //self.selectedBusStops = ko.computed(function()
    //{
    //    var busStops = [];
    //    if (self.selectedBusStop() && self.selectedDepartureTime())
    //    {
    //        var busStopPoint = self.selectedDepartureTime().route.points.getSingle(function(point) 
    //        {
    //            return point.busStop === self.selectedBusStop(); 
    //        });
    //        if (busStopPoint)
    //        {
    //            for (var i = 0; i < self.selectedDepartureTime().route.points.count() ; i++)
    //                if (self.selectedDepartureTime().route.points.getAt(i).busStop != null)
    //                {
    //                    busStops.push(
    //                    {
    //                        name: self.selectedDepartureTime().route.points.getAt(i).busStop.name,
    //                        timeOffset: (self.selectedDepartureTime().route.points.getAt(i).timeOffset - busStopPoint.timeOffset) / 60000
    //                    });
    //                }
    //        }
    //    }
    //    return busStops;
    //});

    //self.selectedDepartureTimes = ko.computed(function()
    //{
    //    var departureTimes = [];
    //    if (self.selectedBusStop() && self.selectedDepartureTime())
    //    {
    //        for (var i = 0; i < 24; i++)
    //            departureTimes[i] = [];
    //        var busLine = self.selectedDepartureTime().route.busLine;
    //        for (var i = 0; i < busLine.departureTimes.count() ; i++)
    //        {
    //            var busStopPoint = busLine.departureTimes.getAt(i).route.points.getSingle(function(point)
    //            {
    //                return point.busStop === self.selectedBusStop();
    //            });
    //            if (busStopPoint)
    //            {
    //                var minutes = busLine.departureTimes.getAt(i).minutes + busStopPoint.timeOffset / 60000;
    //                var hours = busLine.departureTimes.getAt(i).hours + Math.floor(minutes / 60);
    //                var dayOfWeek = busLine.departureTimes.getAt(i).dayOfWeek;
    //                if (hours > 23) 
    //                    dayOfWeek = serverTime.nextDayOfWeek(dayOfWeek);
    //                if (dayOfWeek == self.selectedDayOfWeek())
    //                {
    //                    hours %= 24;
    //                    minutes %= 60;

    //                    departureTimes[hours].push(
    //                    {
    //                        minutes: minutes,
    //                        departureTime: busLine.departureTimes.getAt(i)
    //                    });
    //                }
    //            }
    //        }
    //    }
    //    return departureTimes;
    //});

    //self.selectedBusStop.subscribe(function(newBusStop)
    //{
    //    for (var i = 0; i < self.busStops.count(); i++)
    //        if (self.busStops.getAt(i) != newBusStop)
    //            self.busStops.getAt(i).deselect();
    //        else
    //            self.busStops.getAt(i).select();

    //    if (self.selectedDepartureTime() != null)
    //        self.selectedDepartureTime().route.selectBusStop(newBusStop);

    //    if (newBusStop != null && self.selectedDepartureTime() != null && !self.selectedDepartureTime().route.busLine.busStops.getSingle(function(busStop) { return busStop == newBusStop }))
    //        self.selectedDepartureTime(null);

    //    if (self.selectedDepartureTime() != null)
    //        self.selectedDepartureTime(self.getNextDepartureTime(self.selectedDepartureTime().route.busLine));
    //});

    //self.selectedDepartureTime.subscribe(function(newDepartureTime)
    //{
    //    for (var i = 0; i < self.routes.count(); i++)
    //        if (newDepartureTime != null && self.routes.getAt(i) == newDepartureTime.route)
    //            self.routes.getAt(i).select();
    //        else
    //            self.routes.getAt(i).deselect();

    //    if (newDepartureTime != null)
    //        newDepartureTime.route.selectBusStop(self.selectedBusStop());
    //    else
    //        self.selectedDayOfWeek(serverTime.dayOfWeekToday());

    //    if (newDepartureTime != null && self.selectedBusStop() != null && !newDepartureTime.route.busLine.busStops.getSingle(function(busStop) { return busStop == self.selectedBusStop() }))//!newDepartureTime.route.busLine.containsBusStop(self.busStop()))
    //        self.selectedBusStop(null);
    //});

    //self.selectedDayOfWeek.subscribe(function(selectedDayOfWeek)
    //{
    //    if (self.selectedDepartureTime() != null)
    //        self.selectedDepartureTime(self.getNextDepartureTime(self.selectedDepartureTime().route.busLine, self.selectedBusStop()));
    //});

    //self.getNextDepartureTime = function(busLine)
    //{
    //    var minutesSinceMidnightNow = 60 * serverTime.now().getHours() + serverTime.now().getMinutes();

    //    var daysOfWeek =
    //    [
    //        {
    //            dayOfWeek: serverTime.dayOfWeekYesterday(),
    //            minutesSinceMidnightOffset: -24 * 60
    //        },
    //        {
    //            dayOfWeek: serverTime.dayOfWeekToday(),
    //            minutesSinceMidnightOffset: 0
    //        },
    //        {
    //            dayOfWeek: serverTime.dayOfWeekTomorrow(),
    //            minutesSinceMidnightOffset: 24 * 60
    //        }
    //    ];

    //    for (var i = 0; i < daysOfWeek.length; i++)
    //    {
    //        var departureTimesForDayOfWeek = busLine.departureTimes.get(function(departureTime)
    //        {
    //            return departureTime.dayOfWeek == daysOfWeek[i].dayOfWeek;
    //        });

    //        for (var j = 0; j < departureTimesForDayOfWeek.count(); j++)
    //        {
    //            var busStopPoint = departureTimesForDayOfWeek.getAt(j).route.points.getSingle(function(point)
    //            {
    //                return point.busStop === self.selectedBusStop();
    //            });
    //            if (busStopPoint)
    //            {
    //                var minutesSinceMidnight = departureTimesForDayOfWeek.getAt(j).getMinutesSinceMidnight() + Math.floor(busStopPoint.timeOffset / 60000) + daysOfWeek[i].minutesSinceMidnightOffset;
    //                if (minutesSinceMidnight > minutesSinceMidnightNow)
    //                    return departureTimesForDayOfWeek.getAt(j);
    //            }
    //        }
    //    }

    //    return null;
    //};
}