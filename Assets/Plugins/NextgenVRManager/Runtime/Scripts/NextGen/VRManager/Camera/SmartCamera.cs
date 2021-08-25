using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class SmartCamera : MonoBehaviour
{
    private static SmartCamera Instance;

    public Transform focusTarget;
    public float focusTargetDistance = 5;
    public CameraMount mountedTo;
    public CameraMount mountTo;

    private Camera[] cameras;

    //Input Settings
    public float movementSpeed = 4;
    public float lookSensitivity = 0.1f;

    //Input.
    private Vector3 wasdInput;
    private Vector2 lookInput;
    [SerializeField] private float lookX;
    [SerializeField] private float lookY;
    [SerializeField] private Vector2 mouseScroll;

    void Awake()
    {
        Instance = this;
        cameras = GetComponentsInChildren<Camera>();
    }

    void LateUpdate()
    {
        UpdateInput();
        UpdateCamera();

        if(mountTo != null)
        {
            BindToMount(mountTo);
            mountTo = null;
        }
    }

    void UpdateInput()
    {
        bool rc = UnityEngine.InputSystem.Mouse.current.rightButton.isPressed;

        // Cursors
        Cursor.lockState = rc ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = rc ? false : true;

        float w = UnityEngine.InputSystem.Keyboard.current.wKey.isPressed ? 1 : 0;
        float a = UnityEngine.InputSystem.Keyboard.current.aKey.isPressed ? 1 : 0;
        float s = UnityEngine.InputSystem.Keyboard.current.sKey.isPressed ? 1 : 0;
        float d = UnityEngine.InputSystem.Keyboard.current.dKey.isPressed ? 1 : 0;
        float up = UnityEngine.InputSystem.Keyboard.current.spaceKey.isPressed ? 1 : 0;
        float down = UnityEngine.InputSystem.Keyboard.current.leftCtrlKey.isPressed ? 1 : 0;

        mouseScroll = UnityEngine.InputSystem.Mouse.current.scroll.ReadValue();

        wasdInput = rc ? new Vector3(d - a, up - down, w - s).normalized : Vector3.zero;

        lookInput = rc ? UnityEngine.InputSystem.Mouse.current.delta.ReadValue() : Vector2.zero;

        lookX += lookInput.x * lookSensitivity;
        while (lookX > 180) lookX -= 360;
        while (lookX < -180) lookX += 360;

        lookY = ClampAngle(lookY - lookInput.y * lookSensitivity, -90, 90);

        if (wasdInput != Vector3.zero)
        {
            if (focusTarget != null)
                focusTarget = null;
            if (mountedTo != null)
                UnbindFromMount();
        }
    }

    void UpdateCamera()
    {
        Quaternion rotation = Quaternion.AngleAxis(lookX, Vector3.up) * Quaternion.AngleAxis(lookY, Vector3.right);

        if (focusTarget != null)
        {
            Vector3 vRotation = rotation * -Vector3.forward;

            transform.position = focusTarget.position + vRotation * focusTargetDistance;

            transform.rotation = rotation;

            if(mouseScroll.y != 0)
            {
                focusTargetDistance *= 1 - (mouseScroll.y / 600);
                if (focusTargetDistance < 0.5f)
                    focusTargetDistance = 0.5f;
                if (focusTargetDistance > 25)
                    focusTargetDistance = 25;
            }

        }
        else if (mountedTo != null)
        {
            transform.SetPositionAndRotation(mountedTo.transform.position, mountedTo.transform.rotation);
        }
        else
        {
            transform.rotation = rotation;
            transform.Translate(wasdInput * movementSpeed * Time.deltaTime, transform);
        }
    }

    // Clamping Euler angles
    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    public void BindToMount(CameraMount mount)
    {
        if (mountedTo != null)
            UnbindFromMount();

        foreach (Camera camera in cameras)
        {
            camera.orthographic = mount.orthographic;
            camera.fieldOfView = mount.fov;
        }

        mountedTo = mount;
    }

    public void UnbindFromMount()
    {
        lookX = transform.rotation.eulerAngles.y;
        lookY = transform.rotation.eulerAngles.x;

        mountedTo = null;
    }

    public static void SetFocus(Transform _focusTarget)
    {
        Instance.focusTarget = _focusTarget;
        Instance.focusTargetDistance = 3;
    }
}
