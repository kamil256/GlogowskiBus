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

        self.selectedBusStop = ko.observable();

        self.selectedRoute = ko.observable();

        self.selectedBusLine = ko.observable();

        self.selectedDepartureTime = ko.observable();

        self.selectedDayOfWeek = ko.observable(serverTime.daysOfWeek[0]);

        self.selectedBusStops = ko.computed(function()
        {
            var busStops = [];

            if (self.selectedRoute())
            {
                var busStop = self.selectedBusStop() || self.selectedRoute().busStops()[0];
                var busStopPoint = self.selectedRoute().getBusStopPoint(busStop);

                if (busStop && busStopPoint)
                {
                    for (var i = 0; i < self.selectedRoute().points().length; i++)
                        if (self.selectedRoute().points()[i].busStop())
                        {
                            busStops.push(
                            {
                                name: self.selectedRoute().points()[i].busStop().name(),
                                timeOffset: ko.observable((self.selectedRoute().points()[i].timeOffset() - busStopPoint.timeOffset())/* / 60000*/),
                                originalPoint: self.selectedRoute().points()[i]
                            });
                        }
                }
            }

            return busStops;
        });

        self.selectedDepartureTimes = ko.computed(function()
        {
            var departureTimes = [];
            if (self.selectedRoute())
            {
                for (var i = 0; i < 24; i++)
                    departureTimes[i] = [];
                for (var i = 0; i < self.selectedRoute().departureTimes().length; i++)
                {
                    var minutes = self.selectedRoute().departureTimes()[i].minutes();
                    var hours = self.selectedRoute().departureTimes()[i].hours();
                    var dayOfWeek = self.selectedRoute().departureTimes()[i].dayOfWeek();
                    if (dayOfWeek == self.selectedDayOfWeek())
                    {
                        departureTimes[hours].push(
                        {
                            minutes: minutes,
                            originalDepartureTime: self.selectedRoute().departureTimes()[i]
                        });
                    }
                }
            }
            return departureTimes;
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
            self.selectedBusLine(newRoute ? newRoute.busLine : null);
            self.selectedDepartureTime(newRoute ? newRoute.getNextDepartureTime() : null);
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
            self.selectedRoute(newDepartureTime.route);
            self.selectedBusLine(newDepartureTime.route.busLine);
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
            });
        });
    }
}