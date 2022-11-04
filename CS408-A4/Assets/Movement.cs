using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public float jumpSpeed;
    public float jumpButtonGracePeriod;

    private Animator animator;
    private CharacterController characterController;
    private float ySpeed;
    private float originalStepOffset;
    private float lastGroundTime;
    private float jumpButtonPressedTime;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;
        jumpButtonPressedTime = -5f;
    }

    // Update is called once per frame
    void Update()
    {

        float leftRight = Input.GetAxis("Horizontal");
        float upDown = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(leftRight, 0, upDown);
        float magnitude = Mathf.Clamp01(moveDirection.magnitude) * speed;
        moveDirection.Normalize();
        ySpeed += Physics.gravity.y * Time.deltaTime;

        if (characterController.isGrounded)
        {
            lastGroundTime = Time.time;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpButtonPressedTime = Time.time;
        }

        if (Time.time - lastGroundTime <= jumpButtonGracePeriod)
        {
            characterController.stepOffset = originalStepOffset;
            ySpeed = -0.5f;

            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
            {
                ySpeed = jumpSpeed;
                jumpButtonPressedTime = 0f;
                lastGroundTime = 0f;
            }
        }
        else
        {
            characterController.stepOffset = 0;
        }

        Vector3 velocity = moveDirection * magnitude;
        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);

        if (moveDirection != Vector3.zero)
        {
            resetBools();
            animator.SetBool("isMoving", true);
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("isMoving", false);
            getInput();
        }
        
    }
    void getInput()
    {
        foreach (char c in Input.inputString)
        {
            switch (c)
            {
                case 'f'://t pose to pose 1
                    {
                        resetBools();
                        animator.SetBool("isPose1", true);
                        break;
                    }
                case 'b'://t pose to pose 2
                    {
                        resetBools();
                        animator.SetBool("isPose2", true);
                        break;
                    }
                case 'g'://dancing
                    {
                        resetBools();
                        animator.SetBool("isDancing", true);
                        break;
                    }
            }
        }

    }
    void resetBools()
    {
        animator.SetBool("isDancing", false);
        animator.SetBool("isPose1", false);
        animator.SetBool("isPose2", false);
        //animator.SetBool("isIdle", false);
    }
}
