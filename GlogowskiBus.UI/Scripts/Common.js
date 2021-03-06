﻿function calculateTimeOffsets(route)
{
    if (route.Points[0].BusStopId && route.Points[route.Points.length - 1].BusStopId)
    {
        var LAT_TO_LNG_RATIO_IN_KM = 0.6235;
        var start = 0;
        var end = 0;
        while (end < route.Points.length - 1)
        {
            var totalDistance = 0;
            var distances = [];

            do
            {
                end++;
                var diff_X = route.Points[end].Latitude - route.Points[end - 1].Latitude;
                var diff_Y = LAT_TO_LNG_RATIO_IN_KM * (route.Points[end].Longitude - route.Points[end - 1].Longitude);
                var distance = Math.sqrt(diff_X * diff_X + diff_Y * diff_Y);
                totalDistance += distance;
                distances.push(distance);
            }
            while (!route.Points[end].BusStopId);

            var totalTime = Number(route.Points[end].TimeOffset) - Number(route.Points[start].TimeOffset);

            while (++start != end)
            {
                route.Points[start].TimeOffset = Number(route.Points[start - 1].TimeOffset) + Math.round(totalTime * distances.shift() / totalDistance);
            }
        }
    }
}

function getPositionBetweenTwoPoints(startPoint, endPoint, currentTimeOffset)
{
    var latitudeDifference = endPoint.position().lat() - startPoint.position().lat();
    var longitudeDifference = endPoint.position().lng() - startPoint.position().lng();
    var timeRatio = (currentTimeOffset - startPoint.timeOffset()) / (endPoint.timeOffset() - startPoint.timeOffset());
    var newPointLatitude = startPoint.position().lat() + latitudeDifference * timeRatio;
    var newPointLongitude = startPoint.position().lng() + longitudeDifference * timeRatio;
    return new google.maps.LatLng(newPointLatitude, newPointLongitude);
}

function getDistanceBetweenTwoPoints(point1, point2)
{
    var dx = point2.x - point1.x;
    var dy = point2.y - point1.y;
    return Math.sqrt(dx * dx + dy * dy)
}

var numberOfUnansweredAjaxRequests = 0;

function sendAjaxRequest(url, method, data, onSuccess)
{
    $.ajax(url,
    {
        beforeSend: function()
        {
            if (++numberOfUnansweredAjaxRequests !== 0)
                $('.progressIndicator').show();
        },
        complete: function()
        {
            if (--numberOfUnansweredAjaxRequests === 0)
                $('.progressIndicator').hide();
        },
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
            400: function(response)
            {
                alert('Błąd: ' + JSON.parse(response.responseText).Message);
            },
            404: function()
            {
                alert('Nie znaleziono zasobu!');
            }
        }
    });
}

var asideIsVisible = true;

function toggleAsideVisibility()
{
    if (asideIsVisible)
    {
        asideIsVisible = false;
        $('aside').animate(
        {
            width: '0',
            right: '0'

        }, 500);
    }
    else
    {
        asideIsVisible = true;
        $('aside').animate(
        {
            width: '500px',
            right: '10px'
        }, 500);
    }
}