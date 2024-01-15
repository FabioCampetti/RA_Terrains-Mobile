using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TerrainMenuManager : MonoBehaviour {

    public TMP_InputField terrainSizeInputField;

    public TMP_InputField fileNameInputField;
    public TMP_Dropdown dropdown;

    // Start is called before the first frame update
    void OnEnable() {
        Debug.Log("TERRAIN MENU MANAGER STARTED");
        terrainSizeInputField = transform.Find("TerrainSize").GetComponent<TMP_InputField>();
        fileNameInputField = transform.Find("File").GetComponent<TMP_InputField>();
        dropdown = transform.Find("Dropdown").GetComponent<TMP_Dropdown>();
    }

    public void OnGenerateTerrainButtonClick() {


    int terrainSize = int.Parse(terrainSizeInputField.text);
    terrainSizeInputField.text = "0";

    int heightmapResolution = int.Parse(dropdown.options[dropdown.value].text);

    string fileName = fileNameInputField.text;
    fileNameInputField.text = string.Empty;

    GameManager.instance.Terrain();

    TerrainElevationGeneration terrainManager = new TerrainElevationGeneration(TerrainProyectionEventManager.instance.location, terrainSize, heightmapResolution, fileName);

    terrainManager.generateElevations();
    terrainManager.generateTerrain();
    terrainManager.ExportTerrain();

    new TerrainController().LoadAndAddObject(Application.persistentDataPath + $"/TerrainObjects/{terrainManager.fileName}.obj");
    }
}
