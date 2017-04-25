var map;

function initMap()
{
    map = new google.maps.Map(document.getElementById('map'),
    {
        center: { lat: 51.662601, lng: 16.086173 },
        zoom: 14
    });
}

function initLeftMap()
{
    map = new google.maps.Map(document.getElementById('leftMap'),
    {
        center: { lat: 51.662601, lng: 16.086173 },
        zoom: 14
    });
}

function initRightMap()
{
    map = new google.maps.Map(document.getElementById('rightMap'),
    {
        center: { lat: 51.662601, lng: 16.086173 },
        zoom: 14
    });
}

function drawBusStops()
{
    for (var i = 0; i < busStops.length; i++)
        new google.maps.Marker({ position: busStops[i] }).setMap(map);
}