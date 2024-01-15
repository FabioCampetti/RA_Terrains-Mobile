using System.Collections.Generic;
using UnityEngine;
using System;

public class TerrainElevationGeneration : MonoBehaviour {

     public int terrainSize;

     public int heightmapResolution;
     private List<Location> vertexCoordinatesList;

     public Location location;
     private float highestElevation;
     private float lowestElevation;
     public string fileName;
     public Terrain terrain;

     private string csvFileName;

    public TerrainElevationGeneration (Location location, int terrainSize, int heightmapResolution, string fileName) {
        this.location = location;
        this.terrainSize = terrainSize;
        this.heightmapResolution = heightmapResolution;
        this.fileName = fileName;
    }

    /*public void Start() {

        generateElevations();
        generateTerrain();
        ExportTerrain();
    }*/

   public void generateElevations() {
            getVertexCoordinates();
            csvFileName = $"{fileName}.csv";
            CSVHandler.WriteCSV(csvFileName, APIHandler.getElevations(vertexCoordinatesList, heightmapResolution-1));
    }

    public void generateTerrain() {

        terrain = GameObject.Find("Terrain").GetComponent<Terrain>();

        TerrainData terrainData = terrain.terrainData;

        TerrainElevationData  terrainElevationData = CSVHandler.ReadCSV(csvFileName, heightmapResolution-1);
        
        terrainData.heightmapResolution = heightmapResolution;
       

        highestElevation = (float) terrainElevationData.highestElevation;
        lowestElevation = (float) terrainElevationData.lowestElevation;
        terrainData.size = new Vector3(terrainSize, highestElevation, terrainSize);

        terrainData.SetHeights(0, 0, calculateHeigths(terrainElevationData.elevationResults));

    }

    public void ExportTerrain() {
        new ExportTerrain(terrain).Export(fileName, lowestElevation, heightmapResolution, location);
        terrain.enabled = false;
    }

    private float[,] calculateHeigths(ElevationResult[,] elevations) {

        float[,] newElevations = new float[heightmapResolution, heightmapResolution];
        for (int i = 0; i < heightmapResolution; i++) {
            for (int j = 0; j < heightmapResolution; j++) {
                    if (i == heightmapResolution - 1 || j == heightmapResolution - 1) {
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

    private float getScaleHeigth(ElevationResult elevationResult) {
        float elevation = (float) elevationResult.elevation;
        float result = (elevation - lowestElevation)/(highestElevation - lowestElevation);
        return result;
    }

    private void getVertexCoordinates() {

        vertexCoordinatesList = new List<Location>();
        
        var distance = Math.Sqrt(Math.Pow(terrainSize/2, 2) + Math.Pow(terrainSize/2, 2));

        vertexCoordinatesList.Add(Location.calculateNewCoordinates(location, 315.0, distance));
        vertexCoordinatesList.Add(Location.calculateNewCoordinates(location, 225.0, distance));
        vertexCoordinatesList.Add(Location.calculateNewCoordinates(location, 45.0, distance));
        vertexCoordinatesList.Add(Location.calculateNewCoordinates(location, 135.0, distance));

        if (heightmapResolution > 513) {
            vertexCoordinatesList.Add(Location.calculateNewCoordinates(location, 270.0, distance));
            vertexCoordinatesList.Add(Location.calculateNewCoordinates(location, 0.0, distance));
            vertexCoordinatesList.Add(location);
            vertexCoordinatesList.Add(Location.calculateNewCoordinates(location, 180.0, distance));
            vertexCoordinatesList.Add(Location.calculateNewCoordinates(location, 90.0, distance));
        }
    }
}
