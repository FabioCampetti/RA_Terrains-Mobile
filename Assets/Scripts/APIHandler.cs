using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public static class APIHandler {

    private const string apiKey = "YOUR_API_KEY";

    public static ElevationResult[][] getElevations(List<Location> vertexLocationsList, int resolution) {

        ElevationResult[][] allElevations = new ElevationResult[resolution][];
        ElevationResult[] leftElevations;
        ElevationResult[] rightElevations;

        if (resolution < 513) {

           leftElevations = elevationsBetweenCoordinates(vertexLocationsList[0], vertexLocationsList[1], resolution);
           rightElevations = elevationsBetweenCoordinates(vertexLocationsList[2], vertexLocationsList[3], resolution);

        } else {

            leftElevations = generateElevations(vertexLocationsList[0], vertexLocationsList[1], resolution);
            rightElevations = generateElevations(vertexLocationsList[2], vertexLocationsList[3], resolution);
        }

        for (int i = 0; i < resolution; i++) {
            if (resolution < 513) {
                allElevations[i] = elevationsBetweenCoordinates(leftElevations[i].location, rightElevations[i].location, resolution);
            } else {
                allElevations[i] = generateElevations(leftElevations[i].location,rightElevations[i].location, resolution);
            }
        }
        return allElevations;
    }

    private static ElevationResult[] generateElevations(Location first, Location last, int resolution) {
        int apiCalls = resolution / 256;
        List<ElevationResult> middlePoints = new List<ElevationResult>(elevationsBetweenCoordinates(first, last, apiCalls + 1));
        List<ElevationResult> elevations = new List<ElevationResult>();
        for (int i = 0; i < apiCalls; i++) {
            ElevationResult firstPoint = middlePoints[0];
            middlePoints.RemoveAt(0);
            if (elevations.Count != 0) { 
                elevations.RemoveAt(elevations.Count - 1);
            } 
            elevations.AddRange(elevationsBetweenCoordinates(firstPoint.location, middlePoints[0].location, 257).ToList());
        }
        return elevations.ToArray();
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
