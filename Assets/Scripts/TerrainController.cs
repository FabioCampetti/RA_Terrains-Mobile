using Dummiesman;
using System;
using System.IO;
using UnityEngine;

public class TerrainController : MonoBehaviour {

    public GameObject terrainCanvas;
    public GameObject importedObject;
    public int terrainSize;
    public float terrainHeightDifference;

    public void LoadAndAddObject(string fileName) {

        DisableTerrain();
        ExtractTerrainData(fileName);

        terrainCanvas = GameObject.Find("TerrainCanvas");
        importedObject = new OBJLoader().Load(fileName,null);

        Texture2D texture = Resources.Load<Texture2D>("TerrainTexture");
        if (texture == null) {
            Debug.Log("Failed to load texture at path: ");
        }
        
        Shader doubleSidedShader = Shader.Find("Standard-DoubleSided");

        // Create a new material using the custom shader
        Material newMaterial = new Material(doubleSidedShader);
        newMaterial.mainTexture = texture;

        // Get or add a MeshRenderer component
        MeshRenderer meshRenderer = importedObject.GetComponentInChildren<MeshRenderer>();
        if (meshRenderer == null) {
            // If MeshRenderer doesn't exist, add one
            meshRenderer = importedObject.AddComponent<MeshRenderer>();
        }

        // Assign the material to the object's renderer
        meshRenderer.material = newMaterial;        

        if (importedObject != null) {
            float terrainHeightDifference = TerrainProyectionEventManager.instance.lowestElevation - TerrainProyectionEventManager.instance.highestElevation;
            importedObject.transform.position = new Vector3(940, terrainHeightDifference, TerrainProyectionEventManager.instance.terrainSize);

            // Establece el objeto hijo como hijo del objeto padre
            importedObject.transform.parent = terrainCanvas.transform;
        } else {
            Debug.LogError("Error loading the .obj file");
        }

        importedObject.AddComponent<ObjectViewer>();
    }

    public void OnDeleteTerrainOnClick() {

        Transform importedTransform = terrainCanvas.transform.GetChild(2);

        if (importedTransform!= null && importedTransform.gameObject != null) {
                //Debug.Log("Deleting object: " + imported.name); // Debug statement
            DestroyImmediate(importedTransform.gameObject);
            Debug.Log("Object deleted");
        }
        else {
            Debug.LogWarning("Imported object is null or already destroyed.");
        }
   }   

    private void DisableTerrain() {
        Terrain terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        terrain.enabled = false;
    }

    private  void ExtractTerrainData(string filePath) {
        try {
            // Open the file with a StreamReader
            using (StreamReader reader = new StreamReader(filePath)) {
                // Initialize variables to store extracted values
                terrainSize = 0;
                float highestElevation = 0f;
                float lowestElevation = 0f;
                int resolution = 0;

                // Read lines until you find the necessary information or reach the end of the file
                while (!reader.EndOfStream) {
                    string line = reader.ReadLine();
                    string[] tokens = line.Split(':');

                    if (tokens.Length >= 2) {
                        string key = tokens[0].Trim();
                        string value = tokens[1].Trim();

                        switch (key) {
                            case "Size":
                                terrainSize = int.Parse(value);
                                break;
                            case "Highest Elevation":
                                highestElevation = float.Parse(value);
                                break;
                            case "Lowest Elevation":
                                lowestElevation = float.Parse(value);
                                break;
                        }
                    }

                    // Exit the loop if you have extracted all necessary information
                    if (terrainSize != 0 && highestElevation != 0 && lowestElevation != 0) {
                        terrainHeightDifference = highestElevation - lowestElevation;
                        break;
                    }
                }
            }
        } catch (Exception e) {
                Debug.LogError($"Error reading file: {e.Message}");
            }
    }
}
