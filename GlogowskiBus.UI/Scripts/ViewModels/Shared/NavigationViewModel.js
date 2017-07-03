function NavigationViewModel()
{
    var self = this;

    function ViewGroup(views)
    {
        var self = this;

        self.views = views;
        self.selectedView = ko.observable(self.views[0]);
    }

    self.viewGroups = ko.observableArray([]);
        
    self.selectedViewGroup = ko.observable();

    var hideView = function(view)
    {
        view.isSelected(false);
    };

    var showView = function(view)
    {
        view.isSelected(true);
    };

    self.selectedViewGroup.subscribe(function(oldValue)
    {
        if (oldValue)
            hideView(oldValue.selectedView());
    }, null, 'beforeChange');

    self.selectedViewGroup.subscribe(function(newValue)
    {
        showView(newValue.selectedView());
    });

    self.selectView = function(viewTitle)
    {
        for (var i = 0; i < self.viewGroups().length; i++)
        {
            for (var j = 0; j < self.viewGroups()[i].views.length; j++)
                if (self.viewGroups()[i].views[j].title === viewTitle)
                {
                    self.viewGroups()[i].selectedView(self.viewGroups()[i].views[j]);
                    self.selectedViewGroup(self.viewGroups()[i]);
                    
                    break;
                }
        }
    };

    self.addViewGroup = function(views)
    {
        var newViewGroup = new ViewGroup(views);

        newViewGroup.selectedView.subscribe(function(oldValue)
        {
            if (oldValue)
                hideView(oldValue);
        }, null, 'beforeChange');

        newViewGroup.selectedView.subscribe(function(newValue)
        {
            showView(newValue);
        });

        self.viewGroups.push(newViewGroup);
    };

    self.isStarted = ko.observable(false);

    self.start = function()
    {
        self.selectedViewGroup(self.viewGroups()[0]);
        self.isStarted(true);
    };
}