function Collection()
{
    var items = [];

    this.count = function()
    {
        return items.length;
    };

    this.add = function(item)
    {
        if (items.indexOf(item) == -1)
            items.push(item);
    };

    this.addAt = function(index, item)
    {
        if (items.indexOf(item) == -1)
        {
            items.splice(index, 0, item);
        }
    };

    this.remove = function(item)
    {
        items.splice(items.indexOf(item), 1);
    };

    this.addMany = function(itemsArray)
    {
        for (var i = 0; i < itemsArray.length; i++)
            this.add(itemsArray[i]);
    };

    this.getAt = function(index)
    {
        return items[index];
    };

    this.getFirst = function()
    {
        if (items.length > 0)
            return items[0];
        return null;
    };

    this.getLast = function()
    {
        if (items.length > 0)
            return items[items.length - 1];
        return null;
    };

    this.getSingle = function(condition)
    {
        if (condition)
            for (var i = 0; i < items.length; i++)
                if (condition(items[i]))
                    return items[i];
        return null;
    };

    this.get = function(condition)
    {
        var result = new Collection();
        for (var i = 0; i < items.length; i++)
            if (condition(items[i]))
                result.add(items[i]);
        return result;
    };

    this.toArray = function()
    {
        return items;
    };

    this.sort = function(compareFunction)
    {
        items.sort(compareFunction);
    };
}

function getPositionBetweenTwoPoints(startPoint, endPoint, currentTimeOffset)
{
    var latitudeDifference = endPoint.latitude - startPoint.latitude;
    var longitudeDifference = endPoint.longitude - startPoint.longitude;
    var timeRatio = (currentTimeOffset - startPoint.timeOffset) / (endPoint.timeOffset - startPoint.timeOffset);
    var newPointLatitude = startPoint.latitude + latitudeDifference * timeRatio;
    var newPointLongitude = startPoint.longitude + longitudeDifference * timeRatio;
    return new google.maps.LatLng(newPointLatitude, newPointLongitude);
}

function sendAjaxRequest(url, method, data, onSuccess)
{
    $.ajax(url,
    {
        type: method,
        data: data,
        statusCode:
        {
            200: function(response)
            {
                onSuccess(response);
            },
            201: function(response)
            {
                onSuccess(response);
            },
            400: function(message)
            {
                alert('Bad request: ' + message);
            },
            404: function()
            {
                alert('Not found!');
            }
        }
    });
}