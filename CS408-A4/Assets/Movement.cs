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
    private bool isJumping;

    //GameObject m_MainCamera;
    Camera m_MainCamera;

    // Start is called before the first frame update
    void Start()
    {
        //m_MainCamera = Camera.main.transform.parent.gameObject;
        m_MainCamera = Camera.main;
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
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
            isJumping = false;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpButtonPressedTime = Time.time;
        }

        if (Time.time - lastGroundTime <= jumpButtonGracePeriod) //Checks if grounded recently
        {
            characterController.stepOffset = originalStepOffset;
            ySpeed = -0.5f;

            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod) //Checks if 
            {
                animator.SetBool("isJumping", true);
                isJumping = true;
                ySpeed = jumpSpeed;
                jumpButtonPressedTime = 0f;
                lastGroundTime = 0f;
            }
        }
        else
        {
            characterController.stepOffset = 0;
            if ((isJumping && ySpeed < 0) || ySpeed < 2)
            {
                animator.SetBool("isFalling", true);
            }
        }
        //adjust the movement direction to the angle of the camera.
        moveDirection = Quaternion.Euler(0, m_MainCamera.transform.localEulerAngles.y, 0) * moveDirection;

        Vector3 velocity = moveDirection * magnitude;
        //move the camera with character
        m_MainCamera.transform.position += (velocity * Time.deltaTime);
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
        }
        float deltaHeight = transform.position.y - m_MainCamera.transform.position.y + 1.5f;
        m_MainCamera.transform.position += new Vector3(0, deltaHeight * Time.deltaTime * 10f, 0);
        if (Input.mouseScrollDelta.y != 0f)
        {
            m_MainCamera.transform.position = Vector3.MoveTowards(m_MainCamera.transform.position, transform.position, Input.mouseScrollDelta.y * Time.deltaTime * 100f);
        }

        //if (m_MainCamera.transform.position.y > transform.position.y + 1.5)
        //{
        //    m_MainCamera.transform.position += new Vector3(0,-1f * Time.deltaTime, 0);
        //}
        //if (m_MainCamera.transform.position.y < transform.position.y + 1.5)
        //{
        //    m_MainCamera.transform.position += new Vector3(0, 1f * Time.deltaTime, 0);
        //}
        getInput();
    }
    void getInput()
    {
        foreach (char c in Input.inputString)
        {
            switch (c)
            {
                case 'e'://rotate camera right
                    {
                        m_MainCamera.transform.RotateAround(transform.position, Vector3.up, -7000 * Time.deltaTime);
                        break;
                    }
                case 'q'://rotate camera left
                    {
                        m_MainCamera.transform.RotateAround(transform.position, Vector3.up, 7000 * Time.deltaTime);
                        break;
                    }
                case 'r'://rotate camera behind
                    {
                        Vector3 temp = transform.localEulerAngles;
                        Vector3 peteAngle = new Vector3(0, temp.y, 0);
                        temp = m_MainCamera.transform.localEulerAngles;
                        Vector3 cameraAngle = new Vector3(0, temp.y, 0);
                        m_MainCamera.transform.RotateAround(transform.position, Vector3.up, peteAngle.y-cameraAngle.y);
                        break;
                    }
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
                case 'h'://breathing
                    {
                        resetBools();
                        animator.SetBool("isBreathing", true);
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
        animator.SetBool("isBreathing", false);
    }
}
