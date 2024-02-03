using Dummiesman;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TerrainController : MonoBehaviour {

    public GameObject terrainCanvas;
    public GameObject importedObject;

    public Button upButton;
    public Button downButton;
    public Button leftButton;
    public Button rightButton;

    public TMP_Text maxHeight;
    public TMP_Text minHeight;

    public TMP_Text terrainSize;

    void OnEnable() {
        int scaleMovement = TerrainInfo.instance.terrainSize / 100;
        upButton.onClick.AddListener(() => MoveTerrain(new Vector3(0,scaleMovement,0)));
        downButton.onClick.AddListener(() => MoveTerrain(new Vector3(0,scaleMovement * -1,0)));
        leftButton.onClick.AddListener(() => MoveTerrain(new Vector3(scaleMovement * -1,0,0)));
        rightButton.onClick.AddListener(() => MoveTerrain(new Vector3(scaleMovement,0,0)));
        importedObject = new OBJLoader().Load(TerrainInfo.instance.filePath,null);
        LoadAndAddObject();   
    }

    public void LoadAndAddObject() {

        DisableTerrain();

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
        maxHeight.text = $"Max. Height {TerrainInfo.instance.highestElevation}m";
        minHeight.text = $"Min. Height {TerrainInfo.instance.lowestElevation}m";
        terrainSize.text = $"Terrain Size {TerrainInfo.instance.terrainSize}m";
    }

    public void OnDeleteTerrainOnClick() {

        Transform importedTransform = importedObject.transform;

        if (importedTransform!= null && importedTransform.gameObject != null) {
                //Debug.Log("Deleting object: " + imported.name); // Debug statement
            DestroyImmediate(importedTransform.gameObject);
            Debug.Log("Object deleted");
        }
        else {
            Debug.LogWarning("Imported object is null or already destroyed.");
        }
        Camera.main.fieldOfView = 60;
   }   

    private void DisableTerrain() {
        Destroy(GameObject.Find("Terrain"));
    }

    private void MoveTerrain(Vector3 direction) {
        Transform importedTransform = terrainCanvas.transform.GetChild(8);
        importedTransform.position += direction;
    }
}
