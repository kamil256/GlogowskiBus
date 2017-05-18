function Context(busStopsFromModel, busLinesFromModel)
{
    var context = this;

    context.busStops = new BusStops(busStopsFromModel);
    context.busLines = new BusLines(busLinesFromModel);
    context.routes = new Routes();
    context.points = new Points();
    context.departureTimes = new DepartureTimes();
    context.buses = new Buses();

    context.selectedDayOfWeek = "WorkingDay";
    context.selectedDepartureTime = null;

    context.timeTable = ko.observable();

    context.view = ko.observable('Navigation');

    context.busStops.activeBusStop.subscribe(function(newActiveBusStop)
    {
        if (newActiveBusStop && context.busLines.selectedBusLine())
        {
            // Todo: set current day
            context.selectedDayOfWeek = "WorkingDay";

            // Todo: set closest departure time
            context.selectedDepartureTime = context.departureTimes[10];

            context.timeTable(new TimeTable(context.selectedDepartureTime));
        }
        else
        {
            context.timeTable(null);
        }
    });

    context.setDepartureTime = function(event)
    {
        var hours = event.target.parentElement.parentElement.children[0].children[0].textContent;
        var minutes = event.target.textContent;

        for (var i = 0; i < context.busLines.selectedBusLine().routes.length; i++)
        {
            for (var j = 0; j < context.busLines.selectedBusLine().routes[i].departureTimes.length; j++)
            {
                if (context.busLines.selectedBusLine().routes[i].departureTimes[j].hours == hours &&
                    context.busLines.selectedBusLine().routes[i].departureTimes[j].minutes == minutes)
                {
                    context.selectedDepartureTime = context.busLines.selectedBusLine().routes[i].departureTimes[j];
                    context.timeTable(new TimeTable(context.selectedDepartureTime));
                    break;
                }
            }
        }
    }

    context.busLines.selectedBusLine.subscribe(function(newSelectedBusLine)
    {
        if (context.busStops.activeBusStop() && newSelectedBusLine)
        {
            // Todo: set current day
            context.selectedDayOfWeek = "WorkingDay";

            // Todo: set closest departure time
            context.selectedDepartureTime = context.departureTimes[21];
            context.timeTable(new TimeTable(context.selectedDepartureTime));
        }
        else
        {
            context.timeTable(null);
        }
    });

    function TimeTable(departureTime)
    {
        var self = this;

        self.busStops = ko.observableArray([]);
        var pointForSelectedBusStop = null;
        for (var j = 0; j < departureTime.route.points.length; j++)
        {
            if (departureTime.route.points[j].busStop == context.busStops.activeBusStop())
            {
                pointForSelectedBusStop = departureTime.route.points[j];
                break;
            }
        }
        for (var i = 0; i < departureTime.route.points.length; i++)
            if (departureTime.route.points[i].busStop != null)
            {
                self.busStops.push(
                {
                    name: departureTime.route.points[i].busStop.name,
                    timeOffset: (departureTime.route.points[i].timeOffset - pointForSelectedBusStop.timeOffset) / 60000
                });
            }

        self.hours = [];
        for (var i = 0; i < 24; i++)
            self.hours[i] = [];
        for (var i = 0; i < departureTime.route.departureTimes.length; i++)
        {
            self.hours[departureTime.route.departureTimes[i].hours].push(departureTime.route.departureTimes[i].minutes);
        }
    }

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
            context.busStops.activeBusStop(self);
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

        self.activeBusStop = ko.observable();

        self.activeBusStop.subscribe(function(newActiveBusStop)
        {
            for (var i = 0; i < busStops.length; i++)
                if (busStops[i] != newActiveBusStop)
                    busStops[i].deselect();
                else
                    busStops[i].select();

            if (newActiveBusStop != null && context.busLines.selectedBusLine() != null && !context.busLines.selectedBusLine().containsBusStop(newActiveBusStop))
                context.busLines.selectedBusLine(null);
        });
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
    }

    function DepartureTime(departureTimeFromModel)
    {
        var self = this;

        self.hours = departureTimeFromModel.Hours;
        self.minutes = departureTimeFromModel.Minutes;
        self.workingDays = departureTimeFromModel.WorkingDay;
        self.saturdays = departureTimeFromModel.Saturday;
        self.sundays = departureTimeFromModel.Sunday;

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
            // Todo: check week days
            var departureDate = self.departureDateForDay(now.getFullYear(), now.getMonth(), now.getDate());
            var arrivalDate = self.arrivalDateForDay(now.getFullYear(), now.getMonth(), now.getDate());
            return (now >= departureDate && now < arrivalDate);
        };
    }

    function DepartureTimes()
    {
        var self = this;

        var departureTimes = [];
        for (var i = 0; i < context.routes.length; i++)
            departureTimes = departureTimes.concat(context.routes[i].departureTimes);

        self.length = departureTimes.length;

        for (var i = 0; i < departureTimes.length; i++)
            self[i] = departureTimes[i];

        self.selectedDepartureTime = ko.observable();

        self.selectedDepartureTime.subscribe(function(newSelectedDepartureTime)
        {
            for (var i = 0; i < context.routes.length; i++)
                if (context.routes[i] != newSelectedDepartureTime.route)
                    context.routes[i].hide();
                else
                    context.routes[i].show();

            if (newSelectedBusLine != null && context.busStops.activeBusStop() != null && !newSelectedBusLine.containsBusStop(context.busStops.activeBusStop()))
                context.busStops.activeBusStop(null);
        });
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
            var departureTime = new DepartureTime(routeFromModel.DepartureTimes[i]);
            departureTime.route = self;
            self.departureTimes.push(departureTime);
        }

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
                context.busStops.activeBusStop(newActiveBusStop);
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

        context.busStops.activeBusStop.subscribe(function(newActiveBusStop)
        {
            for (var i = 0; i < self.busStops.length; i++)
                if (self.busStops[i] === newActiveBusStop)
                    busStopMarkers[i].setIcon(markerIcons.activeBusStop);
                else
                    busStopMarkers[i].setIcon(markerIcons.orangeBusStop);
        });
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

        self.hidden = ko.observable(false);

        self.containsBusStop = function(busStop)
        {
            for (var i = 0; i < busStop.points.length; i++)
                if (busStop.points[i].route.busLine === self)
                    return true;
            return false;
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

        self.selectedBusLine = ko.observable();

        self.selectedBusLine.subscribe(function(newSelectedBusLine)
        {
            for (var i = 0; i < context.routes.length; i++)
                if (context.routes[i].busLine != newSelectedBusLine)
                    context.routes[i].hide();
                else
                    context.routes[i].show();

            if (newSelectedBusLine != null && context.busStops.activeBusStop() != null && !newSelectedBusLine.containsBusStop(context.busStops.activeBusStop()))
                context.busStops.activeBusStop(null);
        });
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
            context.busLines.selectedBusLine(busLine);
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

        var now = serverTime.now();
        var departureDate = departureTime.departureDateForDay(now.getFullYear(), now.getMonth(), now.getDate());

        var update = function()
        {
            var now = serverTime.now();
            var currentTimeOffset = now.getTime() - departureDate.getTime();
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