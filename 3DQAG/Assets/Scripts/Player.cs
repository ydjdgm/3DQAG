using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed;//�÷��̾� �⺻ �̵��ӵ�
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public Camera followCam;

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
    bool mouseLeftDown;
    bool rDown;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isFireReady = true;
    bool isReload;

    Vector3 moveVec;//�÷��̾� �̵� ����
    Vector3 dodgeVec;//Dodge�� ����

    Rigidbody rb;
    Animator animator;

    GameObject nearObject;
    Weapon equipWeapon;
    int weaponIndex = -1;
    float fireDelay;

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
        Attack();
        Reload();
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
        mouseLeftDown = Input.GetButton("Fire1");
        rDown = Input.GetButtonDown("Reload");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if(isDodge)
        {
            moveVec = dodgeVec;
        }

        transform.position += moveVec * moveSpeed * (isSwap || isReload ? 0.5f : 1f) * (LSDown ? 0.3f : 1f) * Time.deltaTime;//LSDown (�ȱ�) �� true�� * 0.3���� �̼� ����, reload �Ǵ� swap�� �̼� *0.5

        animator.SetBool("isRun", moveVec != Vector3.zero);
        animator.SetBool("isWalk", LSDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);//�÷��̾ �ٶ󺸴� ���� �ٲٱ�

        if (mouseLeftDown)
        {
            Ray ray = followCam.ScreenPointToRay(Input.mousePosition);//���콺�� ���� �ٲٱ�
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0f;
                transform.LookAt(transform.position + nextVec);
            }
        }
    }

    void Jump()
    {
        if (spaceDown && !isJump && !isSwap && moveVec == Vector3.zero && !isDodge)//Dodge���� �ƴϰ�, �������� ���� ���� Jump�� ����
        {
            rb.AddForce(Vector3.up * 15f, ForceMode.Impulse);
            isJump = true;
            animator.SetBool("isJump", true);
            animator.SetTrigger("doJump");
        }
    }

    void Attack()
    {
        if ( equipWeapon == null)
        {
            return;
        }
        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if (mouseLeftDown && isFireReady && !isDodge && !isJump && !isSwap)
        {
            equipWeapon.Use();
            animator.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }

    void Reload()
    {
        if (equipWeapon == null || equipWeapon.type == Weapon.Type.Melee || ammo == 0)
        {
            return;
        }

        if (rDown && !isJump && !isDodge && !isSwap && isFireReady)
        {
            animator.SetTrigger("doReload");
            isReload = true;
            Invoke("ReloadOut", 1.5f);
        }
    }
    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
    }

    void Dodge()
    {
        if (spaceDown && !isDodge && !isJump && !isSwap && moveVec != Vector3.zero)//�����̰� ���� ���� Jump��� Dodge�� ����
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
                equipWeapon.gameObject.SetActive(false);
            }
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

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
