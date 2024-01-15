using Dummiesman;
using UnityEngine;
using System.IO;

public class TerrainProyection : MonoBehaviour {

    public GameObject GPSManager;
    public GameObject terrain;

    public Camera mainCamera;

    public GameObject TerrainProyectionObj;

    public void LoadTerrain() {

    Debug.Log("LoadingTerrain");

    string fileName = TerrainProyectionEventManager.instance.filePath;

    Debug.Log("FILE NAME: " + fileName);

    Debug.Log("LOCATION: " + TerrainProyectionEventManager.instance.location.lat + " , " + TerrainProyectionEventManager.instance.location.lng);

    GPSManager = GameObject.Find("GPSManager");
    TerrainProyectionObj = GameObject.Find("TerrainProyection");
    StreamReader reader = new StreamReader(fileName);
    for (int i = 0; i < 6; i++) {
        Debug.Log(reader.ReadLine());
    }
    terrain = new OBJLoader().Load(fileName,null);
    if (TerrainProyectionObj == null) {
        Debug.Log("Es null");
    }
    Texture2D texture = Resources.Load<Texture2D>("TerrainTexture");
        if (texture == null)
        {
            Debug.Log("Failed to load texture at path: ");
        }
        Material newMaterial = new(Shader.Find("Standard (Specular setup)"))
        {
            mainTexture = texture
        }; // You can choose a different shader if needed

        // Get or add a MeshRenderer component
                if (!terrain.transform.GetChild(0).TryGetComponent<MeshRenderer>(out var meshRenderer))
            {
                // If MeshRenderer doesn't exist, add one
                meshRenderer = terrain.transform.GetChild(0).gameObject.AddComponent<MeshRenderer>();
            }

            // Assign the material to the object's renderer
            meshRenderer.material = newMaterial;
    terrain.transform.parent = TerrainProyectionObj.transform;
    Transform terrainChild = terrain.transform.GetChild(0);
    MeshFilter meshFilter = terrainChild.GetComponent<MeshFilter>();
    Mesh mesh = meshFilter.mesh;
     // Add a MeshCollider component to the GameObject
    MeshCollider meshCollider = terrainChild.gameObject.AddComponent<MeshCollider>();

    // Assign the mesh to the MeshCollider
    meshCollider.sharedMesh = mesh;
    Location location = new Location(TerrainProyectionEventManager.instance.location.lat, TerrainProyectionEventManager.instance.location.lng);
    GPSManager GPSManagerScript = GPSManager.GetComponent<GPSManager>();
    GPSManagerScript.objetos = new GameObject[] { terrain };
    GPSManagerScript.camera = GameObject.Find("NewCamera");

    GPSManagerScript.InitializeGPSManager();
   }
}
