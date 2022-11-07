using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterBullet : MonoBehaviour
{
    public Rigidbody2D rb;
    public Collider2D _collider;
    public float speed = 10f;

    ShooterPlayer owner;

    public void Set(ShooterPlayer owner, Vector3 position, Vector3 direction, float lag)
    {
        this.owner = owner;
        Physics2D.IgnoreCollision(_collider, owner.GetComponent<Collider2D>());

        rb.position = position;
        rb.velocity = direction * speed;
        rb.position = rb.position + rb.velocity * lag;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<ShooterPlayer>())
            {
                owner.RestoreHealth();
            }
        }
        Destroy(gameObject);
    }
}
