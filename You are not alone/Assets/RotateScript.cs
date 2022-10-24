using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateScript : MonoBehaviour
{
    public float speed = 1f;

    // Update is called once per frame
    void Update()
    {
        //Rotate this object around the global Y axis at 1 degree per second
        transform.Rotate(Vector3.up * (Time.deltaTime * speed));
    }
}
