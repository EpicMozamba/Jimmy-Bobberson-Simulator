using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintModifier;
    private bool isSprinting;

    [SerializeField] private float groundDrag;

    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    [SerializeField] private float sprintJumpVelocity;
    bool readyToJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask ground;
    private bool grounded;

    [SerializeField] private Transform orientation;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    private Rigidbody rb;

    [Header("Camera Settings")]
    [SerializeField] private new Camera camera;
    [SerializeField] private new Camera weaponCamera;
    [SerializeField] private float baseFov = 87.5f;
    [SerializeField] private float sprintFov = 99.9f;
    [SerializeField] private float zoomTime;
    private Coroutine fovCoroutine = null;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;

        //needed when camera is not serialized(public) to automatically set it, only works when u have 1 camera in the scene
        //camera = FindObjectOfType<Camera>();
        camera.fieldOfView = 87.5f;

    }


    // Update is called once per frame
    void Update()
    {
        //ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);

        MyInput();
        SpeedControl();

        //Debug.Log(grounded);
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        //Debug.Log(readyToJump);
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position - new Vector3(0f, playerHeight * 0.5f + 0.2f));
    } */

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //undo
        //when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }


    private void MovePlayer()
    {
        moveDirection = (orientation.forward * verticalInput + (orientation.right * horizontalInput * 0.8f));

        //when grounded
        isSprinting = Input.GetKey(KeyCode.LeftShift);
        if (grounded)
        {
            if (isSprinting)
            {
                Debug.Log("sprinting");
                if (fovCoroutine == null)
                    fovCoroutine = StartCoroutine(LerpFov(baseFov, sprintFov));
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * sprintModifier, ForceMode.Force);

            }
            else
            {
                Debug.Log("walking");
                if (fovCoroutine == null)
                    fovCoroutine = StartCoroutine(LerpFov(sprintFov, baseFov));
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

            }

        }

        //when not grounded
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

    }

    private IEnumerator LerpFov(float startFov, float targetFov)
    {
        float currentTime = 0f;
        while (currentTime < zoomTime)
        {
            currentTime += Time.deltaTime;
            camera.fieldOfView = Mathf.Lerp(startFov, targetFov, currentTime / zoomTime);
            weaponCamera.fieldOfView = Mathf.Lerp(startFov, targetFov, currentTime / zoomTime);
            //Debug.Log("i love cheese and jons and i hate chiken also grande is amazing");
            yield return null;

        }
        camera.fieldOfView = targetFov;
        weaponCamera.fieldOfView = targetFov;

        fovCoroutine = null;
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //limit velocidad when needed (character actual movement speed can exceed set speed)
        if (flatVel.magnitude > moveSpeed && !Input.GetKey(KeyCode.LeftShift))
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
        //allows sprint jumping to be better than walk jumping but prevents full on bhopping using sprintJumpVelocity
        else if (flatVel.magnitude > moveSpeed && Input.GetKey(KeyCode.LeftShift) && !grounded)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed * sprintJumpVelocity;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        //rest y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
