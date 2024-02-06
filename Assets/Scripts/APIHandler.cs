using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public static class APIHandler {

    private const string apiKey = "YOUR_API_KEY";
    
    public static float getElevation(Location location) {
        string url = $"https://maps.googleapis.com/maps/api/elevation/json?locations={location.lat},{location.lng}&key={apiKey}";
        ElevationResult[] results = apiCall(url);
        return (float) results[0].elevation;
    }
    
    public static ElevationResult[][] getElevations(List<Location> vertexLocationsList, int terrainSize) {

        ElevationResult[][] allElevations = new ElevationResult[terrainSize][];
        ElevationResult[] leftElevations;
        ElevationResult[] rightElevations;
        ElevationResult[] middleElevations = null;

        if (terrainSize <= 512) {

           leftElevations = elevationsBetweenCoordinates(vertexLocationsList[0], vertexLocationsList[1], terrainSize);
           rightElevations = elevationsBetweenCoordinates(vertexLocationsList[2], vertexLocationsList[3], terrainSize);

        } else {

            leftElevations = generateElevations(vertexLocationsList[0], vertexLocationsList[4], vertexLocationsList[1]);
            rightElevations = generateElevations(vertexLocationsList[2], vertexLocationsList[8], vertexLocationsList[3]);
            middleElevations = generateElevations(vertexLocationsList[5], vertexLocationsList[6], vertexLocationsList[7]);

        }

        for(int i = 0; i < terrainSize; i++) {
            if (terrainSize <= 512) {
                allElevations[i] = elevationsBetweenCoordinates(leftElevations[i].location, rightElevations[i].location, terrainSize);
            } else {
                allElevations[i] = generateElevations(leftElevations[i].location, middleElevations[i].location, rightElevations[i].location);
            }
        }
        return allElevations;
    }

    private static ElevationResult[] generateElevations(Location left, Location middle, Location right) {
        ElevationResult[] firstHalf = elevationsBetweenCoordinates(left, middle);
        ElevationResult[] secondHalf = elevationsBetweenCoordinates(middle, right);
        return firstHalf.Concat(secondHalf).ToArray();
    }

    private static ElevationResult[] elevationsBetweenCoordinates(Location firstCoordinate, Location secondCoordinate, int cantPoints = 512) {
         string url = $"https://maps.googleapis.com/maps/api/elevation/json?path={firstCoordinate.lat},{firstCoordinate.lng}|{secondCoordinate.lat},{secondCoordinate.lng}&samples={cantPoints}&key={apiKey}";
         return apiCall(url);
    }

    private static ElevationResult[] apiCall(string url) {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url)) {
            webRequest.SendWebRequest();

            while (!webRequest.isDone) {
                // Espera hasta que la solicitud esté completa
            }

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError) {
                Debug.LogError("Error en la solicitud a la API de elevación: " + webRequest.error);
            }

            string jsonResponse = webRequest.downloadHandler.text;
            ElevationResponse elevationResponse =  JsonUtility.FromJson<ElevationResponse>(jsonResponse);

            return elevationResponse.results;
        }
    }
}

[System.Serializable]
public class ElevationResponse {
    public ElevationResult[] results;
    public string status;
}
