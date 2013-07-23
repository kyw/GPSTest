using UnityEngine;
using System.Collections;
/** Accessing the GPS Data over JNI calls
 *  Please check the Java sources under Plugins/Android
 * */
public class GPSManager : MonoBehaviour
{
    AndroidJavaClass gpsActivityJavaClass;
    LocationService locationService;
	LocationInfo locationInfo;
    int maxWait = 10;

    void Start()
    {
        AndroidJNI.AttachCurrentThread();
        gpsActivityJavaClass = new AndroidJavaClass("com.fobikr.gpstest.GPSTest");
        locationService = Input.location;

        // First, check if user has location service enabled
        if (!locationService.isEnabledByUser)
            GameObject.Find("gps_output2").guiText.text = "Unable to determine device location";
        // Start service before querying location
        locationService.Start(0.5f, 0.5f);

        CheckService();
    }
    void Update()
    {
		locationInfo = Input.location.lastData;
        string lat = gpsActivityJavaClass.CallStatic<string>("getLat");
		string lng = gpsActivityJavaClass.CallStatic<string>("getLng");

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            GameObject.Find("gps_output2").guiText.text = "Unable to determine device location";
        }

        else
        {
            GameObject.Find("gps_output2").guiText.text = "" + locationInfo.latitude + ", " + locationInfo.longitude;
        }

        if (lat != null && lng != null)
        {
            //speed = float.Parse(speedMessage);
            GameObject.Find("gps_output").guiText.text = lat + ", " + lng;
        }
        else
        {
            GameObject.Find("gps_output").guiText.text = "No Position.";
        }
    }

    IEnumerator CheckService()
    {
        while (locationService.status
                  == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            GameObject.Find("gps_output2").guiText.text = "Timed out";
        }
    }
}