//function BusLines()
//{
//    var self = this;

//    var busLines = new Collection();

//    self.count = busLines.count;
//    self.add = busLines.add;
//    self.addMany = busLines.addMany;
//    self.getAt = busLines.getAt;
//    self.getFirst = busLines.getFirst;
//    self.getLast = busLines.getLast;
//    self.getSingle = busLines.getSingle;
//    self.get = busLines.get;
//    self.toArray = busLines.toArray;
//    self.sort = busLines.sort;
//}

function BusLine(busNumber, routesModel, busStops)
{
    var self = this;

    self.busNumber = busNumber;

    self.routes = new Collection();//Routes();
    for (var i = 0; i < routesModel.length; i++)
        self.routes.add(new Route(self, routesModel[i].Details, routesModel[i].IndexMark, routesModel[i].DepartureTimes, routesModel[i].Points, busStops));

    self.departureTimes = new Collection();
    for (var i = 0; i < self.routes.count(); i++)
        self.departureTimes.addMany(self.routes.getAt(i).departureTimes.toArray());
    self.departureTimes.sort(function(departureTime1, departureTime2)
    {
        return departureTime1.getMinutesSinceMidnight() - departureTime2.getMinutesSinceMidnight();
    });

    self.busStops = new Collection();
    for (var i = 0; i < self.routes.count(); i++)
        self.busStops.addMany(self.routes.getAt(i).busStops.toArray());

    self.hidden = ko.observable(false);
}

//function Routes()
//{
//    var self = this;

//    var routes = new Collection();

//    self.count = routes.count;
//    self.add = routes.add;
//    self.addMany = routes.addMany;
//    self.getAt = routes.getAt;
//    self.getFirst = routes.getFirst;
//    self.getLast = routes.getLast;
//    self.getSingle = routes.getSingle;
//    self.get = routes.get;
//    self.toArray = routes.toArray;
//    self.sort = routes.sort;
//}

function Route(busLine, details, indexMark, departureTimesModel, pointsModel, busStops)
{
    var self = this;

    self.busLine = busLine;
    self.details = details;
    self.indexMark = indexMark;

    self.departureTimes = new Collection();//DepartureTimes();
    for (var i = 0; i < departureTimesModel.length; i++)
    {
        if (departureTimesModel[i].WorkingDay)
            self.departureTimes.add(new DepartureTime(self, departureTimesModel[i].Hours, departureTimesModel[i].Minutes, serverTime.daysOfWeek[0]));
        if (departureTimesModel[i].Saturday)
            self.departureTimes.add(new DepartureTime(self, departureTimesModel[i].Hours, departureTimesModel[i].Minutes, serverTime.daysOfWeek[1]));
        if (departureTimesModel[i].Sunday)
            self.departureTimes.add(new DepartureTime(self, departureTimesModel[i].Hours, departureTimesModel[i].Minutes, serverTime.daysOfWeek[2]));
    }
    self.departureTimes.sort(function(departureTime1, departureTime2)
    {
        return departureTime1.getMinutesSinceMidnight() - departureTime2.getMinutesSinceMidnight();
    });

    self.points = new Collection();//Points();
    for (var i = 0; i < pointsModel.length; i++)
        self.points.add(new Point(self, pointsModel[i].Latitude, pointsModel[i].Longitude, pointsModel[i].TimeOffset, busStops));

    self.busStops = new BusStops();
    for (var i = 0; i < self.points.count(); i++)
        if (self.points.getAt(i).busStop != null)
            self.busStops.add(self.points.getAt(i).busStop);

    self.selectBusStopEvent = null;

    var busStopMarkers = [];
    for (var i = 0; i < self.busStops.count(); i++)
    {
        var busStopMarker = new google.maps.Marker(
        {
            icon: markerIcons.redBusStopOnRoute,
            map: null,
            optimized: false,
            position:
            {
                lat: self.busStops.getAt(i).latitude,
                lng: self.busStops.getAt(i).longitude
            },
            title: self.busStops.getAt(i).name,
            zIndex: 2
        });

        busStopMarker.addListener('click', function(e)
        {
            if (self.selectBusStopEvent)
                self.selectBusStopEvent(self.busStops.getAt(busStopMarkers.indexOf(this)));
        });

        busStopMarkers.push(busStopMarker);
    }

    var path = [];
    for (var i = 0; i < self.points.count() ; i++)
        path.push(new google.maps.LatLng(self.points.getAt(i).latitude, self.points.getAt(i).longitude));

    var polyline = new google.maps.Polyline(
    {
        map: null,
        path: path,
        strokeColor: '#CC181E',
        strokeOpacity: 1,
        strokeWeight: 4
    });

    self.select = function()
    {
        for (var i = 0; i < busStopMarkers.length; i++)
            if (busStopMarkers[i].getMap() == null)
                busStopMarkers[i].setMap(map);

        if (polyline.getMap() == null)
            polyline.setMap(map);
    };

    self.deselect = function()
    {
        for (var i = 0; i < busStopMarkers.length; i++)
            if (busStopMarkers[i].getMap() != null)
                busStopMarkers[i].setMap(null);

        if (polyline.getMap() != null)
            polyline.setMap(null);
    };

    self.selectBusStop = function(busStop)
    {
        for (var i = 0; i < self.busStops.count() ; i++)
            if (self.busStops.getAt(i) === busStop)
                busStopMarkers[i].setIcon(markerIcons.activeRedBusStop);
            else
                busStopMarkers[i].setIcon(markerIcons.redBusStopOnRoute);
    };
}

