using UnityEngine;

public class WizardNotifier : MonoBehaviour
{
    public delegate void WizardAction();
    public event WizardAction OnWizardCollided;
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
            OnWizardCollided();
        }
    }
}
