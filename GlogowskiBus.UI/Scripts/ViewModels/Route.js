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
                self.departureTimes.push(new DepartureTime2(self, model.DepartureTimes[i], serverTime.daysOfWeek[0], engine));
            if (model.DepartureTimes[i].Saturday)
                self.departureTimes.push(new DepartureTime2(self, model.DepartureTimes[i], serverTime.daysOfWeek[1], engine));
            if (model.DepartureTimes[i].Sunday)
                self.departureTimes.push(new DepartureTime2(self, model.DepartureTimes[i], serverTime.daysOfWeek[2], engine));
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
            self.points.push(new Point2(self, model.Points[i], engine));

    self.getModel = function()
    {
        var routeModel =
        {
            Id: self.id,
            IndexMark: self.indexMark() || 'i',
            Details: self.details() || 'd',
            DepartureTimes: [],
            Points: []
        };
        for (var i = 0; i < self.departureTimes().length; i++)
            routeModel.DepartureTimes.push(self.departureTimes()[i].getModel());
        for (var i = 0; i < self.points().length; i++)
            routeModel.Points.push(self.points()[i].getModel());
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

    var polylines = [];

    var updatePolylinesMaps = function()
    {
        for (var i = 0; i < polylines.length; i++)
            if (engine.selectedRoute() == self)
                polylines[i].setMap(map);
            else
                polylines[i].setMap(null);
    };

    self.updatePolylines = function()
    {
        while (polylines.length > 0)
            polylines.pop().setMap(null);

        for (var i = 0; i < self.points().length - 1; i++)
        {
            var polyline = new google.maps.Polyline(
            {
                path: [new google.maps.LatLng(self.points()[i].position().lat(), self.points()[i].position().lng()), new google.maps.LatLng(self.points()[i + 1].position().lat(), self.points()[i + 1].position().lng())],
                strokeColor: '#CC181E',
                strokeOpacity: 1,
                strokeWeight: 4
            });

            polyline.addListener('click', function(e)
            {
                if (self.isEditable())
                    for (var i = 0; i < polylines.length; i++)
                        if (this == polylines[i])
                        {
                            var newPoint = new Point2(self, null, engine);
                            newPoint.pointPosition(new google.maps.LatLng(e.latLng.lat(), e.latLng.lng()));
                            self.points.splice(i + 1, 0, newPoint);
                            break;
                        }
            });

            polylines.push(polyline);
        }

        updatePolylinesMaps();
    };

    self.updatePolylines();

    var engineSelectedRouteSubscription = engine.selectedRoute.subscribe(function()
    {
        updatePolylinesMaps();
    });

    var selfPointsSubscription = self.points.subscribe(function()
    {
        self.updatePolylines();
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
        self.updatePolylines();
    };
}