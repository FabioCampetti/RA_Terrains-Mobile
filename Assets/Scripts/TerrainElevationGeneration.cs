using System.Collections.Generic;
using UnityEngine;
using System;

public static class TerrainElevationGeneration {

    public static void GenerateTerrain(string fileName) {
        generateElevations(fileName);
        Terrain terrain = generateTerrain($"{fileName}.csv");
        exportTerrain(terrain, fileName);
    }

   private static void generateElevations(string fileName) {
        getVertexCoordinates();
        string csvFileName = $"{fileName}.csv";
        int resolution = TerrainInfo.instance.resolution < 513 ? TerrainInfo.instance.resolution : TerrainInfo.instance.resolution - 1;
        CSVHandler.WriteCSV(csvFileName, APIHandler.getElevations(getVertexCoordinates(), resolution));
    }

    private static Terrain generateTerrain(string csvFileName) {

        Terrain terrain = new GameObject("Terrain").AddComponent<Terrain>();
        terrain.transform.parent = GameObject.Find("TerrainCanvas").transform;
        TerrainData terrainData = new TerrainData();

        int resolution = TerrainInfo.instance.resolution < 513 ? TerrainInfo.instance.resolution : TerrainInfo.instance.resolution - 1;

        ElevationResult[,]  elevationResults = CSVHandler.ReadCSV(csvFileName, resolution);
        
        terrainData.heightmapResolution = TerrainInfo.instance.resolution;
        float highestElevation = (float) TerrainInfo.instance.highestElevation;
        float lowestElevation = (float) TerrainInfo.instance.lowestElevation;
        terrainData.size = new Vector3(TerrainInfo.instance.terrainSize, highestElevation, TerrainInfo.instance.terrainSize);

        terrainData.SetHeights(0, 0, calculateHeigths(elevationResults));
        
        terrain.terrainData = terrainData;
        return terrain;
    }

    private static void exportTerrain(Terrain terrain, string fileName) {
        ExportTerrain.Export(terrain.terrainData, terrain.transform.position, fileName);
    }

    private static float[,] calculateHeigths(ElevationResult[,] elevations) {

        float lowestElevation = TerrainInfo.instance.lowestElevation;
        float highestElevation = TerrainInfo.instance.highestElevation;
        int heightmapResolution = TerrainInfo.instance.resolution;

        float[,] newElevations = new float[heightmapResolution, heightmapResolution];
        for (int i = 0; i < heightmapResolution; i++) {
            for (int j = 0; j < heightmapResolution; j++) {
                    if (heightmapResolution >= 513 && (i == heightmapResolution - 1 || j == heightmapResolution - 1)) {
                        if (i == heightmapResolution - 1) {
                            newElevations[i, j] = newElevations[i-1, j];
                        } else {
                            newElevations[i, j] = newElevations[i, j-1];
                        }
                    } else {
                        newElevations[i, j] = getScaleHeigth(elevations[i,j]);
                    }
            }
        }
        return newElevations;
    }

    private static float getScaleHeigth(ElevationResult elevationResult) {

        float lowestElevation = TerrainInfo.instance.lowestElevation;
        float highestElevation = TerrainInfo.instance.highestElevation;

        float elevation = (float) elevationResult.elevation;
        float result = (elevation - lowestElevation)/(highestElevation - lowestElevation);
        return result;
    }

    private static List<Location> getVertexCoordinates() {
        
        int terrainSize = TerrainInfo.instance.terrainSize;
        Location location = TerrainInfo.instance.location;

        List<Location> vertexCoordinatesList = new List<Location>();
        
        var distance = Math.Sqrt(Math.Pow(terrainSize/2, 2) + Math.Pow(terrainSize/2, 2));

        vertexCoordinatesList.Add(Location.calculateNewCoordinates(location, 315.0, distance));
        vertexCoordinatesList.Add(Location.calculateNewCoordinates(location, 225.0, distance));
        vertexCoordinatesList.Add(Location.calculateNewCoordinates(location, 45.0, distance));
        vertexCoordinatesList.Add(Location.calculateNewCoordinates(location, 135.0, distance));

        if (TerrainInfo.instance.resolution > 513) {
            vertexCoordinatesList.Add(Location.calculateNewCoordinates(location, 270.0, distance));
            vertexCoordinatesList.Add(Location.calculateNewCoordinates(location, 0.0, distance));
            vertexCoordinatesList.Add(location);
            vertexCoordinatesList.Add(Location.calculateNewCoordinates(location, 180.0, distance));
            vertexCoordinatesList.Add(Location.calculateNewCoordinates(location, 90.0, distance));
        }

        return vertexCoordinatesList;
    }
}
