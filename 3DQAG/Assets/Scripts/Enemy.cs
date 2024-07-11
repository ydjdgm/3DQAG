using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHp;
    public float curHp;

    Rigidbody rb;
    BoxCollider BoxCol;
    Material mat;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        BoxCol = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHp -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec, false));

            Debug.Log("Melee: "+curHp);
        }else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHp -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec, false));

            Debug.Log("Range: " + curHp);
        }
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        curHp -= 100;
        Vector3 reactvec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactvec, true));
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(curHp > 0)
        {
            mat.color = Color.white;
        }
        else
        {
            mat.color = Color.gray;
            gameObject.layer = 14;
            gameObject.tag = "DeadEnemy";

            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;

                rb.freezeRotation = false;
                rb.AddForce(reactVec * 5, ForceMode.Impulse);
                rb.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rb.AddForce(reactVec * 5, ForceMode.Impulse);
            }


            Destroy(gameObject, 1f);
        }
    }
}
