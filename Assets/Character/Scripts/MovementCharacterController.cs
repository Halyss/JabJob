using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementCharacterController : MonoBehaviour
{
    public float speed = 10f;
    public float jumpHeight = 2f;
    public float wallJumpForce = 4f;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 velocityWallJump;
    private float forwardInput;
    private float rightInput;
    private float gravityFactor;
    private float maxSpeed;

    private GroundChecker groundChecker;
    private Animator myAnimator;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        groundChecker = GetComponentInChildren<GroundChecker>();
        maxSpeed = Mathf.Sqrt(2 * speed * speed);
        myAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        //Gravity
        gravityFactor += Physics.gravity.y * Time.deltaTime;
        if (groundChecker.isGrounded && gravityFactor < 0)
        {
            gravityFactor = -2f;
        }

        // MOVE
        forwardInput = Input.GetAxis("Vertical");
        rightInput = Input.GetAxis("Horizontal");

        velocity = (forwardInput * transform.forward + rightInput * transform.right) * speed;
        velocity += gravityFactor * transform.up;

        if (Input.GetButtonDown("Jump") && groundChecker.isGrounded)
        {
            Jump();
            
        }

        if(velocityWallJump != Vector3.zero)
        {
            ApplyWallJump();
        }

        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        Debug.Log(velocity);

        controller.Move(velocity * Time.deltaTime);
        Animations();
    }

    public void Animations()
    {
        if (groundChecker.isGrounded) { myAnimator.SetBool("isGrounded", true); }
        else { myAnimator.SetBool("isGrounded", false); }
        if (Input.GetKeyDown("g")) { myAnimator.SetTrigger("dance"); }

        if (Input.GetKey("z")) { myAnimator.SetBool("isRunning", true); }
        else { myAnimator.SetBool("isRunning", false); }

        if (Input.GetKey("s")) { myAnimator.SetBool("isRunningBackwards", true); }
        else { myAnimator.SetBool("isRunningBackwards", false); }

        if (Input.GetKey("q")) { myAnimator.SetBool("isGoingLeft", true); }
        else { myAnimator.SetBool("isGoingLeft", false); }

        if (Input.GetKey("d")) { myAnimator.SetBool("isGoingRight", true); }
        else { myAnimator.SetBool("isGoingRight", false); }

    }

    private void ApplyWallJump()
    {
        velocity += velocityWallJump;
        velocityWallJump = Vector3.MoveTowards(velocityWallJump, Vector3.zero, speed * 2 * Time.deltaTime);
        if (groundChecker.isGrounded)
            velocityWallJump = Vector3.zero;
    }

    private void Jump()
    {
        gravityFactor = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        myAnimator.SetTrigger("jumpRunning");
        
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!groundChecker.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
                velocityWallJump = hit.normal * speed * 2;
                Debug.DrawRay(hit.point, hit.normal, Color.red, 1.25f);
            }
        }
    }
}
