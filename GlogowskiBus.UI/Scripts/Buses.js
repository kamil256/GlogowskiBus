function BusStop(busStopFromModel, context)
{
    var self = this;

    self.name = busStopFromModel.Name;
    self.latitude = busStopFromModel.Latitude;
    self.longitude = busStopFromModel.Longitude;

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
        context.routes.hideAll();
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

function BusStops(busStopsFromModel, context)
{
    var self = this;

    var busStops = [];
    for (var i = 0; i < busStopsFromModel.length; i++)
        busStops.push(new BusStop(busStopsFromModel[i], context));

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

    self.activeBusStop.subscribe(function(newValue)
    {
        console.log(newValue);




        for (var i = 0; i < busStops.length; i++)
            if (busStops[i] != newValue)
                busStops[i].deselect();
            else
                busStops[i].select();

        if (newValue != null)
            for (var i = 0; i < newValue.points.length; i++)
            {
                console.log(999);
                var selectedBusLineContainsActiveBusStop = false;
                if (newValue.points[i].route.busLine == context.busLines.selectedBusLine())
                {
                    selectedBusLineContainsActiveBusStop = true;
                    break;
                }
                if (!selectedBusLineContainsActiveBusStop)
                    context.busLines.selectedBusLine(null);
            }
    });
}

function Point(pointFromModel, busStops)
{
    var self = this;

    self.latitude = pointFromModel.Latitude;
    self.longitude = pointFromModel.Longitude;
    self.timeOffset = pointFromModel.TimeOffset;
    self.busStop = busStops.getByPosition(self.latitude, self.longitude);
}

function Points(routes)
{
    var self = this;

    var points = [];
    for (var i = 0; i < routes.length; i++)
        points = points.concat(routes[i].points);

    for (var i = 0; i < points.length; i++)
    {
        if (points[i].busStop != null)
            points[i].busStop.points = [];
    }
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

function DepartureTimes(routes)
{
    var self = this;

    var departureTimes = [];
    for (var i = 0; i < routes.length; i++)
        departureTimes = departureTimes.concat(routes[i].departureTimes);

    self.length = departureTimes.length;
        
    for (var i = 0; i < departureTimes.length; i++)
        self[i] = departureTimes[i];
}

function Route(routeFromModel, busStops)
{
    var self = this;

    self.details = routeFromModel.Details;

    self.points = [];
    for (var i = 0; i < routeFromModel.Points.length; i++)
    {
        var point = new Point(routeFromModel.Points[i], busStops);
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

    var busStopMarkers = [];
    for (var i = 0; i < self.points.length; i++)
        if (self.points[i].busStop != null)
        {
            var busStopMarker = new google.maps.Marker(
            {
                icon: markerIcons.orangeBusStop,
                map: null,
                optimized: false,
                position:
                {
                    lat: self.points[i].busStop.latitude,
                    lng: self.points[i].busStop.longitude
                },
                zIndex: 2
            });
            busStopMarker.addListener('click', function(e)
            {
                busStops.activeBusStop(busStops.getByPosition(e.latLng.lat(), e.latLng.lng()));
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

        //var routeContainsActiveBusStop = false;
        //for (var i = 0; i < self.busStops.length; i++)
        //{
        //    if (self.busStops[i] == busStops.activeBusStop())
        //    {
        //        routeContainsActiveBusStop = true;
        //        break;
        //    }
        //}
        //if (!routeContainsActiveBusStop)
        //{
        //    console.log('sdfsd');
        //    busStops.activeBusStop(null);
        //}
    };

    busStops.activeBusStop.subscribe(function(newValue)
    {
        for (var i = 0; i < self.busStops.length; i++)
        {
            if (self.busStops[i] == newValue)
                busStopMarkers[i].setIcon(markerIcons.activeBusStop);
            else
                busStopMarkers[i].setIcon(markerIcons.orangeBusStop);
        }

        //if (busStops.activeBusStop() != null)
        //{
        //    var routeContainsActiveBusStop = false;
        //    for (var i = 0; i < self.busStops.length; i++)
        //    {
        //        if (self.busStops[i] == busStops.activeBusStop())
        //        {
        //            routeContainsActiveBusStop = true;
        //            break;
        //        }
        //    }
        //    if (!routeContainsActiveBusStop)
        //    {
        //        console.log('yyyy');
        //        context.busLines.selectedBusLine(null);
        //        self.hide();
        //    }
        //}
    });
}

function Routes(busLines)
{
    var self = this;

    var routes = [];
    for (var i = 0; i < busLines.length; i++)
        routes = routes.concat(busLines[i].routes);

    for (var i = 0; i < routes.length; i++)
    {

    }

    self.length = routes.length;
        
    for (var i = 0; i < routes.length; i++)
        self[i] = routes[i];

    self.hideAll = function()
    {
        for (var i = 0; i < routes.length; i++)
            routes[i].hide();
        busLines.selectedBusLine(null);
    };
}

function BusLine(busLineFromModel, busStops)
{
    var self = this;

    self.busNumber = busLineFromModel.BusNumber;

    self.routes = [];
    for (var i = 0; i < busLineFromModel.Routes.length; i++)
    {
        var route = new Route(busLineFromModel.Routes[i], busStops);
        route.busLine = self;
        self.routes.push(route);
    }

    self.hidden = ko.observable(false);
}

function BusLines(busLinesFromModel, busStops)
{
    var self = this;

    var busLines = [];
    for (var i = 0; i < busLinesFromModel.length; i++)
        busLines.push(new BusLine(busLinesFromModel[i], busStops));

    self.length = busLines.length;
        
    for (var i = 0; i < busLines.length; i++)
        self[i] = busLines[i];

    self.selectedBusLine = ko.observable();

    self.selectedBusLine.subscribe(function(newValue)
    {
        for (var i = 0; i < context.routes.length; i++)
            if (context.routes[i].busLine != newValue)
                context.routes[i].hide();
            else
                context.routes[i].show();

        if (newValue != null && context.busStops.activeBusStop() != null)
            for (var i = 0; i < newValue.routes.length; i++)
            {
                console.log(3423);
                var selectedBusLineContainsActiveBusStop = false;
                for (var j = 0; j < newValue.routes[i].points.length; j++)
                {
                    if (newValue.routes[i].points[j].busStop == context.busStops.activeBusStop())
                    {
                        selectedBusLineContainsActiveBusStop = true;
                        break;
                    }
                }
                if (!selectedBusLineContainsActiveBusStop)
                    context.busStops.activeBusStop(null);
            }
    });
}

function Bus(departureTime, busLines)
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
        busLines.selectedBusLine(busLine);
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

function Buses(departureTimes, busLines)
{
    var self = this;

    var buses = [];

    self.add = function(departureTime)
    {
        if (!self.contains(departureTime))
        {
            var bus = new Bus(departureTime, busLines);
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
        for (var i = 0; i < departureTimes.length; i++)
            if (departureTimes[i].isOnTour(serverTime.now()))
                self.add(departureTimes[i]);
    };

    update();
    tick();
}

function Context(busStopsFromModel, busLinesFromModel)
{
    var self = this;

    self.busStops = new BusStops(busStopsFromModel, self);
    self.busLines = new BusLines(busLinesFromModel, self.busStops, self);
    self.routes = new Routes(self.busLines);
    self.points = new Points(self.routes);
    self.departureTimes = new DepartureTimes(self.routes);
    self.buses = new Buses(self.departureTimes, self.busLines);

    self.view = ko.observable('Navigation');
}