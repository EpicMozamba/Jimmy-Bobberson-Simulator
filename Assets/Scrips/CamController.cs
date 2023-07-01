using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class CamController : MonoBehaviour
{

    [SerializeField] private float sensX;
    [SerializeField] private float sensY;

    [SerializeField] private Transform orientation;

    private float xRotation;
    private float yRotation;

    // Start is called before the first frame update
    void Start()
    {
        //center mouse
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
    
    }

    // Update is called once per frame
    void Update()
    {
        //mouse input
        float mouseX = Input.GetAxisRaw("MouseX") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("MouseY") * Time.deltaTime * sensY;
        //?????????????????
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        //rotate
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
