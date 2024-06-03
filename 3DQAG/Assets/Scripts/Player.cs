using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed;//�÷��̾� �⺻ �̵��ӵ�
    float hAxis;
    float vAxis;
    bool LSDown;//left shift

    Vector3 moveVec;//�÷��̾� �̵� Vector3

    Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        LSDown = Input.GetButton("Walk");

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        transform.position += moveVec * moveSpeed * (LSDown ? 0.3f : 1f) * Time.deltaTime;//LSDown�� true�� * 0.3���� �̼� ����

        animator.SetBool("isRun", moveVec != Vector3.zero);
        animator.SetBool("isWalk", LSDown);

        transform.LookAt(transform.position + moveVec);//�÷��̾ �ٶ󺸴� ���� �ٲٱ�
    }

}
