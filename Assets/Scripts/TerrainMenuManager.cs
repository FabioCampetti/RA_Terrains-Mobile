using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TerrainMenuManager : MonoBehaviour {

    public TMP_InputField terrainSizeInputField;

    public TMP_InputField fileNameInputField;
    public TMP_Dropdown dropdown;

    // Start is called before the first frame update
    void OnEnable() {
        terrainSizeInputField = transform.Find("TerrainSize").GetComponent<TMP_InputField>();
        fileNameInputField = transform.Find("File").GetComponent<TMP_InputField>();
        dropdown = transform.Find("Dropdown").GetComponent<TMP_Dropdown>();
    }

    public void OnGenerateTerrainButtonClick() {


    TerrainInfo.instance.terrainSize = int.Parse(terrainSizeInputField.text);
    terrainSizeInputField.text = "0";

    TerrainInfo.instance.resolution = int.Parse(dropdown.options[dropdown.value].text);

    string fileName = fileNameInputField.text;
    fileNameInputField.text = string.Empty;

    GameManager.instance.Terrain();

    TerrainElevationGeneration.GenerateTerrain(fileName);

    new TerrainController().LoadAndAddObject(Application.persistentDataPath + $"/TerrainObjects/{fileName}.obj");
    }
}
