using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissle : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float pushForce = 5f;

    [SerializeField] private GameObject explosionEffect;

    private FireballBoss bossParent;

    private Transform target;

    private Rigidbody2D rb;

    private void Start()
    {
        bossParent = FindObjectOfType<FireballBoss>();
        target = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        Vector2 direction = (Vector2)target.position - rb.position;

        direction.Normalize();

        float rotateAmount = Vector3.Cross(direction, transform.up).z;

        rb.angularVelocity = -rotateAmount * rotationSpeed;

        rb.velocity = transform.up * speed;
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.name == "Player")
        {

            Damage dmg = new Damage()
            {
                damageAmount = damage,
                origin = transform.position,
                pushForce = pushForce
            };

            col.SendMessage("ReceiveDamage", dmg);

            Instantiate(explosionEffect, transform.position, transform.rotation);
            bossParent.SetLastMissleTime();
            //bossParent.SetActiveMissleExists(false);
            Destroy(gameObject);
        }
    }
}
