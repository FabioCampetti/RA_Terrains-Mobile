using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SimpleFileBrowser;

public class FileLoader : MonoBehaviour {
    public Button LoadTerrainButton;
    public Button LoadProjectionButton;
    
    void Start() {
        LoadTerrainButton = transform.Find("LoadTerrain")?.GetComponent<Button>();
        if (LoadTerrainButton) {
            LoadTerrainButton.onClick.AddListener(() => LoadFile(false));
        }
        LoadProjectionButton = transform.Find("TerrainProyection")?.GetComponent<Button>();
        if (LoadProjectionButton) {
            LoadProjectionButton.onClick.AddListener(() => LoadFile(true));
        }
    }

    public void LoadFile(bool isProjection) {
        string filePath = "";
        string initialDirectory = Application.persistentDataPath + "/TerrainObjects";

        FileBrowser.SetFilters( true, new FileBrowser.Filter("Objects", ".obj" ));
        FileBrowser.SetDefaultFilter( ".obj" );
        FileBrowser.ShowLoadDialog((string[] paths) => {
            // Check if the user selected a file
            if (paths != null && paths.Length > 0) {
                // Get the selected file path
                filePath = paths[0];

                TerrainInfo.instance.FillTerrainData(filePath);

                if (isProjection) {
                    GameManager.instance.TerrainPositionMenu();
                } else {
                    gameObject.AddComponent<TerrainController>().LoadAndAddObject(filePath);
                }
            }
        },
        () => GameManager.instance.MainMenu(),
        FileBrowser.PickMode.Files,
        false,
        initialDirectory,
        "Select");
    }
}
