using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerController : MonoBehaviour
{
    [SerializeField] bool mouseInput;
    [Space]
    [SerializeField] bool isGrounded;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundDist = 0.4f;
    [Space]
    public bool isRunning = false;
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] float gravity = -9.81f;
    [Space]
    [SerializeField] int lineToMove = 1;
    [SerializeField] float lineDist = 2f;

    private Vector3 dir;
    private Vector3 fp;   //First touch position
    private Vector3 lp;   //Last touch position
    private float dragDistance;  //minimum distance for a swipe to be registered

    private CharacterController cc;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();

        dragDistance = Screen.height * 15 / 100; //dragDistance is 15% height of the screen
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning)
            SwipeControl();

        PlayerAnimation(); 
    }

    private void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundLayer);

        if (isGrounded && dir.y < 0)
            dir.y = -2f;

        dir.y += gravity * Time.deltaTime;

        if (isRunning)
        {
            dir.z = speed;
        }
        else
        {
            dir.z = 0f;
        }

        cc.Move(dir * Time.deltaTime);
        SwitchRoad();

    }

    private void SwipeControl()
    {
        if (mouseInput)
        {
            if (Input.GetMouseButtonDown(0))
            {
                fp = Input.mousePosition;
                lp = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                lp = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                lp = Input.mousePosition;
                OperateSwap();
            }
        }
        else
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    fp = touch.position;
                    lp = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    lp = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    lp = touch.position;
                    OperateSwap();
                }
            }
        } 
    }

    private void OperateSwap()
    {
        if (isGrounded)
        {
            if (Mathf.Abs(lp.x - fp.x) > dragDistance || Mathf.Abs(lp.y - fp.y) > dragDistance)
            {
                if (Mathf.Abs(lp.x - fp.x) > Mathf.Abs(lp.y - fp.y))
                {
                    animator.SetTrigger("strafe"); //Анимация по триггеру пуляет ивент RollAnimationEvent() ниже
                }
                else
                {
                    if (lp.y > fp.y)
                    {
                        Debug.Log("Up Swipe");
                        animator.SetTrigger("jump");
                    }
                    else
                    {
                        Debug.Log("Down Swipe");
                        animator.SetTrigger("jump");
                    }
                }
            }
            else
            {
                Debug.Log("Tap");
            }
        }
    }


    private void Jump()
    {
        dir.y = Mathf.Sqrt(jumpForce * -2f * gravity);
    }

    private void RollAnimationEvent()
    {
        Debug.Log("Roll!");

        if ((lp.x > fp.x))
        {
            Debug.Log("Right Swipe");

            if (lineToMove < 2 && isGrounded)
                lineToMove++;
        }
        else
        {
            Debug.Log("Left Swipe");

            if (lineToMove > 0 && isGrounded)
                lineToMove--;
        }
    }

    private void SwitchRoad()
    {
        Vector3 targetPos = transform.position.z * transform.forward;

        if (lineToMove == 0)
            targetPos += Vector3.left * lineDist;
        else if (lineToMove == 2)
            targetPos += Vector3.right * lineDist;

        transform.position = Vector3.Lerp(transform.position, targetPos, 10f * Time.deltaTime);
    }

    private void PlayerAnimation()
    {
        if (isGrounded)
        {
            animator.SetBool("isFalling", false);
            if (isRunning)
            {
                animator.SetBool("isRunning", true);
            }
            else
            {
                animator.SetBool("isRunning", false);
            }
        }
        else
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isFalling", true);
        }
    }



}
