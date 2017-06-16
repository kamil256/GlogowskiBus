﻿//function BusLines()
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

function Route(busLine, details, indexMark, departureTimesModel, pointsModel, busStops, editable)
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

    self.points = new Collection();//Points();
    for (var i = 0; i < pointsModel.length; i++)
        self.points.add(new Point(self, pointsModel[i].Latitude, pointsModel[i].Longitude, pointsModel[i].TimeOffset, busStops));

    self.selectBusStopEvent = null;

    var busStopMarkers = [];
    var pointMarkers = [];
    var polylines = [];

    var visible = false;

    self.select = function()
    {
        visible = true;
        update();
        //for (var i = 0; i < busStopMarkers.length; i++)
        //    if (busStopMarkers[i].getMap() == null)
        //        busStopMarkers[i].setMap(map);

        //if (editable)
        //    for (var i = 0; i < pointMarkers.length; i++)
        //        if (pointMarkers[i].getMap() == null)
        //            pointMarkers[i].setMap(map);

        //if (polyline.getMap() == null)
        //    polyline.setMap(map);
    };

    self.deselect = function()
    {
        visible = false;
        update();
        //for (var i = 0; i < busStopMarkers.length; i++)
        //    if (busStopMarkers[i].getMap() != null)
        //        busStopMarkers[i].setMap(null);

        //for (var i = 0; i < pointMarkers.length; i++)
        //    if (pointMarkers[i].getMap() != null)
        //        pointMarkers[i].setMap(null);

        //if (polyline.getMap() != null)
        //    polyline.setMap(null);
    };

    self.selectBusStop = function(busStop)
    {
        for (var i = 0; i < self.busStops.count() ; i++)
            if (self.busStops.getAt(i) === busStop)
                busStopMarkers[i].setIcon(markerIcons.activeRedBusStop);
            else
                busStopMarkers[i].setIcon(markerIcons.redBusStopOnRoute);
    };

    

    var update = function()
    {
        self.departureTimes.sort(function(departureTime1, departureTime2)
        {
            return departureTime1.getMinutesSinceMidnight() - departureTime2.getMinutesSinceMidnight();
        });

        self.busStops = new Collection();
        for (var i = 0; i < self.points.count(); i++)
            if (self.points.getAt(i).busStop != null)
                self.busStops.add(self.points.getAt(i).busStop);

        for (var i = 0; i < busStopMarkers.length; i++)
            busStopMarkers[i].setMap(null);
        busStopMarkers = [];
        for (var i = 0; i < self.busStops.count() ; i++)
        {
            var busStopMarker = new google.maps.Marker(
            {
                icon: markerIcons.redBusStopOnRoute,
                map: visible ? map : null,//null,
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

        for (var i = 0; i < pointMarkers.length; i++)
            pointMarkers[i].setMap(null);
        pointMarkers = [];
        for (var i = 0; i < self.points.count(); i++)
        {
            var pointMarker = new google.maps.Marker(
            {
                draggable: true,
                //icon: markerIcons.redBusStopOnRoute,
                map: visible && editable ? map : null,//null,
                optimized: false,
                position:
                {
                    lat: self.points.getAt(i).latitude,
                    lng: self.points.getAt(i).longitude
                },
                zIndex: 3
            });

            pointMarker.addListener('click', function(e)
            {
                for (var i = 0; i < pointMarkers.length; i++)
                    if (this == pointMarkers[i])
                    {
                        self.points.remove(self.points.getAt(i));
                        update();
                        //Route.update();
                        break;
                    }
            });

            pointMarker.addListener('dragend', function(e)
            {
                for (var i = 0; i < pointMarkers.length; i++)
                    if (this == pointMarkers[i])
                    {
                        self.points.getAt(i).latitude = pointMarkers[i].position.lat();
                        self.points.getAt(i).longitude = pointMarkers[i].position.lng();
                        self.points.getAt(i).busStop = null;

                        var mousePositionPixels = overlay.getProjection().fromLatLngToContainerPixel(e.latLng);
                        for (var j = 0; j < busStops.count(); j++)
                        {
                            var busStopPositionPixels = overlay.getProjection().fromLatLngToContainerPixel(new google.maps.LatLng(busStops.getAt(j).latitude, busStops.getAt(j).longitude));
                            if (Math.abs(mousePositionPixels.x - busStopPositionPixels.x) <= 9 &&
                                Math.abs(mousePositionPixels.y - busStopPositionPixels.y) <= 9)
                            {
                                self.points.getAt(i).latitude = busStops.getAt(j).latitude;
                                self.points.getAt(i).longitude = busStops.getAt(j).longitude;
                                self.points.getAt(i).busStop = busStops.getAt(j);
                            }
                        }

                        //Route.update();

                        update();
                        break;
                    }
            });

            pointMarkers.push(pointMarker);
        }

        for (var i = 0; i < polylines.length; i++)
            polylines[i].setMap(null);
        polylines = [];
        for (var i = 0; i < self.points.count() - 1; i++)
        {
            var polyline = new google.maps.Polyline(
            {
                map: visible ? map : null,//null,
                path: [new google.maps.LatLng(self.points.getAt(i).latitude, self.points.getAt(i).longitude), new google.maps.LatLng(self.points.getAt(i + 1).latitude, self.points.getAt(i + 1).longitude)],
                strokeColor: '#CC181E',
                strokeOpacity: 1,
                strokeWeight: 4
            });

            polyline.addListener('click', function(e)
            {
                for (var j = 0; j < polylines.length; j++)
                    if (this == polylines[j])
                    {
                        self.points.addAt(j + 1, new Point(self, e.latLng.lat(), e.latLng.lng(), 0, busStops));
                        update();
                        break;
                    }
            });

            polylines.push(polyline);
        }
    };

    update();
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

    // Todo: wrong chechong of busstop!!!
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


