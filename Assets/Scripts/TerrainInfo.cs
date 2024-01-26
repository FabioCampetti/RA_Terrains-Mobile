using UnityEngine;
using System;
using System.IO;

public class TerrainInfo : MonoBehaviour {

    public static TerrainInfo instance;

    public string filePath;
    public Location location;

    public int terrainSize;

    public float lowestElevation;

    public float highestElevation;

    public int resolution;

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    public void FillTerrainData(string filePath) {
        this.filePath = filePath;
    try {
        // Open the file with a StreamReader
        using (StreamReader reader = new StreamReader(filePath)) {

            // Read lines until you find the necessary information or reach the end of the file
            bool isTerrainInfo = false;
            while (!reader.EndOfStream && !isTerrainInfo) {
                string line = reader.ReadLine();
                string[] tokens = line.Split(':');
                if (tokens.Length >= 2) {
                    string key = tokens[0].Trim();
                    string value = tokens[1].Trim();


                    switch (key) {
                        case "# Size":
                            terrainSize = int.Parse(value);
                            break;
                        case "# Highest Elevation":
                            highestElevation = float.Parse(value);
                            break;
                        case "# Lowest Elevation":
                            lowestElevation = float.Parse(value);
                            break;
                        case "# Resolution":
                            resolution = int.Parse(value);
                            break;
                        case "# Location":
                            string[] coordinates = value.Split(',');
                            location = new Location(double.Parse(coordinates[0]), double.Parse(coordinates[1]));
                            isTerrainInfo = true;
                            break;
                    }
                }
            }
            reader.Close();
        }
    } catch (Exception e) {
            Debug.LogError($"Error reading file: {e.Message}");
        }
    }
}