using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    public string sceneName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void ChangeToScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
