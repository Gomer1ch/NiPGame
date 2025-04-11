using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KnifeProjectile : MonoBehaviour
{
    private Rigidbody rb;
    private bool hasHit = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!hasHit && rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            Quaternion look = Quaternion.LookRotation(rb.linearVelocity.normalized);
            transform.rotation = look;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;
        hasHit = true;

        string tag = collision.gameObject.tag;

        if (tag == "Enemy" || tag == "Player")
        {
            collision.gameObject.SendMessage("Die", SendMessageOptions.DontRequireReceiver);
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
            transform.parent = collision.transform;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