//function DepartureTimes()
//{
//    var self = this;

//    var departureTimes = new Collection();

//    self.count = departureTimes.count;
//    self.add = departureTimes.add;
//    self.addMany = departureTimes.addMany;
//    self.getAt = departureTimes.getAt;
//    self.getFirst = departureTimes.getFirst;
//    self.getLast = departureTimes.getLast;
//    self.getSingle = departureTimes.getSingle;
//    self.get = departureTimes.get;
//    self.toArray = departureTimes.toArray;
//    self.sort = departureTimes.sort;

    
//}

function DepartureTime(route, hours, minutes, dayOfWeek)
{
    var self = this;

    self.route = route;
    self.hours = hours;
    self.minutes = minutes;
    self.dayOfWeek = dayOfWeek;

    self.getMinutesSinceMidnight = function()
    {
        return 60 * self.hours + self.minutes;
    };

    self.isOnTour = function()
    {
        var currentMinutesSinceMidnight = 60 * serverTime.now().getHours() + serverTime.now().getMinutes();
        var departureMinutesSinceMidnight = self.getMinutesSinceMidnight();
        var arrivalMinutesSinceMidnight = departureMinutesSinceMidnight + Math.floor(self.route.points.getLast().timeOffset / 60000);

        if (self.dayOfWeek == serverTime.dayOfWeekToday() && currentMinutesSinceMidnight >= departureMinutesSinceMidnight && currentMinutesSinceMidnight < arrivalMinutesSinceMidnight)
            return true;
        else if (self.dayOfWeek == serverTime.dayOfWeekYesterday() && currentMinutesSinceMidnight >= departureMinutesSinceMidnight - 24 * 60 && currentMinutesSinceMidnight < arrivalMinutesSinceMidnight - 24 * 60)
            return true;
        else
            return false;
    };
}


function Point(route, latitude, longitude, timeOffset, busStops)
{
    var self = this;

    self.route = route;
    self.latitude = latitude;
    self.longitude = longitude;
    self.timeOffset = timeOffset;

    self.busStop = busStops.getSingle(function(busStop) { return busStop.latitude.toFixed(6) == self.latitude.toFixed(6) && busStop.longitude.toFixed(6) == self.longitude.toFixed(6); });
    if (self.busStop)
    {
        //if (!self.busStop.points)
        //    self.busStop.points = new Collection();//Points();
        self.busStop.points.add(self);
    }
}

//function Points()
//{
//    var self = this;

//    var points = new Collection();

//    self.count = points.count;
//    self.add = points.add;
//    self.addMany = points.addMany;
//    self.getAt = points.getAt;
//    self.getFirst = points.getFirst;
//    self.getLast = points.getLast;
//    self.getSingle = points.getSingle;
//    self.get = points.get;
//    self.toArray = points.toArray;
//    self.sort = points.sort;
//}


