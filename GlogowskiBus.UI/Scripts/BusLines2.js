

function Route2(busLine, model, engine)
{
    var self = this;

    self.id = model ? model.Id : null;
    self.indexMark = ko.observable(model ? model.IndexMark : '');
    self.details = ko.observable(model ? model.Details : '');

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

    self.isEditable = ko.observable(false);

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

    self.getDepartureTime = function(hours, minutes, dayOfWeek)
    {
        for (var i = 0; i < self.departureTimes().length; i++)
            if (self.departureTimes()[i].hours() == hours && self.departureTimes()[i].minutes() == minutes && self.departureTimes()[i].dayOfWeek() == dayOfWeek)
                return self.departureTimes()[i];
        return null;
    };

    self.points = ko.observableArray([]);
    if (model)
        for (var i = 0; i < model.Points.length; i++)
            self.points.push(new Point2(self, model.Points[i], engine));

    self.busStops = ko.computed(function()
    {
        var result = [];
        for (var i = 0; i < self.points().length; i++)
            if (self.points()[i].busStop() && result.indexOf(self.points()[i].busStop()) === -1)
                result.push(self.points()[i].busStop());
        return result;
    });

    self.getBusStopPoint = function(busStop)
    {
        for (var i = 0; i < self.points().length; i++)
            if (self.points()[i].busStop() === busStop)
                return self.points()[i];
        return null;
    };

    var polylines = [];

    var updatePolylineMap = function(polyline)
    {
        if (engine.selectedRoute() == self)
            polyline.setMap(map);
        else
            polyline.setMap(null);
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

            updatePolylineMap(polyline);

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
    };

    self.updatePolylines();

    var selectedRouteSubscription = engine.selectedRoute.subscribe(function()
    {
        for (var i = 0; i < polylines.length; i++)
            updatePolylineMap(polylines[i]);
    });

    var pointsSubscription = self.points.subscribe(function()
    {
        self.updatePolylines();
    });

    self.dispose = function()
    {
        for (var i = 0; i < polylines.length; i++)
            polylines[i].setMap(null);
        for (var i = 0; i < self.points().length; i++)
            self.points()[i].dispose;
        selectedRouteSubscription.dispose();
        pointsSubscription.dispose();
    };

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
}

function DepartureTime2(route, model, dayOfWeek, engine)
{
    var self = this;

    
    self.id = model ? model.Id: null;
    self.hours = ko.observable(model ? model.Hours : 0);
    self.minutes = ko.observable(model ? model.Minutes : 0);
    self.dayOfWeek = ko.observable(dayOfWeek || serverTime.daysOfWeek[0]);

    self.getModel = function()
    {
        var departureTimeModel =
        {
            Id: self.id,
            Hours: self.hours(),
            Minutes: self.minutes(),
            WorkingDay: self.dayOfWeek() == serverTime.daysOfWeek[0],
            Saturday: self.dayOfWeek() == serverTime.daysOfWeek[1],
            Sunday: self.dayOfWeek() == serverTime.daysOfWeek[2]
        }
        return departureTimeModel;
    };

    self.busLine = route.busLine;
    self.route = route;

    self.minutesSinceMidnight = ko.computed(function()
    {
        return 60 * self.hours() + self.minutes();
    });

    self.isOnTour = function()
    {
        var currentMinutesSinceMidnight = 60 * serverTime.now().getHours() + serverTime.now().getMinutes();
        console.log(currentMinutesSinceMidnight);
        var departureMinutesSinceMidnight = self.minutesSinceMidnight();
        var arrivalMinutesSinceMidnight = departureMinutesSinceMidnight + Math.floor(self.route.points()[self.route.points().length - 1].timeOffset() / 60000);

        if (self.dayOfWeek() == serverTime.dayOfWeekToday() && currentMinutesSinceMidnight >= departureMinutesSinceMidnight && currentMinutesSinceMidnight < arrivalMinutesSinceMidnight)
            return true;
        else if (self.dayOfWeek() == serverTime.dayOfWeekYesterday() && currentMinutesSinceMidnight >= departureMinutesSinceMidnight - 24 * 60 && currentMinutesSinceMidnight < arrivalMinutesSinceMidnight - 24 * 60)
            return true;
        else
            return false;
    };
}

