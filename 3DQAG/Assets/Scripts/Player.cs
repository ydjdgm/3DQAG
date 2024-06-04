using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed;//�÷��̾� �⺻ �̵��ӵ�
    float hAxis;
    float vAxis;
    bool LSDown;//left shift
    bool spaceDown;

    bool isJump;
    bool isDodge;

    Vector3 moveVec;//�÷��̾� �̵� ����
    Vector3 dodgeVec;//Dodge�� ����

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

        transform.position += moveVec * moveSpeed * (LSDown ? 0.3f : 1f) * Time.deltaTime;//LSDown�� true�� * 0.3���� �̼� ����

        animator.SetBool("isRun", moveVec != Vector3.zero);
        animator.SetBool("isWalk", LSDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);//�÷��̾ �ٶ󺸴� ���� �ٲٱ�
    }

    void Jump()
    {
        if (spaceDown && !isJump && moveVec == Vector3.zero && !isDodge)//Dodge���� �ƴϰ�, �������� ���� ���� Jump�� ����
        {
            rb.AddForce(Vector3.up * 15f, ForceMode.Impulse);
            isJump = true;
            animator.SetBool("isJump", true);
            animator.SetTrigger("doJump");
        }
    }

    void Dodge()
    {
        if (spaceDown && !isDodge && !isJump && moveVec != Vector3.zero)//�����̰� ���� ���� Jump��� Dodge�� ����
        {
            dodgeVec = moveVec;
            moveSpeed *= 2f;//��� �� �ӵ� ����
            animator.SetTrigger("doDodge");
            isDodge = true;
            Invoke("DodgeOut", 0.5f);
        }
    }
    void DodgeOut()
    {
        moveSpeed *= 0.5f;//���ӵ� �ӵ� �ٽ� �������
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
