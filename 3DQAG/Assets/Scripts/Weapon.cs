using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range};
    public Type type;
    public float damage;
    public float rate;
    public BoxCollider meleeArea;
    public TrailRenderer trailRenderer;

    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine(Swing());
            StartCoroutine(Swing());
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
}
