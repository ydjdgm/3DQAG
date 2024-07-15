using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool isMelee;
    public bool isRock;
    public int damage;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor" && !isMelee && !isRock)
        {
            Destroy(gameObject, 3f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Enemy" && !isMelee)
        {
            Destroy(gameObject);
        }
    }
}
