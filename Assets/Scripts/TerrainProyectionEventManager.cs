using UnityEngine;
using System;

public class TerrainProyectionEventManager : MonoBehaviour {
    public event Action<string,Location> OnTerrainProyection;

    public static TerrainProyectionEventManager instance;

    public string filePath;
    public Location location;

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
}