function Collection(elementsParam)
{
    var self = this;

    var elements = [];
    for (var i = 0; i < elementsParam.length; i++)
        elements.push(elementsParam[i]);

    this.count = elements.length;

    this.getElementAt = function(index)
    {
        return elements[index];
    };

    this.getFirst = function()
    {
        if (elements.length > 0)
            return elements[0];
        return null;
    };

    this.getLast = function()
    {
        if (elements.length > 0)
            return elements[elements.length - 1];
        return null;
    };

    self.getSingle = function(condition)
    {
        if (condition)
            for (var i = 0; i < elements.length; i++)
                if (condition(elements[i]))
                    return elements[i];
        return null;
    };

    this.get = function(condition)
    {
        var result = [];
        for (var i = 0; i < elements.length; i++)
            if (condition(elements[i]))
                result.push(elements[i]);
        return result;
    };

    this.getAll = function()
    {
        return elements;
    };
}