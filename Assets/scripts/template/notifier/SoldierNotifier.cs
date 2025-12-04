using UnityEngine;

public class SoldierNotifier : MonoBehaviour
{
    public delegate void SoldierAction();
    public event SoldierAction OnSoldierCollided;
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
        if (collision.gameObject.CompareTag("Cube"))
        {
            OnSoldierCollided();
        }
    }
}
