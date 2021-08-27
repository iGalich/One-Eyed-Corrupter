using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMotor : MonoBehaviour
{
    [SerializeField] private Transform lookAt;

    public float boundX = 0.15f;
    public float boundY = 0.05f;

    private void Start()
    {
        lookAt = GameObject.Find("Player").transform;
    }


    private void LateUpdate()
    {
        Vector3 delta = Vector3.zero;

        //check if inside bounds on X axis
        float deltaX = lookAt.position.x - transform.position.x;
        if (deltaX > boundX || deltaX < -boundX)
        {
            if (transform.position.x < lookAt.position.x)
            {
                delta.x = deltaX - boundX;
            }
            else
            {
                delta.x = deltaX + boundX;
            }
        }

        //check if inside bounds on Y axis
        float deltaY = lookAt.position.y - transform.position.y;
        if (deltaY > boundY || deltaY < -boundY)
        {
            if (transform.position.y < lookAt.position.y)
            {
                delta.y = deltaY - boundY;
            }
            else
            {
                delta.y = deltaY + boundY;
            }
        }

        transform.position += new Vector3(delta.x, delta.y, 0);
        lookAt = GameObject.Find("Player").transform;
    }
}
