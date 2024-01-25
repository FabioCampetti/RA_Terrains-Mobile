using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SimpleFileBrowser;

public class FileLoader : MonoBehaviour {
    public Button LoadTerrainButton;
    public Button LoadProjectionButton;
    
    void Start() {
        LoadTerrainButton = transform.Find("LoadTerrain")?.GetComponent<Button>();
        if (LoadTerrainButton) {
            LoadTerrainButton.onClick.AddListener(() => LoadFile(false));
        }
        LoadProjectionButton = transform.Find("TerrainProyection")?.GetComponent<Button>();
        if (LoadProjectionButton) {
            LoadProjectionButton.onClick.AddListener(() => LoadFile(true));
        }
    }

    public void LoadFile(bool isProjection) {
        string filePath = "";
        string initialDirectory = Application.persistentDataPath + "/TerrainObjects";

        FileBrowser.SetFilters( true, new FileBrowser.Filter("Objects", ".obj" ));
        FileBrowser.SetDefaultFilter( ".obj" );
        FileBrowser.ShowLoadDialog((string[] paths) => {
            // Check if the user selected a file
            if (paths != null && paths.Length > 0) {
                // Get the selected file path
                filePath = paths[0];

                ExtractTerrainData(filePath);

                if (isProjection) {
                    TerrainProyectionEventManager.instance.InvokeFileLoaded(filePath);
                    GameManager.instance.TerrainPositionMenu();
                } else {
                    gameObject.AddComponent<TerrainController>().LoadAndAddObject(filePath);
                }
            }
        },
        () => GameManager.instance.MainMenu(),
        FileBrowser.PickMode.Files,
        false,
        initialDirectory,
        "Select");
    }

     private  void ExtractTerrainData(string filePath) {
        try {
            // Open the file with a StreamReader
            using (StreamReader reader = new StreamReader(filePath)) {

                int terrainSize = 0;
                float lowestElevation = 0f;
                float highestElevation = 0f;
                Location location = null;
                // Read lines until you find the necessary information or reach the end of the file
                while (!reader.EndOfStream) {
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
                            case "# Location":
                                string[] coordinates = value.Split(',');
                                location = new Location(double.Parse(coordinates[0]), double.Parse(coordinates[1]));
                                break;
                        }
                    }

                    // Exit the loop if you have extracted all necessary information
                    if (terrainSize != 0 && highestElevation !=0 && lowestElevation != 0 && location != null) {
                        TerrainProyectionEventManager.instance.InvokeTerrainSizeReceived(terrainSize);
                        TerrainProyectionEventManager.instance.highestElevation = highestElevation;
                        TerrainProyectionEventManager.instance.InvokeLowestElevationReceived(lowestElevation);
                        TerrainProyectionEventManager.instance.InvokeCoordinatesReceived(location);
                        break;
                    }
                }
                reader.Close();
            }
        } catch (Exception e) {
                Debug.LogError($"Error reading file: {e.Message}");
            }
    }
}
