using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebMapView : MonoBehaviour {
    WebViewObject webViewObject;
    private Coroutine webCoroutine;

    public Button ADDTerrainButton;

    public Button BackButton;


    void OnEnable()
    {
        ADDTerrainButton = transform.Find("ADDTerrain")?.GetComponent<Button>();
        if (ADDTerrainButton)
        {
            ADDTerrainButton.onClick.AddListener(() => LoadPosition());
        }
        
        BackButton = transform.Find("Back")?.GetComponent<Button>();
         if (BackButton)
        {
            BackButton.onClick.AddListener(() => DeactivateWebView());
        }
        webCoroutine = StartCoroutine(WebCoroutine());
        Debug.Log("INICIO WEB CORUTINE");
    }

    // Coroutine example
    IEnumerator WebCoroutine() {
        GameObject terrainMenuCanvas = GameObject.Find("TerrainMenuCanvas");

        webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
        webViewObject.transform.SetParent(terrainMenuCanvas.transform);
        webViewObject.Init(
            cb: (msg) =>
            {
                Debug.Log($"CallFromJS[{msg}]");
                if (msg is string) {
                    double lat,lng;
                    Debug.Log("MENSAJE: " + msg);
                    string[] coordinates = msg.Split(',');
                    if (coordinates.Length == 2 && double.TryParse(coordinates[0], out lat) && double.TryParse(coordinates[1], out lng)) {
                        RetrieveSelectedPosition(lat, lng);
                    }
                }
            },
            err: (msg) =>
            {
                Debug.Log($"CallOnError[{msg}]");
            },
            httpErr: (msg) =>
            {
                Debug.Log($"CallOnHttpError[{msg}]");
            },
            started: (msg) =>
            {
                Debug.Log($"CallOnStarted[{msg}]");
            },
            hooked: (msg) =>
            {
                Debug.Log($"CallOnHooked[{msg}]");
            },
            cookies: (msg) =>
            {
                Debug.Log($"CallOnCookies[{msg}]");
            },
            ld: (msg) =>
            {
                Debug.Log($"CallOnLoaded[{msg}]");
                webViewObject.EvaluateJS(@"
                    window.Unity = { 
                        call: function(msg) { 
                            window.location = 'unity:' + msg; 
                        } 
                    }
                ");
                webViewObject.EvaluateJS($"Unity.call('ua=' + navigator.userAgent)");
                // Call the JavaScript function to initialize the map
                webViewObject.EvaluateJS("initMap();");
            }
        );

        webViewObject.SetVisibility(true);

        // Set the margins and visibility of the WebView
        if(ADDTerrainButton) {
             webViewObject.SetMargins(5, 0, 0, Screen.height / 2);
             Debug.Log("Encontre el boton");
        } else {
            webViewObject.SetMargins(0,0,0,0);
            Debug.Log("No encontre el boton");
        }
        Debug.Log($"file://{Application.persistentDataPath}/StreamingAssets/map.html");
        // Load the HTML file into the WebView
        webViewObject.LoadURL($"file://{Application.persistentDataPath}/StreamingAssets/map.html");
        yield break;
    }

    private void LoadPosition() {
        Debug.Log("LOAD POSITION");
        webViewObject.EvaluateJS("getSelectedPosition();");
    }

    // Example method to deactivate the WebView
    void DeactivateWebView() {
        // Hide the WebView
        webViewObject.SetVisibility(false);

        // Stop the coroutine when deactivating the WebView
        if (webCoroutine != null)
        {
            StopCoroutine(webCoroutine);
        }

        // Destroy the WebViewObject
        if (webViewObject != null)
        {
            Destroy(webViewObject.gameObject);
            webViewObject = null;
        }
    }

    // Coroutine to introduce a delay before calling getSelectedPosition()
    void RetrieveSelectedPosition(double lat, double lng) {
        DeactivateWebView();
        GetSelectedPosition(lat,lng);
    }

    // Callback method to be called from JavaScript
    void GetSelectedPosition(double lat, double lng) {

        // Use lat and lng in Unity as needed
        Debug.Log($"Selected Position - Latitude: {lat}, Longitude: {lng}");

        TerrainProyectionEventManager.instance.InvokeCoordinatesReceived(new Location(lat,lng));
        if(!ADDTerrainButton) {
            GameManager.instance.DesactivateCamera();
            GameManager.instance.TerrainProyectionMenu();
            gameObject.AddComponent<TerrainProyection>().LoadTerrain();
        }

    }
}
