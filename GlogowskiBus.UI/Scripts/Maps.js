var map;
var overlay;

function initMap()
{
    map = new google.maps.Map(document.getElementById('map'),
    {
        center: { lat: 51.662601, lng: 16.086173 },
        zoom: 14
    });

    overlay = new google.maps.OverlayView();
    overlay.draw = function() { };
    overlay.setMap(map);
}