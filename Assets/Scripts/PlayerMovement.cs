using UnityEngine;

public class PlayerMovement : MonoBehaviour, IGameControl
{
    public float moveSpeed = 6f;
    public float moveAcceleration = 1f;
    public FixedJoystick joystick;
    private CharacterController controller;
    private Animator animator;

    private Vector3 velocity; // ������ ������������ ��������
    public float gravity = -9.81f;

    private bool isControlEnabled;

    public void SetControlEnabled(bool state)
    {
        isControlEnabled = state;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void ApplyGravity()
    {
        if (controller.isGrounded)
            velocity.y = -1f; // ���������� �������� � �����
        else
            velocity.y += gravity * Time.deltaTime; // ����������� �������
    }

    void Update()
    {
        if (!isControlEnabled) return;

        ApplyGravity(); // �������� �����������

        Vector2 input = new Vector2(-joystick.Horizontal, -joystick.Vertical); // �������� ���������
        //float magnitude = input.magnitude;
        Vector2 curvedInput = input * Mathf.Pow(input.magnitude, moveAcceleration); // ���������� ���������

        Vector3 direction = new Vector3(curvedInput.x, 0, curvedInput.y);
        float speed = direction.magnitude;

        // ���������� ������ ������������
        //if (speed < 0.05f) speed = 0f;
        animator.SetFloat("Speed", speed);

        Vector3 move = direction * moveSpeed;

        // ��������� ������������ ��������
        move.y = velocity.y;

        controller.Move(move * Time.deltaTime);
    }
}
