using System.Collections.Generic;
using UnityEngine;

public class CoordianteConverter {

    public static Vector2 ConvertToUnityCoordinates(Location location, List<Location> vertexCoordinatesList, int terrainsize) {

        double latitude = location.lat;
        double longitude = location.lng;

        double lat0 = vertexCoordinatesList[1].lat;
        double lon0 = vertexCoordinatesList[1].lng;
        double lat1 = vertexCoordinatesList[2].lat;
        double lon1 = vertexCoordinatesList[2].lng;

        // Calcular las diferencias entre la latitud/longitud actual y el centro del terreno
        double latDifference = (latitude - lat0)/(lat1 - lat0);
        double lonDifference = (longitude - lon0)/(lon1 - lon0);
        
        // Calcular las coordenadas dentro del terreno en unidades de Unity
        float terrainX = (float)(lonDifference * terrainsize);
        float terrainZ = (float)(latDifference * terrainsize);

        return new Vector2(terrainX, terrainZ);
    }

    public static Vector3 GetPositionHeight(Vector2 position, Terrain terrain) {

        Vector3 initialPosition = new Vector3(position.x, 10000f, position.y);

        Ray ray = new Ray(initialPosition, Vector3.down);

        RaycastHit hit;

        float terrainHeight = 0f;

        // Check if the ray hits the terrain
        if (Physics.Raycast(ray, out hit)) {

            // Check if the hit object is the terrain
            if (hit.collider.gameObject == terrain.gameObject) {

                // Get the terrain height at the hit point
                terrainHeight = hit.point.y;
            }
            else {
                Debug.LogWarning("Raycast did not hit the terrain.");
            }
        }
        else {
            Debug.LogWarning("Raycast did not hit anything.");
        }

        return new Vector3(initialPosition.x, terrainHeight, initialPosition.z);
    }
}
