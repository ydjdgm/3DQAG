using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed;//플레이어 기본 이동속도
    float hAxis;
    float vAxis;
    bool LSDown;//left shift

    Vector3 moveVec;//플레이어 이동 Vector3

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

        transform.position += moveVec * moveSpeed * (LSDown ? 0.3f : 1f) * Time.deltaTime;//LSDown이 true면 * 0.3으로 이속 감소

        animator.SetBool("isRun", moveVec != Vector3.zero);
        animator.SetBool("isWalk", LSDown);

        transform.LookAt(transform.position + moveVec);//플레이어가 바라보는 방향 바꾸기
    }

}
