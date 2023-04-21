using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{
    public ParticleSystem ripple;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        
    }

    private void CreateRipple(int start, int end, int delta, float speed, float size, float lifeTime)
    {
        Vector3 forward = ripple.transform.eulerAngles;
        forward.y = start;
        ripple.transform.eulerAngles = forward;

        for (int i = start; i < end; i+=delta)
        {
            ripple.Emit(transform.position + ripple.transform.forward * 0.5f, ripple.transform.forward * speed, size, lifeTime, Color.white);
            ripple.transform.eulerAngles += Vector3.up * 3;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 4 && rb.velocity.magnitude > 0.03f) 
        {
            CreateRipple(-180, 180, 3, 2, 2, 2);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 4 && rb.velocity.magnitude > 0.02f && Time.renderedFrameCount % 5 == 0)
        {
            int y = (int)transform.eulerAngles.y;
            CreateRipple(y-90, y+90, 3, 5, 2, 1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 4 && rb.velocity.magnitude > 0.03f)
        {
            CreateRipple(-180, 180, 3, 2, 2, 2);
        }
    }
}
