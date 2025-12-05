using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class WarriorNavigator : MonoBehaviour
{
    [Header("Configuración de GPS (Límites)")]
    public float minLat = 40.4165f; 
    public float maxLat = 40.4170f;
    public float minLon = -3.7040f;
    public float maxLon = -3.7030f;
    
    [Header("Ajustes de Movimiento")]
    public float speedMultiplier = 5.0f;
    public float slerpFactor = 2.0f;  

    [Header("Ajustes de Orientación")]
    public float headingOffset = 0f; 

    private bool isInsideRange = false;
    private Quaternion targetRotation;
    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        StartCoroutine(StartLocationService());
        if (Accelerometer.current != null)
            InputSystem.EnableDevice(Accelerometer.current);
    }

    void Update()
    {
        if (Input.location.status != LocationServiceStatus.Running) return;
        CheckGeofence();
        if (isInsideRange)
        {
            ApplyOrientation();
            ApplyMovement();
        }
        else
        {
            Debug.Log("Soldier stop: out of range.");
        }
    }

    void ApplyOrientation()
    {
        float rawHeading = Input.compass.magneticHeading;
        float correctedHeading = rawHeading + headingOffset;
        Quaternion northRotation = Quaternion.Euler(0, correctedHeading, 0); 
        transform.rotation = Quaternion.Slerp(transform.rotation, northRotation, Time.deltaTime * slerpFactor);
    }

    void ApplyMovement()
    {
        if (Accelerometer.current == null) return;

        Vector3 accel = Accelerometer.current.acceleration.ReadValue();
        float rawZ = -accel.z;
        if (Mathf.Abs(rawZ) < 0.1f) rawZ = 0;
        Vector3 movement = transform.forward * rawZ * speedMultiplier * Time.deltaTime;

        transform.Translate(movement, Space.World);
    }

    void CheckGeofence()
    {
        float currentLat = Input.location.lastData.latitude;
        float currentLon = Input.location.lastData.longitude;

        if (currentLat >= minLat && currentLat <= maxLat &&
            currentLon >= minLon && currentLon <= maxLon)
        {
            isInsideRange = true;
        }
        else
        {
            isInsideRange = false;
        }
    }

    IEnumerator StartLocationService()
    {
        if (!Input.location.isEnabledByUser) yield break;

        Input.location.Start(3f, 3f);

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1 || Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("It was not possible to determine the device location.");
        }
        else
        {
            Input.compass.enabled = true;
            Debug.Log("Location service started successfully."); 
        }
    }
    
    void OnDisable()
    {
        Input.location.Stop();
    }
}