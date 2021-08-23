

using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadTown : MonoBehaviour
{
    public string sceneName;

    // Only load town scene if playing as an executable
    #if !UNITY_EDITOR
    // Start is called before the first frame update
    void Awake()
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }
    #endif // UNITY_EDITOR

}
