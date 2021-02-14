using System;
using System.Collections;
using UnityEngine;

public class MovementRigidbodyController : MonoBehaviour
{
    public float speed = 10f;
    public float maxSpeed = 10f;
    public float jumpHeight = 2f;
    [Range(1, 2)] public float counterForce = 1.2f;
    [Range(0, 1)] public float airSpeed = 0.5f;
    public float directionControl = 1f;

    [Header("Camera")]
    public float fovSmoothingSpeed = 1f;
    public float defaultCameraFov = 60f;

    public Rigidbody body { get; private set;  }

    private Animator myAnimator;
    public FirstPersonCamera cameraController { get; private set; }
    private GroundChecker groundChecker;
    private Vector3 targetVelocity;
    private float forwardInput;
    private float rightInput;
    private float directionFactor;
    private readonly float multiplier = 1f;

    private bool inContactWithWall = false;
    private Vector3 wallNormal;


    private float peekSpeed = 0;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        groundChecker = GetComponentInChildren<GroundChecker>();
        cameraController = GetComponentInChildren<FirstPersonCamera>();
        myAnimator = GetComponent<Animator>();

    }

    void Update()
    {
   
        GetMoveInput();

        LimitInputSpeed();

        GetDirectionfactor();

        UpdateTargetVelocity();

        Jump();

        UpdateCameraFovWithSpeed();

        // Debug
        Debug.DrawRay(transform.position, body.velocity.normalized * 5);
        Debug.DrawRay(transform.position, targetVelocity.normalized * 5, Color.red);
    }

    void FixedUpdate()
    {
        CounterMovement();

        MoveThePlayer();

        myAnimator.SetBool("isGrounded", groundChecker.isGrounded);

        Animations();

        
    }

    //Animations mouvements
    public void Animations()
    {
        if (Input.GetKey("z")) { Run(); }
        else { myAnimator.SetBool("isRunning", false); }
        
        if (Input.GetKey("s")) { RunBackwards(); }
        else { myAnimator.SetBool("isRunningBackwards", false); }

        if (Input.GetKey("q")) { myAnimator.SetBool("isGoingLeft", true); }
        else { myAnimator.SetBool("isGoingLeft", false); }
       
        if (Input.GetKey("d")) { myAnimator.SetBool("isGoingRight", true); }
        else { myAnimator.SetBool("isGoingRight", false); }

    }

    //Vitesse depl.
    private void UpdateTargetVelocity()
    {
        targetVelocity = forwardInput * transform.forward + rightInput * transform.right;
        targetVelocity *= speed * directionFactor;
    }

    //Mouvements
    private void GetMoveInput()
    {
        forwardInput = Input.GetAxisRaw("Vertical");
        rightInput = Input.GetAxisRaw("Horizontal");
    }

    //Saut
    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && groundChecker.isGrounded)
        {
            body.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -1f * Physics.gravity.y), ForceMode.VelocityChange);
            myAnimator.SetTrigger("jumpRunning");
        }
        
    }

    //Direction
    private void GetDirectionfactor()
    {
        Vector3 bodyVector = body.velocity.normalized;
        bodyVector.y = 0;

        float angle = Vector3.Angle(bodyVector, targetVelocity);

        directionFactor = 1 + (angle / 180 * directionControl);
    }

    //Vit. Cam.
    private void UpdateCameraFovWithSpeed()
    {
        float fov = cameraController.Camera.fieldOfView;
        float nextFov = defaultCameraFov + (body.velocity.magnitude * 2);
        float lerpSpeed = Time.deltaTime * fovSmoothingSpeed;

        cameraController.Camera.fieldOfView = Mathf.Lerp(fov, nextFov, lerpSpeed);
    }

    //
    private void CounterMovement()
    {
        if (!groundChecker.isGrounded) return;

        Vector3 localVelocity = transform.InverseTransformDirection(body.velocity);

        

        if (forwardInput == 0 && Mathf.Abs(localVelocity.z) > 0.125f)
            body.AddForce(-transform.forward * localVelocity.normalized.z * speed * counterForce);
            


        if (rightInput == 0 && Mathf.Abs(localVelocity.x) > 0.125f)
            body.AddForce(-transform.right * localVelocity.normalized.x * speed * counterForce);
            
    }

    //Vitesse
    private void LimitInputSpeed()
    {
        if (body.velocity.magnitude > maxSpeed && groundChecker.isGrounded)
        {
            forwardInput = 0;
            rightInput = 0;
        }
    }
    
    private void MoveThePlayer()
    {
        body.AddForce(targetVelocity * GetSpeedMultiplier());
    }

    private float GetSpeedMultiplier()
    {
        float multiplier = this.multiplier;

        if (!groundChecker.isGrounded)
            multiplier = airSpeed;

        return multiplier;
    }

    //Collisions
    void OnCollisionStay(Collision collision)
    {
        var contact = collision.GetContact(0);
        if (IsContactAWall(contact))
        {
            TouchWall(contact);
        }
        else
        {
            ResetWall();
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.contactCount == 0)
        {
            ResetWall();
        }
    }
    private void TouchWall(ContactPoint contact)
    {
        inContactWithWall = true;
        wallNormal = contact.normal;
    }
    private void ResetWall()
    {
        inContactWithWall = false;
        wallNormal = Vector3.zero;
    }
    public bool IsContactAWall(ContactPoint contact)
    {
        return contact.normal.x >= 0.4f || contact.normal.x <= -0.4f || contact.normal.z >= 0.4f || contact.normal.z <= -0.4f;
    }

    //Course Av. & Arr.
    public void Run()
    {
        myAnimator.SetBool("isRunning", true);
    }
    public void RunBackwards()
    {
        myAnimator.SetBool("isRunningBackwards", true);
    }
}
