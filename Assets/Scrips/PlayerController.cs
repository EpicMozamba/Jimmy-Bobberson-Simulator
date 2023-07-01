
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Transform camera;
    [SerializeField] private Transform feet;
    private Rigidbody rb;
    private CapsuleCollider collider;

    [Header("Movement")]
    [SerializeField][Range(0, 50)] private float walkSpeed;
    private Vector3 moveDirection;

    [Header("Looking")]
    [SerializeField][Range(0, 100)] private float xSensitivity;
    [SerializeField][Range(0, 100)] private float ySensitivity;
    [SerializeField][Range(0, 90)] private float topLookClamp;
    [SerializeField][Range(0, 90)] private float bottomLookClamp;
    [SerializeField] private bool invertLookX;
    [SerializeField] private bool invertLookY;
    private float xRotation;
    private float yRotation;

    [Header("Jumping")]
    [SerializeField][Range(0, 20)] private float jumpHeight;
    [SerializeField][Range(0, 3)] private float airMultiplier;

    [Header("Sprinting")]
    [SerializeField][Range(0, 50)] private float sprintSpeed;
    private bool isSprinting;

    [Header("Ground Check")]
    [SerializeField][Range(0, 3)] private float groundCheckRadius;
    [SerializeField] private LayerMask environmentMask;
    private bool isGrounded;

    [Header("Drag Control")]
    [SerializeField][Range(0, 10)] private float groundDrag;

    private void Start()
    {

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        collider = GetComponent<CapsuleCollider>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    private void Update()
    {

        isGrounded = Physics.CheckSphere(feet.position, groundCheckRadius, environmentMask);

        camera.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);

        ControlSpeed();

        if (isGrounded)
        {

            rb.drag = groundDrag;

        }
        else
        {

            rb.drag = 0;

        }
    }

    private void FixedUpdate()
    {

        if (isGrounded && !isSprinting)
        {

            rb.AddForce(moveDirection.normalized * walkSpeed * 5f, ForceMode.Force);

        }
        else if (isGrounded && isSprinting)
        {

            rb.AddForce(moveDirection.normalized * sprintSpeed * 5f, ForceMode.Force);

        }
        else if (!isGrounded)
        {

            rb.AddForce(moveDirection.normalized * walkSpeed * 5f * airMultiplier, ForceMode.Force);

        }
    }

    public void Move(Vector2 input)
    {

        moveDirection = transform.forward * input.y + transform.right * input.x;

    }

    public void Look(Vector2 input)
    {

        float mouseX = input.x * xSensitivity / 200f;
        float mouseY = input.y * ySensitivity / 200f;

        if (invertLookX)
        {

            yRotation -= mouseX;

        }
        else
        {

            yRotation += mouseX;

        }

        if (invertLookY)
        {

            xRotation += mouseY;

        }
        else
        {

            xRotation -= mouseY;

        }

        xRotation = Mathf.Clamp(xRotation, -bottomLookClamp, topLookClamp);

    }

    private void ControlSpeed()
    {

        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVelocity.magnitude > walkSpeed)
        {

            Vector3 limitedVelocity = flatVelocity.normalized * (isSprinting ? sprintSpeed : walkSpeed);
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);

        }
    }

    public void Sprint()
    {

        if (isGrounded)
        {

            isSprinting = !isSprinting;

        }
    }

    public void Jump()
    {

        if (isGrounded)
        {

            rb.velocity = new Vector3(rb.velocity.x / 2, 0f, rb.velocity.z / 2);

            rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);

        }
    } 
}