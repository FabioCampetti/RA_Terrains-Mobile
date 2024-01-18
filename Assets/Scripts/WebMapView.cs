using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WebMapView : MonoBehaviour {
    WebViewObject webViewObject;
    private Coroutine webCoroutine;
    public TMP_InputField terrainSizeInputField;

    public Button ADDTerrainButton;

    public Button BackButton;

    public Location selectedLocation;


    void OnEnable() {

        ADDTerrainButton = transform.Find("ADDTerrain")?.GetComponent<Button>();
        terrainSizeInputField = transform.Find("TerrainSize")?.GetComponent<TMP_InputField>();
        if (ADDTerrainButton){
            ADDTerrainButton.onClick.AddListener(() => RetrieveSelectedPosition());
        }
        
        BackButton = transform.Find("Back")?.GetComponent<Button>();
         if (BackButton){
            BackButton.onClick.AddListener(() => DeactivateWebView());
        }
        if (terrainSizeInputField) {
            terrainSizeInputField.onValueChanged.AddListener(UpdateRatius);
        }

        webViewObject = GameObject.Find("WebViewObject").AddComponent<WebViewObject>();
        WebCoroutine();
    }

    // Coroutine example
    void WebCoroutine() {

        webViewObject.Init(
            cb: (msg) =>
            {
                if (msg is string) {
                    if (msg == "Proyect") {
                        RetrieveSelectedPosition();
                    } else {
                        double lat, lng;
                        string[] coordinates = msg.Split(',');
                        if (coordinates.Length == 2 && double.TryParse(coordinates[0], out lat) && double.TryParse(coordinates[1], out lng)) {
                            selectedLocation = new Location(lat,lng);
                        }
                    }
                }
            },
            err: (msg) => {
                Debug.Log($"CallOnError[{msg}]");
            },
            httpErr: (msg) => {
                Debug.Log($"CallOnHttpError[{msg}]");
            },
            started: (msg) => {
                Debug.Log($"CallOnStarted[{msg}]");
            },
            hooked: (msg) => {
                Debug.Log($"CallOnHooked[{msg}]");
            },
            cookies: (msg) => {
                Debug.Log($"CallOnCookies[{msg}]");
            },
            ld: (msg) => {
                // Call the JavaScript function to initialize the map
                int terrainSize = TerrainProyectionEventManager.instance.terrainSize;
                double lat = TerrainProyectionEventManager.instance.location.lat;
                double lng = TerrainProyectionEventManager.instance.location.lng;
                webViewObject.EvaluateJS($"initMap({terrainSize},{lat},{lng});");
            }
        );

        webViewObject.SetVisibility(true);

        // Set the margins and visibility of the WebView
        if(ADDTerrainButton) {
             webViewObject.SetMargins(5, 0, 0, Screen.height / 2);
        } else {
            webViewObject.SetMargins(0,0,0,0);
        }
        TextAsset htmlFile = Resources.Load<TextAsset>("map");
        webViewObject.LoadHTML(htmlFile.text);
    }

    private void UpdateRatius(string input) {
        int ratius = int.Parse(input);
        webViewObject.EvaluateJS($"updateRadius({ratius})");
    }

    // Coroutine to introduce a delay before calling getSelectedPosition()
    void RetrieveSelectedPosition() {
        DeactivateWebView();
        GetSelectedPosition();

    }

    void DeactivateWebView() {
        // Hide the WebView
        webViewObject.SetVisibility(false);

        // Destroy the WebViewObject
        if (webViewObject != null) {
            webViewObject = null;
        }
    }

    // Callback method to be called from JavaScript
    void GetSelectedPosition() {

        // Use lat and lng in Unity as needed
        Debug.Log($"Selected Position - Latitude: {selectedLocation.lat}, Longitude: {selectedLocation.lng}");

        TerrainProyectionEventManager.instance.InvokeCoordinatesReceived(selectedLocation);
        if(ADDTerrainButton) {
            terrainSizeInputField.onValueChanged.RemoveAllListeners();
            gameObject.GetComponent<TerrainMenuManager>().OnGenerateTerrainButtonClick();
        } else {
            GameManager.instance.DesactivateCamera();
            GameManager.instance.TerrainProyectionMenu();
            gameObject.AddComponent<TerrainProyection>().LoadTerrain();
        }
    }
}
