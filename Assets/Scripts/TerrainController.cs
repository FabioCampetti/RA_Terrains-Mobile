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
            float terrainHeightDifference = TerrainInfo.instance.lowestElevation - TerrainInfo.instance.highestElevation;
            importedObject.transform.position = new Vector3(940, terrainHeightDifference, TerrainInfo.instance.terrainSize);

            // Establece el objeto hijo como hijo del objeto padre
            importedObject.transform.parent = terrainCanvas.transform;
        } else {
            Debug.LogError("Error loading the .obj file");
        }

        importedObject.AddComponent<ObjectViewer>();
    }

    public void OnDeleteTerrainOnClick() {

        Transform importedTransform = terrainCanvas.transform.GetChild(1);

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
        Destroy(GameObject.Find("Terrain"));
    }
}
