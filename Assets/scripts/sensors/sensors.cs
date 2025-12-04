using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;
using System.Collections;

public class sensors : MonoBehaviour
{
    public Text firstSensorsText;
    public Text secondSensorsText;
    public float desiredAccuracyInMeters = 3f;
    public float updateDistanceInMeters = 3f; 
    private List<InputDevice> devices;
    private List<Func<string>> sensorReaders;
    private List<string> sensorNames; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        devices =  new List<InputDevice>{
                                Accelerometer.current, 
                                UnityEngine.InputSystem.Gyroscope.current, 
                                GravitySensor.current, 
                                AttitudeSensor.current,
                                LinearAccelerationSensor.current,
                                MagneticFieldSensor.current,
                                LightSensor.current,
                                PressureSensor.current,
                                ProximitySensor.current,
                                HumiditySensor.current,
                                AmbientTemperatureSensor.current,
                                StepCounter.current};
        
        sensorNames = new List<string>{
            "Accelerometer",
            "Gyroscope",
            "Gravity",
            "Attitude",
            "Linear Acceleration",
            "Magnetic Field",
            "Light Level",
            "Pressure",
            "Proximity",
            "Humidity",
            "Ambient Temperature",
            "Step Counter",
            "GPS"
        };
        
        sensorReaders = new List<Func<string>>{
            () => Accelerometer.current != null ? Accelerometer.current.acceleration.ReadValue().ToString() : "Not Exists",
            () => UnityEngine.InputSystem.Gyroscope.current != null ? UnityEngine.InputSystem.Gyroscope.current.angularVelocity.ReadValue().ToString() : "Not Exists",
            () => GravitySensor.current != null ? GravitySensor.current.gravity.ReadValue().ToString() : "Not Exists",
            () => AttitudeSensor.current != null ? AttitudeSensor.current.attitude.ReadValue().ToString() : "Not Exists",
            () => LinearAccelerationSensor.current != null ? LinearAccelerationSensor.current.acceleration.ReadValue().ToString() : "Not Exists",
            () => MagneticFieldSensor.current != null ? MagneticFieldSensor.current.magneticField.ReadValue().ToString() : "Not Exists",
            () => LightSensor.current != null ? LightSensor.current.lightLevel.ReadValue().ToString() : "Not Exists",
            () => PressureSensor.current != null ? PressureSensor.current.atmosphericPressure.ReadValue().ToString() : "Not Exists",
            () => ProximitySensor.current != null ? ProximitySensor.current.distance.ReadValue().ToString() : "Not Exists",
            () => HumiditySensor.current != null ? HumiditySensor.current.relativeHumidity.ReadValue().ToString() : "Not Exists",
            () => AmbientTemperatureSensor.current != null ? AmbientTemperatureSensor.current.ambientTemperature.ReadValue().ToString() : "Not Exists",
            () => StepCounter.current != null ? StepCounter.current.stepCounter.ReadValue().ToString() : "Not Exists",
            () => Input.location.status == LocationServiceStatus.Running ? 
                    "Lat: " + Input.location.lastData.latitude.ToString() + 
                    " Lon: " + Input.location.lastData.longitude.ToString() : 
                    "Not Exists"
        };
    }

    void Start() 
    {
        StartCoroutine(StartGPS());
    }

    // Update is called once per frame
    void Update()
    {
        string firstText = "";
        string secondText = "";
        for (int i = 0; i < sensorReaders.Count; ++i) 
        {
            if (i <= Math.Round((double)sensorReaders.Count / 2) - 1)
            {
                firstText += sensorNames[i] + ": " + sensorReaders[i]().ToString() + "\n";
            } else 
            {
                secondText += sensorNames[i] + ": " + sensorReaders[i]().ToString() + "\n";
            }
        }
        firstSensorsText.text = firstText;
        secondSensorsText.text = secondText;
    }

    protected IEnumerator StartGPS()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("GPS not enabled by user");
            yield break;
        }
        Input.location.Start(desiredAccuracyInMeters, updateDistanceInMeters);
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // If the service didn't initialize in 20 seconds this cancels location service use.
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location");
            yield break;
        }
        else
        {
            // If the connection succeeded, this retrieves the device's current location and displays it in the Console window.
            Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        } 
    }
    
    protected void OnEnable()
    {
        foreach (var device in devices)
        {
            if (device != null) 
            {
                InputSystem.EnableDevice(device);
            }
        }
    }

    protected void OnDisable()
    {
        foreach (var device in devices)
        {
            if (device != null) 
            {
                InputSystem.DisableDevice(device);
            }
        }
    }
}
