using UnityEngine;

public class ShieldReceptor : MonoBehaviour
{
    public ShieldNotifier shieldNotifier;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shieldNotifier.OnShieldCollision += ChangeColor;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ChangeColor()
    {
        Color randomColor = new Color(Random.value, Random.value, Random.value);
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            MeshRenderer renderer = child.GetComponent<MeshRenderer>();
            MeshFilter filter = child.GetComponent<MeshFilter>();
            if (renderer == null || filter == null)
            {
                Debug.LogError("Missing MeshRenderer or MeshFilter on child: " + child.name);
                continue;
            }
            Material newMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            newMat.color = randomColor;
            renderer.material = newMat;
        }
    }
}