function Point2(route, model, engine)
{
    var self = this;
    
    self.id = model ? model.Id : null;

    //self.latitude = ko.observable(model ? model.Latitude : 0);
    //self.longitude = ko.observable(model ? model.Longitude : 0);
    self.pointPosition = ko.observable(new google.maps.LatLng(model ? model.Latitude : 0, model ? model.Longitude : 0));
    self.timeOffset = ko.observable(model ? model.TimeOffset : 0);
    

    self.busStopId = ko.observable(model ? model.BusStopId : null);

    self.getModel = function()
    {
        var pointModel =
        {
            Id: self.id,
            Latitude: self.position().lat(),
            Longitude: self.position().lng(),
            TimeOffset: self.timeOffset(),
            BusStopId: self.busStop() ? self.busStop().id : null
        }
        return pointModel;
    };

    self.busLine = route.busLine;
    self.route = route;

    //self.busStop = ko.observable();
    //if (model.BusStopId)
    //    for (var i = 0; i < engine.busStops().length; i++)
    //        if (engine.busStops()[i].id === model.BusStopId)
    //        {
    //            self.busStop(engine.busStops()[i]);
    //            break;
    //        }

    self.busStop = ko.computed(function() 
    { 
        if (self.busStopId())
            for (var i = 0; i < engine.busStops().length; i++)
                if (engine.busStops()[i].id === self.busStopId())
                    return engine.busStops()[i];
        return null;
    });

    self.timeOffsetInMinutes = ko.observable(self.busStop() ? (self.timeOffset() / 60000).toFixed(0) : null);

    self.timeOffset.subscribe(function(newValue)
    {
    });

    self.timeOffsetInMinutes.subscribe(function(newValue)
    {

        self.timeOffset(newValue * 60000);
    });

    self.position = ko.computed(function()
    {
        if (self.busStop())
            return new self.busStop().position();
        else
            return new self.pointPosition();
    });

    var marker = new google.maps.Marker(
    {
        draggable: true,
        //optimized: false,
        position: self.position()
    });

    //self.busStop().latitude.subscribe(function(newValue)
    //{
    //    console.log(newValue);
    //    if (newValue)
    //        self.position(new google.maps.LatLng(newValue, self.position().lng()));
    //});

    var updateMarkerIcon = function()
    {
        if (self.busStop())
        {
            marker.setIcon(markerIcons.redBusStopOnRoute);
            marker.zIndex = 1;
        }
        else
        {
            marker.setIcon(markerIcons.point);
            marker.zIndex = 2;
        }
    };

    updateMarkerIcon();

    var updateMarkerMap = function()
    {
        if (self.route.isEditable() && self.route === engine.selectedRoute())
            marker.setMap(map);
        else
            marker.setMap(null);
    };

    updateMarkerMap();

    var positionSubscription = self.position.subscribe(function(newValue)
    {
        marker.setPosition(newValue);
        self.route.updatePolylines();
    });

    var selectedRouteSubscription = engine.selectedRoute.subscribe(function(newValue)
    {
        updateMarkerMap();
    });

    var busStopSubscription = self.busStop.subscribe(function(newValue)
    {
        updateMarkerIcon();
        self.timeOffsetInMinutes((self.timeOffset() / 60000).toFixed(0));
    });

    var routeIsEditableSubscription = self.route.isEditable.subscribe(function(newValue)
    {
        updateMarkerMap();
    });

    marker.addListener('click', function()
    {
        if (self.route.points().length > 2)
        {
            self.route.points.remove(self);
            self.dispose();
        }
    });

    marker.addListener('dragend', function(e)
    {
        var mousePositionPixels = overlay.getProjection().fromLatLngToContainerPixel(e.latLng);
        for (var i = 0; i < engine.busStops().length; i++)
            if (self.route.busStops().indexOf(engine.busStops()[i]) === -1)
            {
                var busStopPositionPixels = overlay.getProjection().fromLatLngToContainerPixel(engine.busStops()[i].position());
                if (Math.abs(mousePositionPixels.x - busStopPositionPixels.x) <= 9 && Math.abs(mousePositionPixels.y - busStopPositionPixels.y) <= 9)
                {
                    //self.position(new google.maps.LatLng(engine.busStops()[i].latitude(), engine.busStops()[i].longitude()));
                    //self.latitude(engine.busStops()[i].latitude());
                    //self.longitude(engine.busStops()[i].longitude());
                    self.busStopId(engine.busStops()[i].id);
                    return;
                }
            }
        self.pointPosition(marker.position);
        //self.latitude(marker.position.lat());
        //self.longitude(marker.position.lng());
        self.busStopId(null);
    });

    self.dispose = function()
    {
        marker.setMap(null);
        positionSubscription.dispose();
        selectedRouteSubscription.dispose();
        busStopSubscription.dispose();
        routeIsEditableSubscription.dispose();
    };
}