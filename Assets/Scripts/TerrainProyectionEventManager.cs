using UnityEngine;
using System;

public class TerrainProyectionEventManager : MonoBehaviour {
    public event Action<string,Location> OnTerrainProyection;

    public static TerrainProyectionEventManager instance;

    public string filePath;
    public Location location;

    public int terrainSize;

    public float lowestElevation;

    public float highestElevation;

    private void Awake() {
        if (instance != null && instance != this){

            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    public void InvokeFileLoaded(string filePath) {
        this.filePath = filePath;
    }

    public void InvokeCoordinatesReceived(Location location) {
        this.location = location;
    }

    public void InvokeTerrainSizeReceived(int terrainSize) {
        this.terrainSize = terrainSize;
    }

    public void InvokeLowestElevationReceived(float lowestElevation) {
        this.lowestElevation = lowestElevation;
    }
}