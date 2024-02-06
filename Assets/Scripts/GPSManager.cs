using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GPSManager : MonoBehaviour {
    public Location userLocation;
    static readonly double R = 6371.0; // Radius of the earth in km
    static readonly double pi_sobre_180 = Math.PI / 180.0;

    public GameObject camera;
    public GameObject[] objetos;
    public Button BackButton;

    public void InitializeGPSManager() {
        BackButton.onClick.AddListener(() => StopCoroutine(StartGPS()));
        BackButton.onClick.AddListener(() => CancelInvoke("UpdateCamaraPosition"));
        BackButton.onClick.AddListener(() => Destroy(objetos[0]));
        StartCoroutine(StartGPS());
    }
    // Start is called before the first frame update
    IEnumerator StartGPS() {
        
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

    private Vector3 DistanceToOrigin(Location location) {
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

    private void SetPositionTerrain() {

        for(int i = 0; i < objetos.Length; i++) {
            Location loc = objetos[i].GetComponent<LocationComponent>().location;
            Vector3 nuevaPosicion = DistanceToOrigin(objetos[i].GetComponent<LocationComponent>().location);
            nuevaPosicion.y = TerrainInfo.instance.lowestElevation;
            Debug.Log("Terrain Heigth: " + TerrainInfo.instance.lowestElevation);
            objetos[i].transform.position = nuevaPosicion;
        }
    }

    private void UpdateCamaraPosition() {
        userLocation.lat = Input.location.lastData.latitude;
        userLocation.lng = Input.location.lastData.longitude;
       float userElevation = 0f;
       //userElevation = APIHandler.getElevation(userLocation);
        Vector3 newPosition = DistanceToOrigin(userLocation);
        Vector2 newCameraPosition = new Vector2(newPosition.x, newPosition.z);
        if(isInTerrainRange(newCameraPosition, new Vector2(objetos[0].transform.position.x, objetos[0].transform.position.z))){
            camera.transform.position = GetPositionHeight(newCameraPosition);
        } else {
            camera.transform.position = new Vector3(newPosition.x, userElevation + 2f, newPosition.z);
        }
    }

    private bool isInTerrainRange(Vector2 cameraPosition, Vector2 terrainPosition) {
        int halfTerrainSize = TerrainInfo.instance.terrainSize/2;
        return (cameraPosition.x >= terrainPosition.x - halfTerrainSize) &&
               (cameraPosition.x <= terrainPosition.x + halfTerrainSize) &&
               (cameraPosition.y >= terrainPosition.y - halfTerrainSize) &&
               (cameraPosition.y <= terrainPosition.y + halfTerrainSize);
    }

    private double CalculateDistanceKM(Location p1, Location p2) {
        
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

    private Vector3 GetPositionHeight(Vector2 position) {

        Vector3 initialPosition = new Vector3(position.x, 20000f, position.y);

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
