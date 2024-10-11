using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour
{
    private Transform targetTransform;
    public Vector3 rotation;

    void Start()
    {
        targetTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        targetTransform.rotation = Quaternion.Euler(rotation);
    }
}
