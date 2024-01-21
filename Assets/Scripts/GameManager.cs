using UnityEngine;
using System;

public class GameManager : MonoBehaviour {

    public event Action OnMainMenu;
    public event Action OnTerrainVisualizationMenu;

    public event Action OnTerrainProyectionMenu;
    public event Action OnTerrainMenu;
    public event Action OnTerrain;

    public event Action OnTerrainPositionMenu;

    public Camera mainCamera;


    public static GameManager instance;

    private void Awake() {
        if (instance != null && instance != this){

            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start() {
        MainMenu();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    public void MainMenu(){
        OnMainMenu?.Invoke();
        Debug.Log("Main Menu Start");
    }

    public void TerrainVisualizationMenu(){
        OnTerrainVisualizationMenu?.Invoke();
        Debug.Log("Terrain Visualization Start");
    }

    public void TerrainProyectionMenu(){
        OnTerrainProyectionMenu?.Invoke();
        Debug.Log("Terrain Proyection Start");
    }
    public void TerrainPositionMenu(){
        OnTerrainPositionMenu?.Invoke();
        Debug.Log("Terrain Position Start");
    }

    public void TerrainMenu(){
        OnTerrainMenu?.Invoke();
        Debug.Log("Terrain Menu Start");
    }

    public void Terrain() {
        OnTerrain?.Invoke();
         Debug.Log("Terrain Start");
    }

    public void DesactivateCamera() {
     mainCamera.gameObject.SetActive(false);
    }

    public void ActivateCamera() {
    GameObject newCamera = GameObject.Find("NewCamera");
    mainCamera.gameObject.SetActive(true);
    newCamera.gameObject.SetActive(false);
    }

    public void CloseApp() {
        Application.Quit();
    }
}
