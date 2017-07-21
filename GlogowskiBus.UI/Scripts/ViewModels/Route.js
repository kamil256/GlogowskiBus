function Route(busLine, model, engine)
{
    var self = this;

    self.isEditable = ko.observable(false);

    self.id = model ? model.Id : null;
    self.indexMark = ko.observable(model ? model.IndexMark : '');
    self.details = ko.observable(model ? model.Details : '');
    self.busLine = busLine;

    self.departureTimes = ko.observableArray([]);
    if (model)
        for (var i = 0; i < model.DepartureTimes.length; i++)
        {
            if (model.DepartureTimes[i].WorkingDay)
                self.departureTimes.push(new DepartureTime(self, model.DepartureTimes[i], serverTime.daysOfWeek[0], engine));
            if (model.DepartureTimes[i].Saturday)
                self.departureTimes.push(new DepartureTime(self, model.DepartureTimes[i], serverTime.daysOfWeek[1], engine));
            if (model.DepartureTimes[i].Sunday)
                self.departureTimes.push(new DepartureTime(self, model.DepartureTimes[i], serverTime.daysOfWeek[2], engine));
        }
    self.sortDepartureTimes = function()
    {
        self.departureTimes.sort(function(departureTime1, departureTime2)
        {
            return departureTime1.minutesSinceMidnight() - departureTime2.minutesSinceMidnight();
        });
    };
    self.sortDepartureTimes();

    self.points = ko.observableArray([]);
    if (model)
        for (var i = 0; i < model.Points.length; i++)
            self.points.push(new Point(self, model.Points[i], engine));
    self.points.sort(function(point1, point2)
    {
        return point1.timeOffset() - point2.timeOffset()
    });

    self.getModel = function()
    {
        var routeModel =
        {
            Id: self.id,
            IndexMark: self.indexMark(),
            Details: self.details(),
            DepartureTimes: [],
            Points: []
        };
        for (var i = 0; i < self.departureTimes().length; i++)
            routeModel.DepartureTimes.push(self.departureTimes()[i].getModel());
        for (var i = 0; i < self.points().length; i++)
            routeModel.Points.push(self.points()[i].getModel());
        calculateTimeOffsets(routeModel);
        return routeModel;
    };

    self.busStops = ko.computed(function()
    {
        var result = [];
        for (var i = 0; i < self.points().length; i++)
            if (self.points()[i].busStop() && result.indexOf(self.points()[i].busStop()) === -1)
                result.push(self.points()[i].busStop());
        return result;
    });

    self.getDepartureTime = function(hours, minutes, dayOfWeek)
    {
        for (var i = 0; i < self.departureTimes().length; i++)
            if (self.departureTimes()[i].hours() == hours && self.departureTimes()[i].minutes() == minutes && self.departureTimes()[i].dayOfWeek() == dayOfWeek)
                return self.departureTimes()[i];
        return null;
    };

    self.getBusStopPoint = function(busStop)
    {
        for (var i = 0; i < self.points().length; i++)
            if (self.points()[i].busStop() === busStop)
                return self.points()[i];
        return null;
    };

    var polyline = new google.maps.Polyline(
    {
        map: map,
        strokeOpacity: 0.8,
        strokeWeight: 6
    });

    polyline.addListener('click', function(e)
    {
        if (self.isEditable())
        {
            var newPoint = overlay.getProjection().fromLatLngToContainerPixel(e.latLng);
            for (var i = 1; i < self.points().length; i++)
            {
                var point1 = overlay.getProjection().fromLatLngToContainerPixel(self.points()[i - 1].position());
                var point2 = overlay.getProjection().fromLatLngToContainerPixel(self.points()[i].position());
                if (Math.abs(getDistanceBetweenTwoPoints(point1, point2) - getDistanceBetweenTwoPoints(point1, newPoint) - getDistanceBetweenTwoPoints(newPoint, point2)) <= 4)
                {
                    var newPoint = new Point(self, null, engine);
                    newPoint.position(e.latLng);
                    self.points.splice(i, 0, newPoint);
                    break;
                }
            }
        }
    });

    var path = ko.computed(function()
    {
        var result = [];
        for (var i = 0; i < self.points().length; i++)
            result.push(self.points()[i].position());
        return result;
    });

    var updatePolylinePath = function()
    {
        polyline.setPath(path());
    }

    updatePolylinePath();

    var updatePolylinesVisibility = function()
    {
        if (engine.selectedBusLine() == self.busLine)
        {
            polyline.setVisible(true);
            if (engine.selectedRoute() === self)
            {
                polyline.setOptions({ strokeColor: '#CC181E' });
                polyline.setOptions({ zIndex: 2 });
            }
            else
            {
                polyline.setOptions({ strokeColor: '#404040' });
                polyline.setOptions({ zIndex: 1 });
            }
        }
        else
        {
            polyline.setVisible(false);
        }
    };

    updatePolylinesVisibility();

    var engineSelectedRouteSubscription = engine.selectedRoute.subscribe(function()
    {
        updatePolylinesVisibility();
    });

    var engineSelectedRouteSubscription = engine.selectedBusLine.subscribe(function()
        {
        updatePolylinesVisibility();
    });

    var pathSubscription = path.subscribe(function()
    {
        updatePolylinePath();
    });

    self.getDepartureTimesForDayOfWeek = function(dayOfWeek)
    {
        var result = [];
        for (var i = 0; i < self.departureTimes().length; i++)
            if (self.departureTimes()[i].dayOfWeek() === dayOfWeek)
                result.push(self.departureTimes()[i]);
        return result;
    };

    self.getNextDepartureTime = function()
    {
        var minutesSinceMidnightNow = 60 * serverTime.now().getHours() + serverTime.now().getMinutes();
        var daysOfWeek =
        [
            {
                dayOfWeek: serverTime.dayOfWeekYesterday(),
                minutesSinceMidnightOffset: -24 * 60
            },
            {
                dayOfWeek: serverTime.dayOfWeekToday(),
                minutesSinceMidnightOffset: 0
            },
            {
                dayOfWeek: serverTime.dayOfWeekTomorrow(),
                minutesSinceMidnightOffset: 24 * 60
            }
        ];

        for (var i = 0; i < daysOfWeek.length; i++)
        {
            var departureTimesForDayOfWeek = self.getDepartureTimesForDayOfWeek(daysOfWeek[i].dayOfWeek);

            for (var j = 0; j < departureTimesForDayOfWeek.length; j++)
            {
                var busStopPoint = departureTimesForDayOfWeek[j].route.getBusStopPoint(engine.selectedBusStop());
                if (busStopPoint)
                {
                    var minutesSinceMidnight = departureTimesForDayOfWeek[j].minutesSinceMidnight() + Math.floor(busStopPoint.timeOffset() / 60000) + daysOfWeek[i].minutesSinceMidnightOffset;
                    if (minutesSinceMidnight > minutesSinceMidnightNow)
                        return departureTimesForDayOfWeek[j];
                }
            }
        }

        return null;
    };

    self.dispose = function()
    {
        engineSelectedRouteSubscription.dispose();
        selfPointsSubscription.dispose();
        for (var i = 0; i < self.departureTimes().length; i++)
            self.departureTimes()[i].dispose();
        self.departureTimes([]);
        for (var i = 0; i < self.points().length; i++)
            self.points()[i].dispose();
        self.points([]);
        self.polyline.setVisible(false);
        self.polyline.setMap(null);
    };
}