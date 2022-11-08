using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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
    private bool isSwimming = false;
    private bool isDiving = false;
    private float gravity = 3f;

    [SerializeField]
    GameObject water;
    //GameObject m_MainCamera;
    [SerializeField]
    CinemachineVirtualCamera m_MainCamera;
    //Camera m_MainCamera;

    // Start is called before the first frame update
    void Start()
    {
        //m_MainCamera = Camera.main;
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
        ySpeed += Physics.gravity.y * Time.deltaTime * gravity;

        if (transform.position.y + 1.1f < water.transform.position.y)
        {
            animator.SetBool("isSwimming", true);
            //transform.position += (new Vector3(0, transform.position.y + 1.1f - water.transform.position.y, 0) * Time.deltaTime);
            isSwimming = true;
        }
        else
        {
            animator.SetBool("isSwimming", false);
            isSwimming = false;
        }
        //if (false)
        if (isSwimming)
        {
            ySpeed = 0f;
            if (Input.inputString != "")
            {
                animator.SetBool("isDiving", false);
                animator.SetBool("isSwimUp", false);
            }
            foreach (char c in Input.inputString)
            {
                switch (c)
                {
                    case '-'://dive
                        {
                            ySpeed -= 100f;
                            animator.SetBool("isDiving", true);
                            break;
                        }
                    case '+'://Swim Up
                        {
                            ySpeed += 100f;
                            animator.SetBool("isSwimUp", true);
                            //animator.SetBool("isDiving", false);
                            break;
                        }
                }
            }
        }
        if (characterController.isGrounded)
        {
            lastGroundTime = Time.time;
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
            animator.SetBool("isDiving", false);
            isJumping = false;
            //ySpeed = -0.5f;
        }
        //if (Input.GetButtonDown("Jump"))
        //{
        //   jumpButtonPressedTime = Time.time;
        //}

        if (Time.time - lastGroundTime <= jumpButtonGracePeriod) //Checks if grounded recently
        {
            characterController.stepOffset = originalStepOffset;
            //ySpeed = -0.5f;
            if (Input.GetButtonDown("Jump"))
            {
               jumpButtonPressedTime = Time.time;
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
        //if (isSwimming != true)
        //{
            velocity.y = ySpeed;
        //}

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
        //float deltaHeight = transform.position.y - m_MainCamera.transform.position.y + 1.5f;
        //m_MainCamera.transform.position += new Vector3(0, deltaHeight * Time.deltaTime * 10f, 0);
        
    

        if (Input.mouseScrollDelta.y != 0f)
        {
            CinemachineComponentBase componentBase = m_MainCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
            if (componentBase is CinemachineFramingTransposer)
            {
                (componentBase as CinemachineFramingTransposer).m_CameraDistance = (componentBase as CinemachineFramingTransposer).m_CameraDistance + Input.mouseScrollDelta.y * Time.deltaTime * 100f; // your value
            }
            //m_MainCamera.transform.position = Vector3.MoveTowards(m_MainCamera.transform.position, transform.position, Input.mouseScrollDelta.y * Time.deltaTime * 100f);
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
        if (isSwimming)
            ySpeed = 0f;
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
                case 'g'://breathing
                    {
                        resetBools();
                        animator.SetBool("isBreathing", true);
                        break;
                    }
                case 'h'://dancing
                    {
                        resetBools();
                        animator.SetBool("isDancing", true);
                        break;
                    }
                case 'j'://Flair
                    {
                        resetBools();
                        animator.SetBool("isFlair", true);
                        break;
                    }
                case 'k'://Shuffle
                    {
                        resetBools();
                        animator.SetBool("isShuffle", true);
                        break;
                    }
                case 'l'://Range of Movement
                    {
                        resetBools();
                        animator.SetBool("isStretching", true);
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
        animator.SetBool("isFlair", false);
        animator.SetBool("isShuffle", false);
        animator.SetBool("isStretching", false);
    }
}
