using System;
using UnityEngine;

[System.Serializable]
public class Location: MonoBehaviour {

    private const double EarthRadius = 6371000; // Earth's radius in meters
    
    public double lat;
    public double lng;

    public Location(double lat, double lng) {
        this.lat = lat;
        this.lng = lng;
    }

    public string toString() {
        return lat + "," + lng;
    }

    public static Location calculateNewCoordinates(Location location, double bearing, double distanceInMeters)
    {
        // Convert initial coordinates from degrees to radians
        double lat1 = location.lat * Math.PI / 180.0;
        double lon1 = location.lng * Math.PI / 180.0;

        // Convert bearing from degrees to radians
        double bearingRad = bearing * Math.PI / 180.0;

        // Calculate the angular distance in radians
        double angularDistance = distanceInMeters / EarthRadius;

        // Calculate new latitude in radians
        double newLat = Math.Asin(Math.Sin(lat1) * Math.Cos(angularDistance) + Math.Cos(lat1) * Math.Sin(angularDistance) * Math.Cos(bearingRad));

        // Calculate new longitude in radians
        double newLon = lon1 + Math.Atan2(Math.Sin(bearingRad) * Math.Sin(angularDistance) * Math.Cos(lat1), Math.Cos(angularDistance) - Math.Sin(lat1) * Math.Sin(newLat));

        // Convert new coordinates back to degrees
        newLat = newLat * 180.0 / Math.PI;
        newLon = newLon * 180.0 / Math.PI;

        return new Location(newLat, newLon);
    }
}
