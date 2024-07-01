using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{

    public float mouseSensitivity = 500f;

    public float topClamp = -90f;
    public float bottomClamp = 90f;


    float XRotation = 0f;
    float YRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //Look UP/DOWN
        XRotation -= mouseY;
        //Look RIGHT/lEFT
        YRotation += mouseX;

        //Clamp the rotation
        XRotation = Mathf.Clamp(XRotation, topClamp, bottomClamp);

        //Apply rotation
        transform.localRotation = Quaternion.Euler(XRotation, YRotation, 0f);
    }
}
