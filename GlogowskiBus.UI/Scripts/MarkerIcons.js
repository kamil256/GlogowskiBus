function MarkerIcons()
{
    var self = this;

    self.point = new google.maps.MarkerImage
    (
        '/Content/Images/point.png',
        new google.maps.Size(10, 10),
        new google.maps.Point(0, 0),
        new google.maps.Point(5, 5)
    );

    self.grayBusStop = new google.maps.MarkerImage
    (
        '/Content/Images/bus_stop_gray.png',
        new google.maps.Size(18, 18),
        new google.maps.Point(0, 0),

        new google.maps.Point(9, 9)
    );

    self.redBusStop = new google.maps.MarkerImage
    (
        '/Content/Images/bus_stop.png',
        new google.maps.Size(18, 18),
        new google.maps.Point(0, 0),
        new google.maps.Point(9, 9)
    );

    self.activeRedBusStop = new google.maps.MarkerImage
    (
        '/Content/Images/bus_stop_active.png',
        new google.maps.Size(18, 18),
        new google.maps.Point(0, 0),
        new google.maps.Point(9, 9)
    );

    self.redBusStopOnRoute = new google.maps.MarkerImage
    (
        '/Content/Images/bus_stop_on_route.png',
        new google.maps.Size(18, 18),
        new google.maps.Point(0, 0),
        new google.maps.Point(9, 9)
    );

    self.redActiveBusStopOnRoute = new google.maps.MarkerImage
    (
        '/Content/Images/bus_stop_active_on_route.png',
        new google.maps.Size(18, 18),
        new google.maps.Point(0, 0),
        new google.maps.Point(9, 9)
    );

    self.grayBusStopOnRoute = new google.maps.MarkerImage
    (
        '/Content/Images/bus_stop_on_route_gray.png',
        new google.maps.Size(18, 18),
        new google.maps.Point(0, 0),
        new google.maps.Point(9, 9)
    );

    self.redBus =
    {
        url: '/Content/Images/bus.png',
        size: new google.maps.Size(28, 34),
        origin: new google.maps.Point(0, 0),
        anchor: new google.maps.Point(14, 33),
        labelOrigin: new google.maps.Point(14, 14)
    };

    self.activeRedBus =
    {
        url: '/Content/Images/active_red_bus.png',
        size: new google.maps.Size(28, 34),
        origin: new google.maps.Point(0, 0),
        anchor: new google.maps.Point(14, 33),
        labelOrigin: new google.maps.Point(14, 14)
    };

    self.grayBus =
    {
        url: '/Content/Images/grayBus.png',
        size: new google.maps.Size(28, 34),
        origin: new google.maps.Point(0, 0),
        anchor: new google.maps.Point(14, 33),
        labelOrigin: new google.maps.Point(14, 14)
    };
};