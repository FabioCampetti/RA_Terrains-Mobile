using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TerrainMenuManager : MonoBehaviour {

    public TMP_InputField terrainSizeInputField;

    public TMP_InputField fileNameInputField;
    public TMP_Dropdown dropdown;

    public Button generateTerrainButton;

    public TerrainElevationGeneration terrainElevationGeneration;

    // Start is called before the first frame update
    void Start() {

        terrainSizeInputField = transform.Find("TerrainSize").GetComponent<TMP_InputField>();
        fileNameInputField = transform.Find("File").GetComponent<TMP_InputField>();
        dropdown = transform.Find("Dropdown").GetComponent<TMP_Dropdown>();
        generateTerrainButton = transform.Find("ADDTerrain").GetComponent<Button>();
        //generateTerrainButton.onClick.AddListener(OnGenerateTerrainButtonClick);
        
    }

    public void OnGenerateTerrainButtonClick() {


    int terrainSize = int.Parse(terrainSizeInputField.text);

    Debug.Log(dropdown.options[dropdown.value].text);
    int heightmapResolution = int.Parse(dropdown.options[dropdown.value].text);

    string fileName = fileNameInputField.text;


    TerrainElevationGeneration terrainManager = new TerrainElevationGeneration(TerrainProyectionEventManager.instance.location, terrainSize, heightmapResolution, fileName);

    terrainManager.generateElevations();
    terrainManager.generateTerrain();
    terrainManager.ExportTerrain();

    new TerrainController().LoadAndAddObject(Application.persistentDataPath + $"/TerrainObjects/{terrainManager.fileName}.obj");
    }
}
