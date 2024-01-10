using System.Collections;
using UnityEngine;
using System;

public class GPSManager : MonoBehaviour {
    public Location userLocation;
    static readonly double R = 6371.0; // Radius of the earth in km
    static readonly double pi_sobre_180 = Math.PI / 180.0;

    new public GameObject camera;
    public GameObject[] objetos;

    public void InitializeGPSManager() {
        StartCoroutine(StartGPS());
    }
    // Start is called before the first frame update
    IEnumerator StartGPS() {

        Debug.Log("GPSManager Ejecutando");
        
        if (!Input.location.isEnabledByUser)
            yield break;
        // Start service before querying location
        Input.location.Start(1, 0.1f);
        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }
        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            yield break;
        }

        SetPositionTerrain();

        InvokeRepeating("UpdateCamaraPosition", 1, 1);
    }

    public Vector3 DistanceToOrigin(Location location) {
            Vector3 newPos;
            Location origin = new Location(0,0);

            Location projectionY = new Location(location.lat, origin.lng);
            Location projectionX = new Location(origin.lat, location.lng);
            double distX = CalculateDistanceKM(origin, projectionX);
            double distY = CalculateDistanceKM(origin, projectionY);

            if (location.lat < origin.lat)
                distY *= -1.0;

            if (location.lng < origin.lng)
                distX *= -1.0;

            newPos = new Vector3(((float)(distX * 1000.0)), 0, ((float)(distY * 1000.0)));

            return newPos;
    }

    public void SetPositionTerrain() {

        for(int i = 0; i < objetos.Length; i++)
        {
            Vector3 nuevaPosicion = DistanceToOrigin(objetos[i].GetComponent<Location>());
            objetos[i].transform.position = nuevaPosicion;
        }
    }

    public void UpdateCamaraPosition()
    {
        userLocation.lat = Input.location.lastData.latitude;
        userLocation.lng = Input.location.lastData.longitude;

       
            Vector3 newPosition = DistanceToOrigin(userLocation);
            camera.transform.position = GetPositionHeight(new Vector2(newPosition.x, newPosition.z));
        Debug.Log("Camara Position" + camera.transform.position.x + " , " +camera.transform.position.z);
        Debug.Log("Terrain Position" + objetos[0].transform.position.x + " , " +objetos[0].transform.position.z);

    }


    public Vector3 UpdatePosition(Location userLocation, Location poiLocation)
    {
        Vector3 newPos;

        Location projectionY = new Location(poiLocation.lat, userLocation.lng);
        Location projectionX = new Location(userLocation.lat, poiLocation.lng);
        double distX = CalculateDistanceKM(userLocation, projectionX);
        double distY = CalculateDistanceKM(userLocation, projectionY);

        if (poiLocation.lat < userLocation.lat)
            distY *= -1.0;

        if (poiLocation.lng < userLocation.lng)
            distX *= -1.0;

        newPos = new Vector3(((float)(distX * 1000.0)), -7000, ((float)(distY * 1000.0)));

        return newPos;
    }

    public double CalculateDistanceKM(Location p1, Location p2)
    {
        double dLat = (p2.lat - p1.lat) * pi_sobre_180;  // deg2rad below
        double dLon = (p2.lng - p1.lng) * pi_sobre_180;

        double a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(p1.lat * pi_sobre_180) * Math.Cos(p2.lat * pi_sobre_180) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double d = R * c; // Distance in km

        return d;
    }

    public Vector3 GetPositionHeight(Vector2 position) {

        Vector3 initialPosition = new Vector3(position.x, 10000f, position.y);

        Ray ray = new Ray(initialPosition, Vector3.down);

        RaycastHit hit;


        float terrainHeight = 0f;

        // Check if the ray hits the terrain
        if (Physics.Raycast(ray, out hit)) {

            // Check if the hit object is the terrain

                // Get the terrain height at the hit point
                terrainHeight = hit.point.y;
        }
        else {
            Debug.LogWarning("Raycast did not hit anything.");
        }

        return new Vector3(initialPosition.x, terrainHeight + 2f, initialPosition.z);
    }
}
