using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float curXRot;
    public float lookSens;

    [Header("Movement")]
    public float moveSpeed;
    private Vector2 curMovementInput;
    public float jumpForce;
    public LayerMask groundLayerMask;

    private Vector2 mouseDelta; 

    [HideInInspector]
    public bool  canLook = true;  

    //components
    private Rigidbody rig;

    public static PlayerController instance;

    void Awake()
    {
        rig = GetComponent<Rigidbody>();
        instance = this;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate()
    {
        Move();
    }

    void LateUpdate()
    {
        if (canLook == true)
            CameraLook();
    }

    void Move() 
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;
        dir.y = rig.velocity.y;
        rig.velocity = dir;
    }

    void CameraLook() 
    {
        curXRot += mouseDelta.y * lookSens;
        curXRot = Mathf.Clamp(curXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-curXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSens, 0);
    }

    public void OnLookInput(InputAction.CallbackContext context) 
    {
        mouseDelta = context.ReadValue<Vector2>();
    }
    public void OnMoveInput(InputAction.CallbackContext context) 
    {
        if (context.phase == InputActionPhase.Performed) {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled) {
            curMovementInput = Vector2.zero;
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context) 
    {
        //is this the first frame we're pressing the button?
        if (context.phase == InputActionPhase.Started) {
            if (IsGrounded()) {
                rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }
    
    bool IsGrounded() 
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (Vector3.up * 0.01f), Vector3.down)
        };

        for (int i  = 0; i < rays.Length; i++) {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask)) {
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay(transform.position + (transform.forward * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.forward * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (transform.right * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.right * 0.2f), Vector3.down);
    }

    public void ToggleCursor(bool toggle) 
    {
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
}