function Engine(disableBuses, busStopsAlwaysVisible)
{
    var self = this;

    self.busStopsAlwaysVisible = busStopsAlwaysVisible;

    defineObjects();
    defineDependencies();
    defineEventListeners();
    loadData();

    function defineObjects()
    {
        self.busStops = ko.observableArray([]);

        self.busLines = ko.observableArray([]);

        self.routes = null;

        self.departureTimes = null;

        self.points = null;

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

        self.departureTimesOfRoute = function(route)
        {
            var result = [];
            if (route)
            {
                var busStopPoint = route.getBusStopPoint(self.selectedBusStop() || (route.busStops().length > 0 ? route.busStops()[0] : null));
                if (busStopPoint)
                {
                    for (var i = 0; i < 24; i++)
                        result[i] = [];
                    for (var i = 0; i < route.departureTimes().length; i++)
                    {
                        if (busStopPoint)
                        {
                            var minutes = route.departureTimes()[i].minutes() + busStopPoint.timeOffset() / 60000;
                            var hours = route.departureTimes()[i].hours() + Math.floor(minutes / 60);
                            var dayOfWeek = route.departureTimes()[i].dayOfWeek();
                            if (hours > 23)
                                dayOfWeek = serverTime.nextDayOfWeek(dayOfWeek);
                            if (dayOfWeek == self.selectedDayOfWeek())
                            {
                                hours %= 24;
                                minutes %= 60;
                                result[hours].push(
                                {
                                    minutes: minutes,
                                    originalDepartureTime: route.departureTimes()[i]
                                });
                            }
                        }
                    }
                    for (var i = 0; i < 24; i++)
                    {
                        result[i] = result[i].sort(function(hours1, hours2)
                        {
                            return hours1.minutes - hours2.minutes;
                        });
                    }
                }
            }
            return result;
        };

        self.departureTimesOfSelectedRoute = ko.computed(function()
        {
            return self.departureTimesOfRoute(self.selectedRoute());
        });

        self.departureTimesOfSelectedBusLine = ko.computed(function()
        {
            var result = [];
            if (self.selectedBusLine())
            {
                for (var i = 0; i < 24; i++)
                    result[i] = [];
                for (var i = 0; i < self.selectedBusLine().routes().length; i++)
                {
                    var departureTimesOfRoute = self.departureTimesOfRoute(self.selectedBusLine().routes()[i]);
                    for (var j = 0; j < departureTimesOfRoute.length; j++)
                        result[j] = result[j].concat(departureTimesOfRoute[j]);
                }
                for (var i = 0; i < 24; i++)
                {
                    result[i] = result[i].sort(function(hours1, hours2)
                    {
                        return hours1.minutes - hours2.minutes;
                    });
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
            {
                var newRoute = null;
                for (var i = 0; i < self.selectedBusLine().routes().length; i++)
                    if (self.selectedBusLine().routes()[i].busStops().indexOf(newValue) !== -1)
                    {
                        newRoute = self.selectedBusLine().routes()[i];
                        break;
                    }
                self.selectRoute(newRoute);
            }
        });

        self.selectedRoute.subscribe(function(newValue)
        {
            if (newValue && self.selectedBusStop() && self.selectedRoute().busStops().indexOf(self.selectedBusStop()) === -1)//self.selectedBusStop().routes().indexOf(newValue) === -1)
                self.selectBusStop(null);
        });

        self.selectBusStop = function(newBusStop)
        {
            self.selectedBusStop(newBusStop);
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

        self.busNumbersForBusStop = function(busStop)
        {
            var busNumbers = [];
            for (var i = 0; i < self.busLines().length; i++)
            {
                if (self.busLines()[i].busStops().indexOf(busStop) !== -1 && busNumbers.indexOf(self.busLines()[i].busNumber()) === -1)
                {
                    busNumbers.push(self.busLines()[i].busNumber());
                }
            }
            return busNumbers;
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
            var start = new Date().getTime();
            for (var i = 0; i < model.length; i++)
            {
                var busStop = new BusStop(model[i], self);
                self.busStops.push(busStop);
            }

            sendAjaxRequest('/api/BusLine', 'GET', null, function(model)
            {
                for (var i = 0; i < model.length; i++)
                    self.busLines.push(new BusLine(model[i], self));

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
                    return result;
                });

                self.points = ko.computed(function()
                {
                    var result = [];
                    for (var i = 0; i < self.routes().length; i++)
                        result = result.concat(self.routes()[i].points());
                    return result;
                });

                if (!disableBuses)
                {
                    updateBuses();

                    setInterval(function()
                    {
                        if (serverTime.now().getSeconds() === 0)
                            updateBuses();
                    }, 1000);

                    setInterval(function()
                    {
                        for (var i = 0; i < self.buses().length; i++)
                            self.buses()[i].update();
                    }, 500);
                }
                
                var end = new Date().getTime();
                console.log(end - start);
            });
        });
        
    }

    function updateBuses()
    {
        for (var i = 0; i < self.departureTimes().length; i++)
            if (self.departureTimes()[i].isOnTour())
            {
                var bus = null;
                for (var j = 0; j < self.buses().length; j++)
                    if (self.buses()[j].departureTime === self.departureTimes()[i])
                    {
                        bus = self.buses()[j];
                        break;
                    }

                if (!bus)
                {
                    bus = new Bus(self.departureTimes()[i], self);
                    self.buses.push(bus);
                }
            }
    };
}