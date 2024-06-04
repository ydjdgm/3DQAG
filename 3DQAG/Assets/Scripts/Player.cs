using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed;//플레이어 기본 이동속도
    float hAxis;
    float vAxis;
    bool LSDown;//left shift
    bool spaceDown;

    bool isJump;
    bool isDodge;

    Vector3 moveVec;//플레이어 이동 백터
    Vector3 dodgeVec;//Dodge시 백터

    Rigidbody rb;
    Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        LSDown = Input.GetButton("Walk");
        spaceDown = Input.GetButtonDown("Jump");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if(isDodge)
        {
            moveVec = dodgeVec;
        }

        transform.position += moveVec * moveSpeed * (LSDown ? 0.3f : 1f) * Time.deltaTime;//LSDown이 true면 * 0.3으로 이속 감소

        animator.SetBool("isRun", moveVec != Vector3.zero);
        animator.SetBool("isWalk", LSDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);//플레이어가 바라보는 방향 바꾸기
    }

    void Jump()
    {
        if (spaceDown && !isJump && moveVec == Vector3.zero && !isDodge)//Dodge중이 아니고, 움직이지 않을 때만 Jump가 나감
        {
            rb.AddForce(Vector3.up * 15f, ForceMode.Impulse);
            isJump = true;
            animator.SetBool("isJump", true);
            animator.SetTrigger("doJump");
        }
    }

    void Dodge()
    {
        if (spaceDown && !isDodge && !isJump && moveVec != Vector3.zero)//움직이고 있을 때만 Jump대신 Dodge가 나감
        {
            dodgeVec = moveVec;
            moveSpeed *= 2f;//대시 시 속도 가속
            animator.SetTrigger("doDodge");
            isDodge = true;
            Invoke("DodgeOut", 0.5f);
        }
    }
    void DodgeOut()
    {
        moveSpeed *= 0.5f;//가속된 속도 다시 원래대로
        isDodge = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isJump = false;
            animator.SetBool("isJump", false);
        }
    }
}
