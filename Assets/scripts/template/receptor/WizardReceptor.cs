using UnityEngine;

public class WizardReceptor : MonoBehaviour
{
    public Shield shield;
    public float forceMagnitude = 50f;

    void Start()
    {
        GameObject[] allSoldiers = GameObject.FindGameObjectsWithTag("Type 2");
        for (int i = 0; i < allSoldiers.Length; i++)
        {
            SoldierNotifier notifier = allSoldiers[i].GetComponent<SoldierNotifier>();
            if (notifier != null)
            {
                notifier.OnSoldierCollided += RespondToSoldierCollision;
            }
        }
        if (!shield.CompareTag("Type 1"))
        {
            Debug.LogError("Please assign a Shield with tag 'Type 1' to the WizardReceptor script.");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void RespondToSoldierCollision()
    {
        Vector3 distance = (shield.transform.position - transform.position).normalized;
        GetComponent<Rigidbody>().AddForce(distance * forceMagnitude, ForceMode.VelocityChange); 
    }
}
