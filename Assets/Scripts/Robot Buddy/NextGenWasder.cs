using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextGenWasder : MonoBehaviour
{
    public Vector2 mouseLookRot;
    public float movementSpeed = 6;
    public float mouseSens = 1;

    public Transform childTrans;

    // Start is called before the first frame update
    void Start()
    {
        childTrans = transform.GetChild(0);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        MouseLook();
        Wasd();
    }

    void MouseLook()
    {
        mouseLookRot += new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y")) * mouseSens;
        mouseLookRot = new Vector2(mouseLookRot.x, Mathf.Clamp(mouseLookRot.y, -90, 90));
        transform.localEulerAngles = new Vector3(0, mouseLookRot.x, 0);
        childTrans.localEulerAngles = new Vector3(mouseLookRot.y, 0, 0);
    }

    void Wasd()
    {
        Vector2 movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 movement = new Vector3(movementInput.x, 0, movementInput.y) * Time.deltaTime * movementSpeed;
        transform.Translate(movement);
    }
}
