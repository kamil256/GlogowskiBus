function calculateTimeOffsets(route)
{
    if (route.Points[0].BusStopId && route.Points[route.Points.length - 1].BusStopId)
    {

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
                var diff_Y = route.Points[end].Longitude - route.Points[end - 1].Longitude;
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
            400: function(response)
            {
                alert('Bad request: ' + response.responseText);
            },
            404: function()
            {
                alert('Not found!');
            }
        }
    });
}