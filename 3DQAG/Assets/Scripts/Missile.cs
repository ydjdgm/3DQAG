using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(Vector3.forward * 30 * Time.deltaTime);
    }
}