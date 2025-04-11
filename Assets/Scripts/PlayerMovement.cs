using UnityEngine;

public class PlayerMovement : MonoBehaviour, IGameControl
{
    public float moveSpeed = 6f;
    public float moveAcceleration = 1f;
    public FixedJoystick joystick;
    private CharacterController controller;
    private Animator animator;

    private Vector3 velocity; // хранит вертикальную скорость
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
            velocity.y = -1f; // удерживаем прижатие к земле
        else
            velocity.y += gravity * Time.deltaTime; // накапливаем падение
    }

    void Update()
    {
        if (!isControlEnabled) return;

        ApplyGravity(); // вызываем обязательно

        Vector2 input = new Vector2(-joystick.Horizontal, -joystick.Vertical); // инверсия сохранена
        //float magnitude = input.magnitude;
        Vector2 curvedInput = input * Mathf.Pow(input.magnitude, moveAcceleration); // сглаживаем поведение

        Vector3 direction = new Vector3(curvedInput.x, 0, curvedInput.y);
        float speed = direction.magnitude;

        // Фильтрация ложных срабатываний
        //if (speed < 0.05f) speed = 0f;
        animator.SetFloat("Speed", speed);

        Vector3 move = direction * moveSpeed;

        // Добавляем вертикальное движение
        move.y = velocity.y;

        controller.Move(move * Time.deltaTime);
    }
}
