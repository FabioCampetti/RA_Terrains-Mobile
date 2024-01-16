using Dummiesman;
using UnityEngine;

public class TerrainController : MonoBehaviour {

    public GameObject terrainObject;
    // Start is called before the first frame update
    public GameObject importedObject;

    public int terrainSize;
    public float terrainHeightDifference;

    public void LoadAndAddObject(string fileName) {

        DisableTerrain();
        ExtractTerrainData();
        
        terrainObject = GameObject.Find("TerrainObject");
        importedObject = new OBJLoader().Load(fileName,null);

        Texture2D texture = Resources.Load<Texture2D>("TerrainTexture");
        if (texture == null) {
            Debug.Log("Failed to load texture at path: ");
        }

        Material newMaterial = new(Shader.Find("Standard (Specular setup)")) {
            mainTexture = texture
        };

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
            catch (Exception e) {
            Debug.LogError($"Error reading file: {e.Message}");
            }
        }
    }
}
