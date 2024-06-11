using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed;//플레이어 기본 이동속도
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;

    public int ammo;
    public int coin;
    public int health;
    public int hasGrenades;

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    float hAxis;
    float vAxis;

    bool LSDown;//left shift
    bool spaceDown;
    bool eDown;
    bool down1;
    bool down2;
    bool down3;

    bool isJump;
    bool isDodge;
    bool isSwap;

    Vector3 moveVec;//플레이어 이동 백터
    Vector3 dodgeVec;//Dodge시 백터

    Rigidbody rb;
    Animator animator;

    GameObject nearObject;
    GameObject equipWeapon;
    int weaponIndex = -1;

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
        Interaction();
        Swap();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        LSDown = Input.GetButton("Walk");
        spaceDown = Input.GetButtonDown("Jump");
        eDown = Input.GetButtonDown("Interaction");
        down1 = Input.GetButtonDown("Swap1");
        down2 = Input.GetButtonDown("Swap2");
        down3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if(isDodge)
        {
            moveVec = dodgeVec;
        }

        transform.position += moveVec * moveSpeed * (LSDown ? 0.3f : 1f) * Time.deltaTime;//LSDown (걷기) 이 true면 * 0.3으로 이속 감소

        animator.SetBool("isRun", moveVec != Vector3.zero);
        animator.SetBool("isWalk", LSDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);//플레이어가 바라보는 방향 바꾸기
    }

    void Jump()
    {
        if (spaceDown && !isJump && !isSwap && moveVec == Vector3.zero && !isDodge)//Dodge중이 아니고, 움직이지 않을 때만 Jump가 나감
        {
            rb.AddForce(Vector3.up * 15f, ForceMode.Impulse);
            isJump = true;
            animator.SetBool("isJump", true);
            animator.SetTrigger("doJump");
        }
    }

    void Dodge()
    {
        if (spaceDown && !isDodge && !isJump && !isSwap && moveVec != Vector3.zero)//움직이고 있을 때만 Jump대신 Dodge가 나감
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

    void Interaction()
    {
        if (eDown && nearObject != null)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }

    void Swap()
    {
        if (down1)
        {
            if (weaponIndex == 0)
            {
                return;
            }
            weaponIndex = 0;
        }else if (down2)
        {
            if (weaponIndex == 1)
            {
                return;
            }
            weaponIndex = 1;
        }else if (down3)
        {
            if (weaponIndex == 2)
            {
                return;
            }
            weaponIndex = 2;
        }

        if ((down1 || down2 || down3) && hasWeapons[weaponIndex] && !isJump && !isDodge)
        {
            if (equipWeapon != null)
            {
                equipWeapon.SetActive(false);
            }
            equipWeapon = weapons[weaponIndex];
            equipWeapon.SetActive(true);

            animator.SetTrigger("doSwap");
            isSwap = true;
            Invoke("SwapOut", 0.3f);
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isJump = false;
            animator.SetBool("isJump", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch(item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if(ammo > maxAmmo)
                    {
                        ammo = maxAmmo;
                    }
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                    {
                        coin = maxCoin;
                    }
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                    {
                        health = maxHealth;
                    }
                    break;
                case Item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades)
                    {
                        hasGrenades = maxHasGrenades;
                    }
                    break;
            }
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = null;
        }
    }
}
