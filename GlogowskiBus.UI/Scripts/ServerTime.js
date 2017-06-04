function ServerTime(serverTimeMilliseconds)
{
    var self = this;

    var serverTime = Number(serverTimeMilliseconds);

    var timeDifference = serverTime - new Date().getTime();

    self.now = function()
    {
        return new Date(new Date().getTime() + timeDifference);
    };

    self.daysOfWeek = ['Dzień roboczy', 'Sobota', 'Niedziela'];

    self.getDayOfWeek = function(day)
    {
        switch (day)
        {
            case 0:
                return self.daysOfWeek[2];
                break;
            case 1: case 2: case 3: case 4: case 5:
                return self.daysOfWeek[0];
                break;
            case 6:
                return self.daysOfWeek[1];
        }
        return null;
    };

    self.dayOfWeekYesterday = function()
    {
        return self.getDayOfWeek((self.now().getDay() - 1) % 7);
    };

    self.dayOfWeekToday = function()
    {
        return self.getDayOfWeek(self.now().getDay());
    };

    self.dayOfWeekTomorrow = function()
    {
        return self.getDayOfWeek((self.now().getDay() + 1) % 7);
    };

    self.nextDayOfWeek = function(dayOfWeek)
    {
        switch (dayOfWeek)
        {
            case self.daysOfWeek[0]:
                return self.dayOfWeekTomorrow();
            case self.daysOfWeek[1]:
                return self.daysOfWeek[2];
            case self.daysOfWeek[2]:
                return self.daysOfWeek[0];
        }
        return null;
    };
}