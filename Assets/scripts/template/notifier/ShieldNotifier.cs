using UnityEngine;

public class ShieldNotifier : MonoBehaviour
{
    public delegate void ShieldCollisionHandler();
    public event ShieldCollisionHandler OnShieldCollision;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Wizard>() != null ||
            collision.gameObject.GetComponent<Soldier>() != null)
        {
            OnShieldCollision();
        }
    }
}
