using UnityEngine;

public class ToggleVrManagerVisibility : MonoBehaviour
{
    public Canvas canvas;
    [SerializeField] private bool defaultVisible = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        
    }
}
