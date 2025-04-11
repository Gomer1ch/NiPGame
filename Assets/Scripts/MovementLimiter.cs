using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementLimiter : MonoBehaviour
{
    public float minX = -5f;
    public float maxX = 5f;
    public float minZ = -5f;
    public float maxZ = 0f;

    public bool drawGizmos = true;
    public Color gizmoColor = new Color(1f, 0f, 0f, 0.2f); // Красный, прозрачный

    private CharacterController controller;
    private Vector3 lastValidPosition;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        lastValidPosition = transform.position;
    }

    void LateUpdate()
    {
        Vector3 clamped = transform.position;
        clamped.x = Mathf.Clamp(clamped.x, minX, maxX);
        clamped.z = Mathf.Clamp(clamped.z, minZ, maxZ);
        clamped.y = transform.position.y; // не ограничиваем высоту

        if (clamped != transform.position)
        {
            // вернём персонажа в зону, если он вышел
            controller.enabled = false;
            transform.position = clamped;
            controller.enabled = true;
        }

        lastValidPosition = clamped;
    }

    void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        Gizmos.color = gizmoColor;
        Vector3 center = new Vector3((minX + maxX) / 2f, transform.position.y, (minZ + maxZ) / 2f);
        Vector3 size = new Vector3(maxX - minX, 0.1f, maxZ - minZ);
        Gizmos.DrawCube(center, size);
    }
}
