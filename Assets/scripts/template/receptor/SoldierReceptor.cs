using UnityEngine;

public class SoldierReceptor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Shield shield;
    public float forceMagnitude = 50f;
    void Start()
    {
        GameObject[] allWizards = GameObject.FindGameObjectsWithTag("Type 1");
        for (int i = 0; i < allWizards.Length; i++)
        {
            WizardNotifier notifier = allWizards[i].GetComponent<WizardNotifier>();
            if (notifier != null)
            {
                notifier.OnWizardCollided += RespondToWizardCollision;
            }
        }
        if (!shield.CompareTag("Type 2"))
        {
            Debug.LogError("Please assign a Shield with tag 'Type 2' to the SoldierReceptor script.");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void RespondToWizardCollision()
    {
        Vector3 distance = (shield.transform.position - transform.position).normalized;
        GetComponent<Rigidbody>().AddForce(distance * forceMagnitude, ForceMode.VelocityChange);
    }
}
