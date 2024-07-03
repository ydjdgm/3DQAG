using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range};
    public Type type;
    public float damage;
    public float rate;
    public int maxAmmo;
    public int curAmmo;

    public BoxCollider meleeArea;
    public TrailRenderer trailRenderer;
    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletcasePos;
    public GameObject bulletcase;

    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine(Swing());
            StartCoroutine(Swing());
        }else if (type == Type.Range && curAmmo > 0)
        {
            curAmmo--;
            StopCoroutine(Shot());
            StartCoroutine(Shot());
        }
    }

    IEnumerator Swing()
    {
        yield return null;
        meleeArea.enabled = true;
        trailRenderer.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailRenderer.enabled = false;
        yield break;
    }

    IEnumerator Shot()
    {
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRb = instantBullet.GetComponent<Rigidbody>();
        bulletRb.velocity = bulletPos.forward * 50;

        yield return null;
        GameObject instantCase = Instantiate(bulletcase, bulletcasePos.position, bulletcasePos.rotation);
        Rigidbody caseRb = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletcasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRb.AddForce(caseVec, ForceMode.Impulse);
        caseRb.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }
}
