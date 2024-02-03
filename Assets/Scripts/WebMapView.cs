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

        if (ADDTerrainButton != null) {
            ADDTerrainButton.onClick.AddListener(() => RetrieveSelectedPosition());
        }

        if (terrainSizeInputField != null) {
            terrainSizeInputField.onValueChanged.AddListener(UpdateRatius);
        }

        if (BackButton != null) {
            BackButton.onClick.AddListener(() => DeactivateWebView());
        }
        
        GameObject canvas = GameObject.Find("TerrainMenuCanvas") ?? GameObject.Find("TerrainPositionCanvas");
        webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
        webViewObject.transform.SetParent(canvas.transform);
        webCoroutine =  StartCoroutine(WebCoroutine());
    }

    // Coroutine example
    IEnumerator WebCoroutine() {

        webViewObject.Init(
            cb: (msg) => {
                if (msg is string) {
                    if (msg == "Proyect") {
                        RetrieveSelectedPosition();
                    } else if (msg == "Back") {
                        DeactivateWebView();
                        GameManager.instance.MainMenu();
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
                int terrainSize = ADDTerrainButton ? 0 : TerrainInfo.instance.terrainSize;
                double lat = TerrainInfo.instance.location.lat;
                double lng = TerrainInfo.instance.location.lng;
                int isTerrainProyection = ADDTerrainButton ? 0 : 1;
                webViewObject.EvaluateJS($"initMap({terrainSize}," + $"{lat}," + $"{lng}," + $"{isTerrainProyection});");
            }
        );

        webViewObject.SetVisibility(true);

        // Set the margins and visibility of the WebView
        if(ADDTerrainButton) {
             webViewObject.SetMargins(0, 0, 0, Screen.height / 2);
        } else {
            webViewObject.SetMargins(0,0,0,0);
        }
        // Load the HTML file into the WebView
        webViewObject.LoadURL($"file://{Application.persistentDataPath}/StreamingAssets/map.html");
         // Continue executing the coroutine until the WebView is destroyed
        while (webViewObject != null) {
            yield return null;
        }
    }

    private void UpdateRatius(string input) {
        
        int ratius = string.IsNullOrEmpty(input) ? 0 : int.Parse(input);
        webViewObject.EvaluateJS($"updateRadius({ratius})");
    }

    // Coroutine to introduce a delay before calling getSelectedPosition()
    void RetrieveSelectedPosition() {
        DeactivateWebView();
        GetSelectedPosition();

    }

    void DeactivateWebView() {
        Destroy(webViewObject.gameObject);
         // Stop the coroutine if the GameObject is destroyed
        if (webCoroutine != null) {
            StopCoroutine(webCoroutine);
        }
    }

    // Callback method to be called from JavaScript
    void GetSelectedPosition() {

        // Use lat and lng in Unity as needed

        TerrainInfo.instance.location = selectedLocation;
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
