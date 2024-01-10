using UnityEngine;

public class UIManager : MonoBehaviour {

    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject terrainVisualizationCanvas;
    [SerializeField] private GameObject terrainProyectionCanvas;
    [SerializeField] private GameObject terrainPositionCanvas;
    [SerializeField] private GameObject terrainMenuCanvas;
    [SerializeField] private GameObject terrainCanvas;

    // Start is called before the first frame update
    void Start() {

        ActivateMainMenu();
        GameManager.instance.OnMainMenu += ActivateMainMenu;
        GameManager.instance.OnTerrainProyectionMenu += ActivateTerrainProyectionMenu;
        GameManager.instance.OnTerrainPositionMenu += ActivateTerrainPositionMenu;
        GameManager.instance.OnTerrainVisualizationMenu += ActivateTerrainVisualizationMenu;
        GameManager.instance.OnTerrainMenu += ActivateTerrainMenu;
        GameManager.instance.OnTerrain += ActivateTerrain;
    }

    void ActivateMainMenu()
    {
        mainMenuCanvas.SetActive(true);
        terrainProyectionCanvas.SetActive(false);
        terrainPositionCanvas.SetActive(false);
        terrainVisualizationCanvas.SetActive(false);
        terrainMenuCanvas.SetActive(false);
        terrainCanvas.SetActive(false);
    }

    void ActivateTerrainProyectionMenu()
    {
        mainMenuCanvas.SetActive(false);
        terrainProyectionCanvas.SetActive(true);
        terrainPositionCanvas.SetActive(false);
        terrainVisualizationCanvas.SetActive(false);
        terrainMenuCanvas.SetActive(false);
        terrainCanvas.SetActive(false);
    }

    void ActivateTerrainPositionMenu()
    {
        mainMenuCanvas.SetActive(false);
        terrainProyectionCanvas.SetActive(false);
        terrainPositionCanvas.SetActive(true);
        terrainVisualizationCanvas.SetActive(false);
        terrainMenuCanvas.SetActive(false);
        terrainCanvas.SetActive(false);
    }

    void ActivateTerrainVisualizationMenu()
    {
        mainMenuCanvas.SetActive(false);
        terrainProyectionCanvas.SetActive(false);
        terrainPositionCanvas.SetActive(false);
        terrainVisualizationCanvas.SetActive(true);
        terrainMenuCanvas.SetActive(false);
        terrainCanvas.SetActive(false);
    }

    void ActivateTerrainMenu()
    {
        mainMenuCanvas.SetActive(false);
        terrainProyectionCanvas.SetActive(false);
        terrainPositionCanvas.SetActive(false);
        terrainVisualizationCanvas.SetActive(false);
        terrainMenuCanvas.SetActive(true);
        terrainCanvas.SetActive(false);
    }

    void ActivateTerrain()
    {
        mainMenuCanvas.SetActive(false);
        terrainProyectionCanvas.SetActive(false);
        terrainPositionCanvas.SetActive(false);
        terrainVisualizationCanvas.SetActive(false);
        terrainMenuCanvas.SetActive(false);
        terrainCanvas.SetActive(true);
    }
}
