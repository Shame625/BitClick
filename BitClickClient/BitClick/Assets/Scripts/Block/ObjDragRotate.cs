using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjDragRotate : MonoBehaviour
{
    public float rotSpeed = 20.0f;

    private void OnMouseDrag()
    {
        if (GameManager.inGame && Input.GetMouseButton(0))
        {
            float rotX = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;
            float rotY = Input.GetAxis("Mouse Y") * rotSpeed * Mathf.Deg2Rad;

            transform.RotateAround(Vector3.up, -rotX);
            transform.RotateAround(Vector3.right, rotY);
        }
    }
}
