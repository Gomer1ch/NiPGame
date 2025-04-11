using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ZoneOutline : MonoBehaviour
{
    public float minX = -5f, maxX = 5f;
    public float minZ = -5f, maxZ = 0f;
    public float yOffset = 0.05f;
    public float width = 0.05f;

    public Color baseColor = Color.red;
    public float pulseSpeed = 3f;
    public float pulseMin = 0.5f;
    public float pulseMax = 1.0f;

    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();

        lr.positionCount = 5;
        lr.loop = true;
        lr.useWorldSpace = true;
        lr.startWidth = width;
        lr.endWidth = width;
        lr.material = new Material(Shader.Find("Sprites/Default"));

        Vector3[] points = new Vector3[5];
        points[0] = new Vector3(minX, yOffset, minZ);
        points[1] = new Vector3(minX, yOffset, maxZ);
        points[2] = new Vector3(maxX, yOffset, maxZ);
        points[3] = new Vector3(maxX, yOffset, minZ);
        points[4] = points[0];

        lr.SetPositions(points);
    }

    void Update()
    {
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * ((pulseMax - pulseMin) / 2f) + ((pulseMax + pulseMin) / 2f);
        Color pulseColor = baseColor * pulse;
        pulseColor.a = pulse; // прозрачность тоже мерцает

        lr.startColor = pulseColor;
        lr.endColor = pulseColor;
    }
}
