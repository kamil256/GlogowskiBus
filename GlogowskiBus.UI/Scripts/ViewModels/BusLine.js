function BusLine(model, engine)
{
    var self = this;

    self.id = model ? model.Id : null;
    self.busNumber = ko.observable(model ? model.BusNumber : 'Nowa linia');

    self.routes = ko.observableArray([]);
    if (model)
        for (var i = 0; i < model.Routes.length; i++)
            self.routes.push(new Route(self, model.Routes[i], engine));

    self.getModel = function()
    {
        var busLineModel =
        {
            Id: self.id,
            BusNumber: self.busNumber(),
            Routes: []
        };
        for (var i = 0; i < self.routes().length; i++)
            busLineModel.Routes.push(self.routes()[i].getModel());
        return busLineModel;
    };

    self.departureTimes = ko.computed(function()
    {
        var result = [];
        for (var i = 0; i < self.routes().length; i++)
            result = result.concat(self.routes()[i].departureTimes());
        result.sort(function(departureTime1, departureTime2)
        {
            return departureTime1.minutesSinceMidnight() - departureTime2.minutesSinceMidnight();
        });
        return result;
    });

    self.points = ko.computed(function()
    {
        var result = [];
        for (var i = 0; i < self.routes().length; i++)
            result = result.concat(self.routes()[i].points());
        return result;
    });

    self.busStops = ko.computed(function()
    {
        var result = [];
        for (var i = 0; i < self.points().length; i++)
            if (self.points()[i].busStop() && result.indexOf(self.points()[i].busStop()) === -1)
                result.push(self.points()[i].busStop());
        return result;
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
        for (var i = 0; i < self.routes().length; i++)
            self.routes()[i].dispose;
    };
}