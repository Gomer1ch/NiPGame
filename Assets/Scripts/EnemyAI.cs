
using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public Transform throwOrigin;
    public GameObject knifePrefab;

    public float moveSpeed = 3f;
    public float throwInterval = 3f;
    public float throwSpeed = 10f;
    public float gravity = 9.81f;

    [Header("Movement Zone")]
    //public Vector2 moveXRange = new Vector2(-4f, 4f);
    //public Vector2 moveZRange = new Vector2(1f, 5f);

    public Vector2 directionChangeDelayRange = new Vector2(1f, 3f);

    private Vector3 moveTarget;
    private float directionChangeTimer;
    private float throwTimer;
    private Animator animator;
    private MovementLimiter limiter;


    void Start()
    {
        limiter = GetComponent<MovementLimiter>();
        directionChangeTimer = UnityEngine.Random.Range(directionChangeDelayRange.x, directionChangeDelayRange.y);
        moveTarget = transform.position;
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        float moveAmount = (moveTarget - transform.position).magnitude;
        if (animator != null)
        {
            animator.SetFloat("Speed", moveAmount > 0.05f ? 1f : 0f);
        }

        // Перерасчёт цели
        directionChangeTimer -= Time.deltaTime;
        if (directionChangeTimer <= 0f)
        {
            float newX = UnityEngine.Random.Range(limiter.minX, limiter.maxX);
            float newZ = UnityEngine.Random.Range(limiter.minZ, limiter.maxZ);
            moveTarget = new Vector3(newX, transform.position.y, newZ);
            directionChangeTimer = UnityEngine.Random.Range(directionChangeDelayRange.x, directionChangeDelayRange.y);
        }

        // Движение
        transform.position = Vector3.MoveTowards(transform.position, moveTarget, moveSpeed * Time.deltaTime);

        // поворот на игрока
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.1f)
            transform.forward = lookDir.normalized;

        // таймер броска
        throwTimer -= Time.deltaTime;
        if (throwTimer <= 0f)
        {
            ThrowKnifeAtPlayer();
            throwTimer = throwInterval;
        }
    }

    void ThrowKnifeAtPlayer()
    {
        if (player == null || knifePrefab == null) return;

        if (animator != null)
        {
            animator.SetTrigger("Throw");
        }

        float flightTime = 1.2f;
        Vector3 velocity = CalculateThrowVelocity(throwOrigin.position, player.position, flightTime);

        GameObject knife = Instantiate(knifePrefab, throwOrigin.position, Quaternion.LookRotation(velocity));
        Rigidbody rb = knife.GetComponent<Rigidbody>();
        rb.linearVelocity = velocity;

    }

    Vector3 CalculateThrowVelocity(Vector3 start, Vector3 target, float flightTime)
    {
        Vector3 toTarget = target - start;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0f, toTarget.z);
        float y = toTarget.y;
        float xz = toTargetXZ.magnitude;

        float vy = y / flightTime + 0.5f * gravity * flightTime;
        float vxz = xz / flightTime;

        Vector3 result = toTargetXZ.normalized * vxz;
        result.y = vy;
        return result;
    }
}
