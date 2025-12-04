using UnityEngine;
using UnityEngine.InputSystem; // Necesario para el Accelerometer.current
using System.Collections;

public class WarriorNavigator : MonoBehaviour
{
    [Header("Configuración de GPS (Límites)")]
    public float minLat = 40.4165f; // Ejemplo: Puerta del Sol, Madrid
    public float maxLat = 40.4170f;
    public float minLon = -3.7040f;
    public float maxLon = -3.7030f;
    
    [Header("Ajustes de Movimiento")]
    public float speedMultiplier = 5.0f; // Sensibilidad del movimiento
    public float slerpFactor = 2.0f;     // Suavizado de rotación

    private bool isInsideRange = false;
    private Quaternion targetRotation;

    void Start()
    {
        // 1. Configuración de Pantalla: CRUCIAL para que las coordenadas coincidan
        // Forzamos LandscapeLeft (Botón home/swipe a la derecha, muesca a la izquierda)
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        // 2. Iniciar Servicios
        StartCoroutine(StartLocationService());
        
        // Habilitar el acelerómetro si no está activo (del New Input System)
        if (Accelerometer.current != null)
            InputSystem.EnableDevice(Accelerometer.current);
    }

    void Update()
    {
        // Si el GPS no está listo o el usuario no dio permisos, no hacemos nada
        if (Input.location.status != LocationServiceStatus.Running) return;

        // A. Comprobar Geofencing
        CheckGeofence();

        if (isInsideRange)
        {
            // B. Aplicar Orientación (Brújula)
            ApplyOrientation();

            // C. Aplicar Movimiento (Acelerómetro)
            ApplyMovement();
        }
        else
        {
            // Opcional: Feedback visual de que está detenido
            Debug.Log("Guerrero detenido: Fuera de rango GPS.");
        }
    }

    // --- LÓGICA DE ORIENTACIÓN ---
    void ApplyOrientation()
    {
        // Input.compass.trueHeading devuelve el ángulo respecto al Norte geográfico (0=Norte, 90=Este)
        // Unity ajusta esto automáticamente según Screen.orientation, pero aplicamos Slerp.
        float heading = Input.compass.trueHeading;

        // Creamos la rotación objetivo solo en el eje Y (suelo)
        Quaternion northRotation = Quaternion.Euler(0, heading, 0);

        // Interpolación esférica (Slerp)
        transform.rotation = Quaternion.Slerp(transform.rotation, northRotation, Time.deltaTime * slerpFactor);
    }

    // --- LÓGICA DE MOVIMIENTO ---
    void ApplyMovement()
    {
        if (Accelerometer.current == null) return;

        // Leemos la aceleración del New Input System
        Vector3 accel = Accelerometer.current.acceleration.ReadValue();

        // Interpretación: "Invertir el valor z"
        // Z positivo en el dispositivo suele ser "hacia la pantalla".
        // Al inclinar hacia adelante para "caminar", Z se vuelve negativo.
        // Invertimos (-accel.z) para que inclinar hacia adelante signifique avanzar.
        float rawZ = -accel.z;

        // Zona muerta para evitar temblores (opcional)
        if (Mathf.Abs(rawZ) < 0.1f) rawZ = 0;

        // Calculamos el desplazamiento
        // Usamos Vector3.forward relativo al mundo (Norte) o relativo al objeto. 
        // Como el objeto ya mira al Norte gracias a ApplyOrientation, usamos transform.forward.
        Vector3 movement = transform.forward * rawZ * speedMultiplier * Time.deltaTime;

        transform.Translate(movement, Space.World);
    }

    // --- LÓGICA DE GEOFENCING ---
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

    // --- INICIALIZACIÓN (Basado en tu script sensors.cs pero simplificado) ---
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
            Debug.LogError("Fallo al iniciar GPS");
        }
        else
        {
            // IMPORTANTE: Habilitar la brújula una vez el GPS inicia
            Input.compass.enabled = true;
            Debug.Log("GPS y Brújula listos.");
        }
    }
    
    void OnDisable()
    {
        Input.location.Stop();
    }
}