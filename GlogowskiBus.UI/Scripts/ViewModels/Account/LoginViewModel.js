function LoginViewModel(engine)
{
    var self = this;

    self.title = 'LOGOWANIE';

    self.isSelected = ko.observable(false);

    self.isSelected.subscribe(function(newValue)
    {
        if (newValue === true)
        {
            engine.mapClickListener = null;
            engine.busStopClickListener = null;
        }
    });
}