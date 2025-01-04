using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 5.0f;
    public float rotationSpeed = 100.0f;

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, Time.fixedDeltaTime * rotationSpeed);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);
        }

        // if (translation != 0)
        // {
        //     anim.SetBool("isWalking", true);
        //     anim.SetFloat("characterSpeed", translation);
        // }
        // else
        // {
        //     anim.SetBool("isWalking", false);
        //     anim.SetFloat("characterSpeed", 0);
        // }
    }
}
