function Context(busStopsFromModel, busLinesFromModel)
{
    var context = this;

    // Todo: move to serverTime
    context.daysOfWeek = ['Dzień roboczy', 'Sobota', 'Niedziela'];
    context.actualDayOfWeek = null;
    switch (serverTime.now().getDay())
    {
        case 0:
            context.actualDayOfWeek = context.daysOfWeek[2];
            break;
        case 1: case 2: case 3: case 4: case 5:
            context.actualDayOfWeek = context.daysOfWeek[0];
            break;
        case 6:
            context.actualDayOfWeek = context.daysOfWeek[1];
    }

    context.busStops = new BusStops(busStopsFromModel);
    context.busLines = new BusLines(busLinesFromModel);
    context.routes = new Routes();
    context.points = new Points();
    context.departureTimes = new DepartureTimes();
    context.buses = new Buses();

    context.view = ko.observable('Navigation');
    
    function ActualSelection()
    {
        var self = this;

        self.busStop = ko.observable();

        self.departureTime = ko.observable();

        //self.daysOfWeek = ['Dzień roboczy', 'Sobota', 'Niedziela'];
        
        self.selectedDayOfWeek = ko.observable(context.actualDayOfWeek);

        self.busStops = ko.computed(function()
        {
            var busStops = [];
            if (self.busStop() && self.departureTime())
            {
                var busStopPoint = context.points.getPointForBusStopOnTheRoute(self.busStop(), self.departureTime().route);
                if (busStopPoint)
                {
                    for (var i = 0; i < self.departureTime().route.points.length; i++)
                        if (self.departureTime().route.points[i].busStop != null)
                        {
                            busStops.push(
                            {
                                name: self.departureTime().route.points[i].busStop.name,
                                timeOffset: (self.departureTime().route.points[i].timeOffset - busStopPoint.timeOffset) / 60000
                            });
                        }
                }
            }
            return busStops;
        });

        self.departureTimes = ko.computed(function()
        {
            var departureTimes = [];
            if (self.busStop() && self.departureTime())
            {
                var busStopPoint = context.points.getPointForBusStopOnTheRoute(self.busStop(), self.departureTime().route);
                if (busStopPoint)
                {
                    for (var i = 0; i < 24; i++)
                        departureTimes[i] = [];
                    var busLine = self.departureTime().route.busLine;
                    for (var i = 0; i < busLine.routes.length; i++)
                        for (var j = 0; j < busLine.routes[i].departureTimes.length; j++)
                        {
                            var departureTime = context.departureTimes.getDepartureTimeForBusStop(busLine.routes[i].departureTimes[j], self.busStop());
                            if (departureTime && departureTime.dayOfWeek == self.selectedDayOfWeek())
                            {
                                departureTimes[departureTime.hours].push(
                                {
                                    minutes: departureTime.minutes,
                                    active: self.departureTime().hours == busLine.routes[i].departureTimes[j].hours &&
                                            self.departureTime().minutes == busLine.routes[i].departureTimes[j].minutes &&
                                            self.departureTime().dayOfWeek == busLine.routes[i].departureTimes[j].dayOfWeek
                                });
                                //    departureTime.minutes);
                                //if (self.selectedDayOfWeek() == context.actualDayOfWeek)
                                //    departureTimes[departureTime.hours][j].active = true;
                            }
                        }
                }
            }
            return departureTimes;
        });

        self.setDepartureTime = function(event)
        {
            var hours = event.target.parentElement.parentElement.children[0].children[0].textContent;
            var minutes = event.target.textContent;
            for (var i = 0; i < self.departureTime().route.busLine.departureTimes.length; i++)
            {
                var departureTime = context.departureTimes.getDepartureTimeForBusStop(self.departureTime().route.busLine.departureTimes[i], self.busStop());
                
                if (departureTime && departureTime.hours == hours && departureTime.minutes == minutes && departureTime.dayOfWeek == self.selectedDayOfWeek())
                {
                    self.departureTime(self.departureTime().route.busLine.departureTimes[i]);
                    break;
                }
            }
        }

        self.busStop.subscribe(function(newBusStop)
        {
            for (var i = 0; i < context.busStops.length; i++)
                if (context.busStops[i] != newBusStop)
                    context.busStops[i].deselect();
                else
                    context.busStops[i].select();

            if (self.departureTime() != null)
                self.departureTime().route.selectBusStop(newBusStop);

            if (newBusStop != null && self.departureTime() != null && !self.departureTime().route.busLine.containsBusStop(newBusStop))
                self.departureTime(null);

            if (self.departureTime() != null)
                self.departureTime(context.departureTimes.getNextDepartureTime(self.departureTime().route.busLine));
        });

        self.departureTime.subscribe(function(newDepartureTime)
        {
            for (var i = 0; i < context.routes.length; i++)
                if (newDepartureTime != null && context.routes[i] == newDepartureTime.route)
                    context.routes[i].show();
                else
                    context.routes[i].hide();

            // Todo: if null then set day of week as today
            if (newDepartureTime != null)
                newDepartureTime.route.selectBusStop(self.busStop());
            else
                self.selectedDayOfWeek(context.actualDayOfWeek);

            if (newDepartureTime != null && self.busStop() != null && !newDepartureTime.route.busLine.containsBusStop(self.busStop()))
                self.busStop(null);

            
        });

        self.selectedDayOfWeek.subscribe(function(selectedDayOfWeek)
        {
            if (self.departureTime() != null)
                self.departureTime(context.departureTimes.getNextDepartureTime(self.departureTime().route.busLine));
        });
    }

    context.actualSelection = new ActualSelection();

    function BusStop(busStopFromModel)
    {
        var self = this;

        self.name = busStopFromModel.Name;
        self.latitude = busStopFromModel.Latitude;
        self.longitude = busStopFromModel.Longitude;

        self.points = [];

        var marker = new google.maps.Marker(
        {
            icon: markerIcons.inactiveBusStop,
            map: map,
            optimized: false,
            position:
            {
                lat: self.latitude,
                lng: self.longitude
            },
            zIndex: 0
        });

        marker.addListener('click', function(e)
        {
            context.actualSelection.busStop(self);
        });

        self.select = function()
        {
            marker.setIcon(markerIcons.activeBusStop);
        };

        self.deselect = function()
        {
            marker.setIcon(markerIcons.inactiveBusStop);
        };
    };

    function BusStops(busStopsFromModel)
    {
        var self = this;

        var busStops = [];
        for (var i = 0; i < busStopsFromModel.length; i++)
            busStops.push(new BusStop(busStopsFromModel[i]));

        self.length = busStops.length;

        for (var i = 0; i < busStops.length; i++)
            self[i] = busStops[i];

        self.getByPosition = function(latitude, longitude)
        {
            for (var i = 0; i < busStops.length; i++)
                if (busStops[i].latitude.toFixed(6) === latitude.toFixed(6) &&
                    busStops[i].longitude.toFixed(6) === longitude.toFixed(6))
                    return busStops[i];
            return null;
        };
    }

    function Point(pointFromModel)
    {
        var self = this;

        self.latitude = pointFromModel.Latitude;
        self.longitude = pointFromModel.Longitude;
        self.timeOffset = pointFromModel.TimeOffset;

        self.busStop = context.busStops.getByPosition(self.latitude, self.longitude);
        self.route = null;
    }

    function Points()
    {
        var self = this;

        var points = [];
        for (var i = 0; i < context.routes.length; i++)
            points = points.concat(context.routes[i].points);

        for (var i = 0; i < points.length; i++)
            if (points[i].busStop != null)
                points[i].busStop.points.push(points[i]);

        self.length = points.length;

        for (var i = 0; i < points.length; i++)
            self[i] = points[i];

        self.getPointForBusStopOnTheRoute = function(busStop, route)
        {
            for (var i = 0; i < route.points.length; i++)
                if (route.points[i].busStop === busStop)
                    return route.points[i];
            return null;
        };
    }

    function DepartureTime(hours, minutes, dayOfWeek)
    {
        var self = this;

        self.hours = hours;
        self.minutes = minutes;
        self.dayOfWeek = dayOfWeek;

        self.route = null;

        self.departureDateForDay = function(year, month, day)
        {
            return new Date(year, month, day, self.hours, self.minutes, 0);
        };

        self.arrivalDateForDay = function(year, month, day)
        {
            return new Date(self.departureDateForDay(year, month, day).getTime() + self.route.points[self.route.points.length - 1].timeOffset);
        };

        self.isOnTour = function(now)
        {
            var currentMinutesSinceMidnight = 60 * serverTime.now().getHours() + serverTime.now().getMinutes();
            var departureMinutesSinceMidnight = 60 * self.hours + self.minutes;
            var arrivalMinutesSinceMidnight = departureMinutesSinceMidnight + Math.floor(self.route.points[self.route.points.length - 1].timeOffset / 60000);

            var previousDay = null;
            switch ((serverTime.now().getDay() - 1) % 7)
            {
                case 0:
                    previousDay = context.daysOfWeek[2];
                    break;
                case 1: case 2: case 3: case 4: case 5:
                    previousDay = context.daysOfWeek[0];
                    break;
                case 6:
                    previousDay = context.daysOfWeek[1];
            }

            if (self.dayOfWeek == context.actualDayOfWeek && currentMinutesSinceMidnight >= departureMinutesSinceMidnight && currentMinutesSinceMidnight < arrivalMinutesSinceMidnight)
                return true;
            else if (self.dayOfWeek == previousDay && currentMinutesSinceMidnight >= departureMinutesSinceMidnight - 24 * 60 && currentMinutesSinceMidnight < arrivalMinutesSinceMidnight - 24 * 60)
                return true;
            else
                return false;

            // Todo: check week days
            //var departureDate = self.departureDateForDay(now.getFullYear(), now.getMonth(), now.getDate());
            //var arrivalDate = self.arrivalDateForDay(now.getFullYear(), now.getMonth(), now.getDate());
            //return (now >= departureDate && now < arrivalDate);
        };
    }

    function DepartureTimes()
    {
        var self = this;

        var departureTimes = [];
        for (var i = 0; i < context.routes.length; i++)
            departureTimes = departureTimes.concat(context.routes[i].departureTimes);
        departureTimes.sort(function(departureTime1, departureTime2)
        {
            var departureTime1MinutesFromMidnight = 60 * departureTime1.hours + departureTime1.minutes;
            var departureTime2MinutesFromMidnight = 60 * departureTime2.hours + departureTime2.minutes;
            return departureTime1MinutesFromMidnight - departureTime2MinutesFromMidnight;
        });

        self.length = departureTimes.length;

        for (var i = 0; i < departureTimes.length; i++)
            self[i] = departureTimes[i];

        self.getDepartureTimeForBusStop = function(departureTime, busStop)
        {
            var result = null;
            var busStopPoint = context.points.getPointForBusStopOnTheRoute(busStop, departureTime.route);
            if (busStopPoint)
            {
                var minutes = departureTime.minutes + busStopPoint.timeOffset / 60000;
                var hours = departureTime.hours + Math.floor(minutes / 60);
                var dayOfWeek = departureTime.dayOfWeek;
                
                if (hours > 23)
                {
                    if ((dayOfWeek == context.daysOfWeek[0] && serverTime.now().getDay() == 5) || dayOfWeek != context.daysOfWeek[0])
                    {
                        dayOfWeek = context.daysOfWeek[(context.daysOfWeek.indexOf(dayOfWeek) + 1) % 3]
                    }
                }
                hours %= 24;
                minutes %= 60;

                result =
                {
                    minutes: minutes,
                    hours: hours,
                    dayOfWeek: dayOfWeek
                };
            }
            return result;
        };

        //self.getNextDepartureTime = function(busLine)
        //{
        //    var nextDepartureTime = null;
        //    var nextDepartureTimeMinutesSinceMidnight;
        //    if (context.actualSelection.busStop())
        //    {
        //        var currentMinutesSinceMidnight = 60 * serverTime.now().getHours() + serverTime.now().getMinutes();
        //        for (var i = 0; i < busLine.routes.length; i++)
        //        {
        //            var busStopPoint = context.points.getPointForBusStopOnTheRoute(context.actualSelection.busStop(), busLine.routes[i]);
        //            if (busStopPoint)
        //            {
        //                var departureTimesForToday = busLine.routes[i].getDepartureTimesForToday();
        //                for (var j = 0; j < departureTimesForToday.length; j++)
        //                {
        //                    var departureTimeMinutesSinceMidnight = 60 * departureTimesForToday[j].hours + departureTimesForToday[j].minutes + Math.floor(busStopPoint.timeOffset / 60000);
        //                    if ((departureTimeMinutesSinceMidnight >= currentMinutesSinceMidnight) && (!nextDepartureTime || (departureTimeMinutesSinceMidnight < nextDepartureTimeMinutesSinceMidnight)))
        //                    {
        //                        nextDepartureTime = departureTimesForToday[j];
        //                        nextDepartureTimeMinutesSinceMidnight = departureTimeMinutesSinceMidnight;
        //                    }
        //                }
        //            }
        //        }

        //        if (!nextDepartureTime)
        //        {
        //            nextDepartureTime = busLine.getDepartureTimesForNextDay()[0];
        //        }
        //    }
        //    return nextDepartureTime;
        //};

        self.getNextDepartureTime = function(busLine)
        {
            var nextDepartureTime = null;

            if (context.actualSelection.busStop())
            {
                var currentMinutesSinceMidnight = 60 * serverTime.now().getHours() + serverTime.now().getMinutes();

                for (var i = 0; i < busLine.routes.length; i++)
                {
                    var busStopPoint = context.points.getPointForBusStopOnTheRoute(context.actualSelection.busStop(), busLine.routes[i]);
                    if (busStopPoint)
                    {
                        var departureTimesForPreviousDay = busLine.routes[i].getDepartureTimesForPreviousDay();
                        for (var j = 0; j < departureTimesForPreviousDay.length; j++)
                        {
                            var departureTimeMinutesSinceMidnight = 60 * departureTimesForPreviousDay[j].hours + departureTimesForPreviousDay[j].minutes + Math.floor(busStopPoint.timeOffset / 60000) - 24 * 60;
                            if (departureTimeMinutesSinceMidnight > currentMinutesSinceMidnight)
                            {
                                nextDepartureTime = departureTimesForPreviousDay[j];
                                break;
                            }
                        }
                    }
                }

                if (!nextDepartureTime)
                {
                    for (var i = 0; i < busLine.routes.length; i++)
                    {
                        var busStopPoint = context.points.getPointForBusStopOnTheRoute(context.actualSelection.busStop(), busLine.routes[i]);
                        if (busStopPoint)
                        {
                            var departureTimesForToday = busLine.routes[i].getDepartureTimesForToday();
                            for (var j = 0; j < departureTimesForToday.length; j++)
                            {
                                var departureTimeMinutesSinceMidnight = 60 * departureTimesForToday[j].hours + departureTimesForToday[j].minutes + Math.floor(busStopPoint.timeOffset / 60000);
                                if (departureTimeMinutesSinceMidnight > currentMinutesSinceMidnight)
                                {
                                    nextDepartureTime = departureTimesForToday[j];
                                    break;
                                }
                            }
                        }
                    }
                }

                if (!nextDepartureTime)
                {
                    for (var i = 0; i < busLine.routes.length; i++)
                    {
                        var busStopPoint = context.points.getPointForBusStopOnTheRoute(context.actualSelection.busStop(), busLine.routes[i]);
                        if (busStopPoint)
                        {
                            var departureTimesForNextDay = busLine.routes[i].getDepartureTimesForNextDay();
                            for (var j = 0; j < departureTimesForNextDay.length; j++)
                            {
                                var departureTimeMinutesSinceMidnight = 60 * departureTimesForNextDay[j].hours + departureTimesForNextDay[j].minutes + Math.floor(busStopPoint.timeOffset / 60000) + 24 * 60;
                                if (departureTimeMinutesSinceMidnight > currentMinutesSinceMidnight)
                                {
                                    nextDepartureTime = departureTimesForNextDay[j];
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return nextDepartureTime;
        };
    }

    function Route(routeFromModel)
    {
        var self = this;

        self.details = routeFromModel.Details;

        self.points = [];
        for (var i = 0; i < routeFromModel.Points.length; i++)
        {
            var point = new Point(routeFromModel.Points[i]);
            point.route = self;
            self.points.push(point);
        }

        self.busStops = [];
        for (var i = 0; i < self.points.length; i++)
            if (self.points[i].busStop != null)
                self.busStops.push(self.points[i].busStop);

        self.departureTimes = [];
        for (var i = 0; i < routeFromModel.DepartureTimes.length; i++)
        {
            if (routeFromModel.DepartureTimes[i].WorkingDay)
            {
                var departureTime = new DepartureTime(routeFromModel.DepartureTimes[i].Hours, routeFromModel.DepartureTimes[i].Minutes, context.daysOfWeek[0]);
                departureTime.route = self;
                self.departureTimes.push(departureTime);
            }
            if (routeFromModel.DepartureTimes[i].Saturday)
            {
                var departureTime = new DepartureTime(routeFromModel.DepartureTimes[i].Hours, routeFromModel.DepartureTimes[i].Minutes, context.daysOfWeek[1]);
                departureTime.route = self;
                self.departureTimes.push(departureTime);
            }
            if (routeFromModel.DepartureTimes[i].Sunday)
            {
                var departureTime = new DepartureTime(routeFromModel.DepartureTimes[i].Hours, routeFromModel.DepartureTimes[i].Minutes, context.daysOfWeek[2]);
                departureTime.route = self;
                self.departureTimes.push(departureTime);
            }
        }
        self.departureTimes.sort(function(departureTime1, departureTime2)
        {
            var departureTime1MinutesFromMidnight = 60 * departureTime1.hours + departureTime1.minutes;
            var departureTime2MinutesFromMidnight = 60 * departureTime2.hours + departureTime2.minutes;
            return departureTime1MinutesFromMidnight - departureTime2MinutesFromMidnight;
        });

        self.busLine = null;

        var busStopMarkers = [];
        for (var i = 0; i < self.busStops.length; i++)
        {
            var busStopMarker = new google.maps.Marker(
            {
                icon: markerIcons.orangeBusStop,
                map: null,
                optimized: false,
                position:
                {
                    lat: self.busStops[i].latitude,
                    lng: self.busStops[i].longitude
                },
                zIndex: 2
            });

            busStopMarker.addListener('click', function(e)
            {
                var newActiveBusStop = self.busStops[busStopMarkers.indexOf(this)];
                context.actualSelection.busStop(newActiveBusStop);
            });

            busStopMarkers.push(busStopMarker);
        }

        var path = [];
        for (var i = 0; i < self.points.length; i++)
            path.push(new google.maps.LatLng(self.points[i].latitude, self.points[i].longitude));

        var polyline = new google.maps.Polyline(
        {
            map: null,
            path: path,
            strokeColor: '#FF7F00',
            strokeOpacity: 1,
            strokeWeight: 3,
        });

        self.hide = function()
        {
            for (var i = 0; i < busStopMarkers.length; i++)
                if (busStopMarkers[i].getMap() != null)
                    busStopMarkers[i].setMap(null);

            if (polyline.getMap() != null)
                polyline.setMap(null);
        };

        self.show = function()
        {

            for (var i = 0; i < busStopMarkers.length; i++)
                if (busStopMarkers[i].getMap() == null)
                    busStopMarkers[i].setMap(map);

            if (polyline.getMap() == null)
                polyline.setMap(map);
        };

        self.selectBusStop = function(busStop)
        {
            for (var i = 0; i < self.busStops.length; i++)
                if (self.busStops[i] === busStop)
                    busStopMarkers[i].setIcon(markerIcons.activeBusStop);
                else
                    busStopMarkers[i].setIcon(markerIcons.orangeBusStop);
        };

        //context.actualSelection.busStop.subscribe(function(newBusStop)
        //{
        //    for (var i = 0; i < self.busStops.length; i++)
        //        if (self.busStops[i] === newBusStop)
        //            busStopMarkers[i].setIcon(markerIcons.activeBusStop);
        //        else
        //            busStopMarkers[i].setIcon(markerIcons.orangeBusStop);
        //});

        //context.busStops.activeBusStop.subscribe(function(newActiveBusStop)
        //{
        //    for (var i = 0; i < self.busStops.length; i++)
        //        if (self.busStops[i] === newActiveBusStop)
        //            busStopMarkers[i].setIcon(markerIcons.activeBusStop);
        //        else
        //            busStopMarkers[i].setIcon(markerIcons.orangeBusStop);
        //});
        self.getDepartureTimesForDayOfWeek = function(dayOfWeek)
        {
            var departureTimes = [];
            for (var i = 0; i < self.departureTimes.length; i++)
            {
                if (self.departureTimes[i].dayOfWeek == dayOfWeek)
                    departureTimes.push(self.departureTimes[i]);
            }
            return departureTimes;
        };

        self.getDepartureTimesForPreviousDay = function()
        {
            var previousDay = null;
            switch ((serverTime.now().getDay() - 1) % 7)
            {
                case 0:
                    previousDay = context.daysOfWeek[2];
                    break;
                case 1: case 2: case 3: case 4: case 5:
                    previousDay = context.daysOfWeek[0];
                    break;
                case 6:
                    previousDay = context.daysOfWeek[1];
            }
            return self.getDepartureTimesForDayOfWeek(previousDay);
        };

        self.getDepartureTimesForToday = function()
        {
            return self.getDepartureTimesForDayOfWeek(context.actualDayOfWeek);
        };

        self.getDepartureTimesForNextDay = function()
        {
            var nextDay = null;
            switch ((serverTime.now().getDay() + 1) % 7)
            {
                case 0:
                    nextDay = context.daysOfWeek[2];
                    break;
                case 1: case 2: case 3: case 4: case 5:
                    nextDay = context.daysOfWeek[0];
                    break;
                case 6:
                    nextDay = context.daysOfWeek[1];
            }
            return self.getDepartureTimesForDayOfWeek(nextDay);
        };
    }

    function Routes()
    {
        var self = this;

        var routes = [];
        for (var i = 0; i < context.busLines.length; i++)
            routes = routes.concat(context.busLines[i].routes);

        self.length = routes.length;

        for (var i = 0; i < routes.length; i++)
            self[i] = routes[i];
    }

    function BusLine(busLineFromModel)
    {
        var self = this;

        self.busNumber = busLineFromModel.BusNumber;

        self.routes = [];
        for (var i = 0; i < busLineFromModel.Routes.length; i++)
        {
            var route = new Route(busLineFromModel.Routes[i]);
            route.busLine = self;
            self.routes.push(route);
        }

        self.departureTimes = [];
        for (var i = 0; i < self.routes.length; i++)
            self.departureTimes = self.departureTimes.concat(self.routes[i].departureTimes);
        self.departureTimes.sort(function(departureTime1, departureTime2)
        {
            var departureTime1MinutesFromMidnight = 60 * departureTime1.hours + departureTime1.minutes;
            var departureTime2MinutesFromMidnight = 60 * departureTime2.hours + departureTime2.minutes;
            return departureTime1MinutesFromMidnight - departureTime2MinutesFromMidnight;
        });

        self.hidden = ko.observable(false);

        self.containsBusStop = function(busStop)
        {
            for (var i = 0; i < busStop.points.length; i++)
                if (busStop.points[i].route.busLine === self)
                    return true;
            return false;
        };

        self.getDepartureTimesForDayOfWeek = function(dayOfWeek)
        {
            var departureTimes = [];
            for (var i = 0; i < self.departureTimes.length; i++)
            {
                if (self.departureTimes[i].dayOfWeek == dayOfWeek)
                    departureTimes.push(self.departureTimes[i]);
            }
            return departureTimes;
        };

        self.getDepartureTimesForPreviousDay = function()
        {
            var previousDay = null;
            switch ((serverTime.now().getDay() - 1) % 7)
            {
                case 0:
                    previousDay = context.daysOfWeek[2];
                    break;
                case 1: case 2: case 3: case 4: case 5:
                    previousDay = context.daysOfWeek[0];
                    break;
                case 6:
                    previousDay = context.daysOfWeek[1];
            }
            return self.getDepartureTimesForDayOfWeek(previousDay);
        };

        self.getDepartureTimesForToday = function()
        {
            return self.getDepartureTimesForDayOfWeek(context.actualDayOfWeek);
        };

        self.getDepartureTimesForNextDay = function()
        {
            var nextDay = null;
            switch ((serverTime.now().getDay() + 1) % 7)
            {
                case 0:
                    nextDay = context.daysOfWeek[2];
                    break;
                case 1: case 2: case 3: case 4: case 5:
                    nextDay = context.daysOfWeek[0];
                    break;
                case 6:
                    nextDay = context.daysOfWeek[1];
            }
            return self.getDepartureTimesForDayOfWeek(nextDay);
        };
    }

    function BusLines(busLinesFromModel)
    {
        var self = this;

        var busLines = [];
        for (var i = 0; i < busLinesFromModel.length; i++)
            busLines.push(new BusLine(busLinesFromModel[i]));

        self.length = busLines.length;

        for (var i = 0; i < busLines.length; i++)
            self[i] = busLines[i];
    }

    function Bus(departureTime)
    {
        var self = this;

        var route = departureTime.route;
        var points = route.points;
        var busLine = route.busLine;

        self.busNumber = busLine.busNumber;
        self.departureTime = departureTime;

        var marker = new google.maps.Marker(
        {
            icon: markerIcons.redBus,
            label:
            {
                color: '#FF0000',
                fontSize: '10px',
                fontWeight: 'bold',
                text: self.busNumber
            },
            map: busLine.hidden() ? null : map,
            position:
            {
                lat: points[0].latitude,
                lng: points[0].longitude
            }
        });

        marker.addListener('click', function(e)
        {
            context.actualSelection.departureTime(self.departureTime);
        });

        self.hide = function()
        {
            if (marker.getMap() != null)
                marker.setMap(null);
        };

        self.show = function()
        {
            if (marker.getMap() == null)
                marker.setMap(map);
        };

        busLine.hidden.subscribe(function(newValue)
        {
            if (newValue)
                self.hide();
            else
                self.show();
        });

        var departureMillisecondsSinceMidnight = 60 * 60 * 1000 * self.departureTime.hours + 60 * 1000 * self.departureTime.minutes;

        var update = function()
        {
            var now = serverTime.now();
            var currentMillisecondsSinceMidnight = 60 * 60 * 1000 * now.getHours() + 60 * 1000 * now.getMinutes() + 1000 * now.getSeconds() + now.getMilliseconds();
            if (currentMillisecondsSinceMidnight < departureMillisecondsSinceMidnight)
                departureMillisecondsSinceMidnight -= 24 * 60 * 60 * 1000;
            var currentTimeOffset = currentMillisecondsSinceMidnight - departureMillisecondsSinceMidnight;

            if (currentTimeOffset > points[points.length - 1].timeOffset)
            {
                self.hide();
                self.onremove(self);
            }
            else
            {
                for (var i = 0; i < points.length - 1; i++)
                    if ((currentTimeOffset >= points[i].timeOffset && currentTimeOffset < points[i + 1].timeOffset))
                    {
                        var newPosition = getPositionBetweenTwoPoints(points[i], points[i + 1], currentTimeOffset);
                        marker.setPosition(newPosition);
                        break;
                    }
                setTimeout(update, 100);
            }
        }

        update();
    }

    function Buses()
    {
        var self = this;

        var buses = [];

        self.add = function(departureTime)
        {
            if (!self.contains(departureTime))
            {
                var bus = new Bus(departureTime);
                bus.onremove = function(busToRemove)
                {
                    buses.splice(buses.indexOf(busToRemove), 1);
                };
                buses.push(bus);
            }
        };

        self.contains = function(departureTime)
        {
            for (var i = 0; i < buses.length; i++)
                if (buses[i].departureTime === departureTime)
                    return true;
            return false;
        };

        var tick = function()
        {
            if (serverTime.now().getSeconds() === 0)
                update();
            setTimeout(tick, 1000);
        }

        var update = function()
        {
            for (var i = 0; i < context.departureTimes.length; i++)
                if (context.departureTimes[i].isOnTour(serverTime.now()))
                    self.add(context.departureTimes[i]);
        };

        update();
        tick();
    }
}