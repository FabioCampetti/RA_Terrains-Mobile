using Dummiesman;
using UnityEngine;

public class TerrainController : MonoBehaviour {

    public GameObject terrainObject;
    // Start is called before the first frame update
    public GameObject importedObject;

    public int terrainSize;
    public float terrainHeightDifference;

    public void LoadAndAddObject(string fileName) {

        terrainObject = GameObject.Find("TerrainObject");
       
        importedObject = new OBJLoader().Load(fileName,null);
        Texture2D texture = Resources.Load<Texture2D>("TerrainTexture");
        if (texture == null) {
            Debug.Log("Failed to load texture at path: ");
        }
        Material newMaterial = new(Shader.Find("Standard (Specular setup)")) {
            mainTexture = texture
        }; // You can choose a different shader if needed

        // Get or add a MeshRenderer component
        if (!importedObject.transform.GetChild(0).TryGetComponent<MeshRenderer>(out var meshRenderer)) {
        // If MeshRenderer doesn't exist, add one
        meshRenderer = importedObject.transform.GetChild(0).gameObject.AddComponent<MeshRenderer>();
        }

        // Assign the material to the object's renderer
        meshRenderer.material = newMaterial;
        if (importedObject != null) {
            Debug.Log("importado");
            importedObject.transform.position = new Vector3(940, 0, 12000);;

            // Ajusta la posición del objeto padre a la posición fija
            //tObject.transform.position = new Vector3(516, 266.5f, 67);

            // Establece el objeto hijo como hijo del objeto padre
            importedObject.transform.parent = terrainObject.transform;
        } else {
            Debug.LogError("Error loading the .obj file");
        }
    }

    public void OnDeleteTerrainOnClick() {
        if (terrainObject.transform.childCount > 0) {

            Transform importedTransform = terrainObject.transform.GetChild(0);

            if (importedTransform!= null && importedTransform.gameObject != null) {
                //Debug.Log("Deleting object: " + imported.name); // Debug statement
                DestroyImmediate(importedTransform.gameObject);
                Debug.Log("Object deleted");
            }
            else {
                Debug.LogWarning("Imported object is null or already destroyed.");
            }
        }
    }

    private void DisableTerrain() {
        Terrain terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        terrain.enabled = false;
    }
}
