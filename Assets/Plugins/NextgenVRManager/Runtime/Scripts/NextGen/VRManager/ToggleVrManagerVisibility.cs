using UnityEngine;

public class ToggleVrManagerVisibility : MonoBehaviour
{
    public Canvas canvas;
    [SerializeField] private bool defaultVisible = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        canvas.enabled = defaultVisible;
    }

    private void Update()
    {
        bool lShift;
        bool f1;

        lShift = UnityEngine.InputSystem.Keyboard.current.leftShiftKey.isPressed;
        f1 = UnityEngine.InputSystem.Keyboard.current.f1Key.wasPressedThisFrame;

        if(lShift && f1)
        {
            canvas.enabled = !canvas.enabled;
        }
    }
}
