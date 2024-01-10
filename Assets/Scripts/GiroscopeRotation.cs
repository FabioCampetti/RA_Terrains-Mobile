using UnityEngine;

public class GiroscopeRotation : MonoBehaviour
{
    Gyroscope gyro;
 
	float defasaje=0;
	float oldGrados=0;
 
	// Use this for initialization
	void Start () {
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
 
		gyro = Input.gyro; // Store the reference for Gyroscope sensor 
		gyro.enabled = true; //Enable the Gyroscope sensor 
 
		Input.location.Start ();
		Input.compass.enabled = true;
 
	}
 
	// Update is called once per frame
	void Update () {
        #if UNITY_EDITOR
        //Debug.Log("editor");
        #elif UNITY_ANDROID
        actualizarDatosGyro ();
		#endif
	}
 
 
	public void actualizarDatosGyro(){
		Quaternion nuevo = Input.gyro.attitude;
 
		nuevo.x*=-1.0f;
		nuevo.y*=-1.0f;
		nuevo = Quaternion.Euler(90, 0, 0)*nuevo;
 
		nuevo.eulerAngles=new Vector3(nuevo.eulerAngles.x,nuevo.eulerAngles.y-defasaje,nuevo.eulerAngles.z);
 
		transform.localRotation =  nuevo;
	}
}
