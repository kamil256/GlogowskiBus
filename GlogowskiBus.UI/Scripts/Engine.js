function Engine()
{
    var self = this;
    defineObjects();
    defineDependencies();
    defineEventListeners();
    loadData();

    function defineObjects()
    {
        self.busStops = ko.observableArray([]);

        self.busLines = ko.observableArray([]);

        self.routes = ko.computed(function()
        {
            var result = [];
            for (var i = 0; i < self.busLines().length; i++)
                result = result.concat(self.busLines()[i].routes());
            return result;
        });

        self.departureTimes = ko.computed(function()
        {
            var result = [];
            for (var i = 0; i < self.routes().length; i++)
                result = result.concat(self.routes()[i].departureTimes());
            result.sort(function(departureTime1, departureTime2)
            {
                return departureTime1.minutesSinceMidnight() - departureTime2.minutesSinceMidnight();
            });
            return result;
        });

        self.points = ko.computed(function()
        {
            var result = [];
            for (var i = 0; i < self.routes().length; i++)
                result = result.concat(self.routes()[i].points());
            return result;
        });

        self.buses = ko.observableArray([]);

        self.selectedBusStop = ko.observable();

        self.selectedRoute = ko.observable();

        self.selectedBusLine = ko.observable();

        self.selectedDepartureTime = ko.observable();

        self.selectedDayOfWeek = ko.observable(serverTime.daysOfWeek[0]);

        self.selectedBusStops = ko.computed(function()
        {
            var result = [];

            if (self.selectedRoute())
            {
                var busStopPoint = self.selectedRoute().getBusStopPoint(self.selectedBusStop() || self.selectedRoute().busStops()[0]);
                if (busStopPoint)
                    for (var i = 0; i < self.selectedRoute().points().length; i++)
                        if (self.selectedRoute().points()[i].busStop())
                            result.push(
                            {
                                name: self.selectedRoute().points()[i].busStop().name(),
                                timeOffset: ko.observable((self.selectedRoute().points()[i].timeOffset() - busStopPoint.timeOffset()) / 60000),
                                originalPoint: self.selectedRoute().points()[i]
                            });
            }
            return result;
        });

        self.selectedDepartureTimes = ko.computed(function()
        {
            var result = [];
            if (self.selectedRoute() && self.selectedBusLine())
            {
                var originalDepartureTimes = self.selectedBusStop() ? self.selectedBusLine().departureTimes() : self.selectedRoute().departureTimes();
                for (var i = 0; i < 24; i++)
                    result[i] = [];
                for (var i = 0; i < originalDepartureTimes.length; i++)
                {
                    var busStopPoint = originalDepartureTimes[i].route.getBusStopPoint(self.selectedBusStop() || self.selectedRoute().busStops()[0]);
                    if (busStopPoint)
                    {
                        
                        var minutes = originalDepartureTimes[i].minutes() + busStopPoint.timeOffset() / 60000;
                        var hours = originalDepartureTimes[i].hours() + Math.floor(minutes / 60);
                        var dayOfWeek = originalDepartureTimes[i].dayOfWeek();
                        if (hours > 23)
                            dayOfWeek = serverTime.nextDayOfWeek(dayOfWeek);
                        if (dayOfWeek == self.selectedDayOfWeek())
                        {
                            hours %= 24;
                            minutes %= 60;
                            result[hours].push(
                            {
                                minutes: minutes,
                                originalDepartureTime: originalDepartureTimes[i]
                            });
                        }
                    }
                }
            }
            return result;
        });
    }

    function defineDependencies()
    {
        self.selectedBusStop.subscribe(function(newValue)
        {
            if (newValue && self.selectedRoute() && self.selectedRoute().busStops().indexOf(newValue) === -1)
                self.selectRoute(null);
        });

        //self.selectedDepartureTime.subscribe(function(newValue)
        //{
        //    if (newValue)
        //    {
        //        self.selectedRoute(newValue.route);
        //        self.selectedBusLine(newValue.route.busLine);
        //    }
        //    else
        //    {

        //    }
        //});

        self.selectedRoute.subscribe(function(newValue)
        {
            if (newValue && self.selectedBusStop() && self.selectedBusStop().routes().indexOf(newValue) === -1)
                self.selectBusStop(null);

            //if (newValue && self.selectedBusLine() !== newValue.busLine)
            //    self.selectedBusLine(newValue.busLine);
            //else if (!newValue && self.selectedBusLine())
            //    self.selectedBusLine(null);
        });

        //self.selectedBusLine.subscribe(function(newValue)
        //{
        //    if (!newValue || newValue.routes().length == 0)
        //    {
        //        if (self.selectedRoute())
        //            self.selectedRoute(null);
        //    }
        //    else
        //    {
        //        if (!self.selectedRoute() || (self.selectedRoute() && self.selectedRoute().busLine != newValue))
        //            self.selectedRoute(newValue.routes()[0]);
        //    }
        //});

        self.selectBusStop = function(newBusStop)
        {
            self.selectedBusStop(newBusStop);

            //if (newBusStop && self.selectedRoute() && self.selectedRoute().busStops().indexOf(newBusStop) === -1)
            //    self.selectedRoute(null);
        };

        self.selectRoute = function(newRoute)
        {
            self.selectedRoute(newRoute);
            self.selectedBusLine(self.selectedRoute() ? self.selectedRoute().busLine : null);
            self.selectedDepartureTime(self.selectedRoute() ? self.selectedRoute().getNextDepartureTime() : null);
        };

        self.selectBusLine = function(newBusLine)
        {
            self.selectedBusLine(newBusLine);
            if (newBusLine && newBusLine.routes().length > 0)
            {
                self.selectedDepartureTime(newBusLine.getNextDepartureTime());
                self.selectedRoute(self.selectedDepartureTime() ? self.selectedDepartureTime().route : newBusLine.routes()[0]);
            }
            else
            {
                self.selectedDepartureTime(null);
                self.selectedRoute(null);
            }
            
        };

        self.selectDepartureTime = function(newDepartureTime)
        {
            
            self.selectedDepartureTime(newDepartureTime);
            self.selectedRoute(self.selectedDepartureTime() ? self.selectedDepartureTime().route : null);

            
            self.selectedBusLine(self.selectedRoute() ? self.selectedRoute().busLine : null);
        };
    }

    function defineEventListeners()
    {
        self.mapClickListener = null;

        map.addListener('click', function(e)
        {
            if (self.mapClickListener)
                self.mapClickListener(e);
        });

        self.busStopClickListener = null;
    }

    function loadData()
    {
        sendAjaxRequest('/api/BusStop', 'GET', null, function(model)
        {
            for (var i = 0; i < model.length; i++)
            {
                var busStop = new BusStop2(model[i], self);
                self.busStops.push(busStop);
            }

            sendAjaxRequest('/api/BusLine', 'GET', null, function(model)
            {
                for (var i = 0; i < model.length; i++)
                    self.busLines.push(new BusLine2(model[i], self));
                updateBuses();
            });
        });
    }

    function updateBuses()
    {
        console.log('update buses');
        for (var i = 0; i < self.departureTimes().length; i++)
            if (self.departureTimes()[i].isOnTour())
            {
                console.log('sdsdf');
                var bus = null;
                for (var j = 0; j < self.buses().length; j++)
                    if (self.buses()[j].departureTime === self.departureTimes()[i])
                    {
                        bus = self.buses()[j];
                        break;
                    }

                if (!bus)
                {
                    bus = new Bus(self.departureTimes()[i]);
                    bus.selectBusEvent = function(departureTime)
                    {
                        self.selectedDepartureTime(departureTime);
                    };
                    bus.removeBusEvent = function(bus)
                    {
                        self.buses.remove(bus);
                    };
                    self.buses.push(bus);
                }
            }

        setInterval(function()
        {
            if (serverTime.now().getSeconds() === 0)
                updateBuses();
        }, 1000);
    };    
}