using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offSet;

    private void Update()
    {
        transform.position = target.position + offSet;
    }
}
