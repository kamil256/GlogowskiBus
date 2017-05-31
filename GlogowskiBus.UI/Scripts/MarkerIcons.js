function MarkerIcons()
{
    var self = this;

    self.inactiveBusStop = new google.maps.MarkerImage
    (
        '/Content/Images/inactive_bus_stop.png',
        new google.maps.Size(12, 12),
        new google.maps.Point(0, 0),
        new google.maps.Point(6, 6)
    );

    self.activeBusStop = new google.maps.MarkerImage
    (
        '/Content/Images/active_bus_stop.png',
        new google.maps.Size(12, 12),
        new google.maps.Point(0, 0),
        new google.maps.Point(6, 6)
    );

    self.orangeBusStop = new google.maps.MarkerImage
    (
        '/Content/Images/orange_bus_stop.png',
        new google.maps.Size(12, 12),
        new google.maps.Point(0, 0),
        new google.maps.Point(6, 6)
    );

    self.redBusStopOnRoute = new google.maps.MarkerImage
    (
        '/Content/Images/red_bus_stop_on_route.png',
        new google.maps.Size(18, 18),
        new google.maps.Point(0, 0),
        new google.maps.Point(9, 9)
    );

    self.activeRedBusStop = new google.maps.MarkerImage
    (
        '/Content/Images/active_red_bus_stop.png',
        new google.maps.Size(18, 18),
        new google.maps.Point(0, 0),
        new google.maps.Point(9, 9)
    );

    self.redBusStop = new google.maps.MarkerImage
    (
        '/Content/Images/red_bus_stop.png',
        new google.maps.Size(18, 18),
        new google.maps.Point(0, 0),
        new google.maps.Point(9, 9)
    );

    self.greenBusStop = new google.maps.MarkerImage
    (
        '/Content/Images/green_bus_stop.png',
        new google.maps.Size(12, 12),
        new google.maps.Point(0, 0),
        new google.maps.Point(6, 6)
    );

    self.purpleBusStop = new google.maps.MarkerImage
    (
        '/Content/Images/purple_bus_stop.png',
        new google.maps.Size(12, 12),
        new google.maps.Point(0, 0),
        new google.maps.Point(6, 6)
    );

    self.blueBusStop = new google.maps.MarkerImage
    (
        '/Content/Images/blue_bus_stop.png',
        new google.maps.Size(12, 12),
        new google.maps.Point(0, 0),
        new google.maps.Point(6, 6)
    );

    self.redBus =
    {
        url: '/Content/Images/red_bus.png',
        size: new google.maps.Size(26, 32),
        origin: new google.maps.Point(0, 0),
        anchor: new google.maps.Point(13, 31),
        labelOrigin: new google.maps.Point(13, 13)
    };

    self.redBus2 =
    {
        url: '/Content/Images/red_bus2.png',
        size: new google.maps.Size(28, 34),
        origin: new google.maps.Point(0, 0),
        anchor: new google.maps.Point(14, 33),
        labelOrigin: new google.maps.Point(14, 14)
    };
    //    new google.maps.MarkerImage
    //(
    //    '/Content/Images/red_bus.png',
    //    new google.maps.Size(26, 32),
    //    new google.maps.Point(0, 0),
    //    new google.maps.Point(13, 32)
    //);
};