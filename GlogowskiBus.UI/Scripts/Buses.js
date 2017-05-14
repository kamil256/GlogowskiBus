function BusStop(busStopFromModel)
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
        self.onactivate(self);
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

    self.activate = function()
    {
        marker.setIcon(markerIcons.activeBusStop);
    };

    self.deactivate = function()
    {
        marker.setIcon(markerIcons.inactiveBusStop);
    };
};

function BusStops(busStopsFromModel)
{
    var self = this;

    var busStops = [];
    for (var i = 0; i < busStopsFromModel.length; i++)
    {
        var busStop = new BusStop(busStopsFromModel[i]);

        busStop.onactivate = function(busStop)
        {
            self.activate(busStop);
        };

        busStops.push(busStop);
    }

    self.getByPosition = function(latitude, longitude)
    {
        for (var i = 0; i < busStops.length; i++)
            if (busStops[i].latitude === latitude && busStops[i].longitude === longitude)
                return busStops[i];
        return null;
    };

    self.hideAll = function()
    {
        for (var i = 0; i < busStops.length; i++)
            busStops[i].hide();
    };

    self.showAll = function()
    {
        for (var i = 0; i < busStops.length; i++)
            busStops[i].show();
    };

    self.activate = function(busStop)
    {
        busStop.activate();
    };

    self.deactivateAll = function()
    {
        for (var i = 0; i < busStops.length; i++)
                busStops[i].deactivate();
    };
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
            busStopMarkers.push(new google.maps.Marker(
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
            }));

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
}

function Routes(busLines)
{
    var self = this;

    var routes = [];
    for (var i = 0; i < busLines.length; i++)
        routes = routes.concat(busLines[i].routes);

    self.length = routes.length;
        
    for (var i = 0; i < routes.length; i++)
        self[i] = routes[i];

    self.hideAll = function()
    {
        for (var i = 0; i < routes.length; i++)
            routes[i].hide();
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
        route.show();
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

function Buses(departureTimes)
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

    self.busStops = new BusStops(busStopsFromModel);
    self.busLines = new BusLines(busLinesFromModel, self.busStops);
    self.routes = new Routes(self.busLines);
    self.points = new Points(self.routes);
    self.departureTimes = new DepartureTimes(self.routes);
    self.buses = new Buses(self.departureTimes);

    self.view = ko.observable('Navigation');
}