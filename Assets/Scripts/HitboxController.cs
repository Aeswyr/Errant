using System;
using UnityEngine;

public class HitboxController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rbody;
    [SerializeField] private float destroyAfterDelay = 300;
    [SerializeField] private Vector2 velocity;
    [SerializeField] private bool destroyOnHit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        destroyAfterDelay += Time.time;
        rbody.linearVelocity = velocity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time > destroyAfterDelay)
            Destroy(gameObject);
    }

    public void OnHit()
    {
        if (destroyOnHit)
            Destroy(gameObject);
    }
}
