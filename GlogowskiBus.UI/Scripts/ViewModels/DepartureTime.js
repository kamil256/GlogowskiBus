function DepartureTime(route, model, dayOfWeek, engine)
{
    var self = this;

    self.id = model ? model.Id : null;
    self.hours = ko.observable(model ? model.Hours : 0);
    self.minutes = ko.observable(model ? model.Minutes : 0);
    self.dayOfWeek = ko.observable(dayOfWeek || serverTime.daysOfWeek[0]);
    self.busLine = route.busLine;
    self.route = route;

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

    self.dispose = function()
    {
    };
}