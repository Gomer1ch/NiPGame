using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using Unity.Cinemachine;
using System.Collections;

public class KnifeThrowController : MonoBehaviour, IGameControl
{
    [Header("Input")]
    public InputActionAsset inputActions;
    private InputAction swipeAction;

    [Header("Cameras")]
    public CinemachineCamera playerCamera;
    public CinemachineCamera aimCamera;

    [Header("Throw Settings")]
    public Transform throwOrigin;
    public LineRenderer trajectoryLine;
    public float throwSpeed = 10f;
    public float gravity = 9.81f;
    public int trajectoryPoints = 30;
    public float timeStep = 0.05f;
    public float aimTurnSpeed = 1.5f;
    public float minSwipe = 50f;
    public float maxSwipe = 500f;
    public float returnSpeed = 5f;
    public Animator playerAnimator;
    public Transform visual;
    public GameObject knifePrefab;

    private Vector2 startTouch;
    private Vector2 endTouch;
    private bool swipeStarted = false;
    private Quaternion initialVisualLocalRotation;
    private Quaternion originalPlayerRotation;
    private Coroutine rotationResetCoroutine;
    private bool isResetPending = false;
    private bool isControlEnabled;

    public void SetControlEnabled(bool state)
    {
        isControlEnabled = state;
    }

    void Awake()
    {
        originalPlayerRotation = transform.rotation;
        initialVisualLocalRotation = visual.localRotation;

        if (inputActions != null)
        {
            var map = inputActions.FindActionMap("UI");
            swipeAction = map.FindAction("Point");
            inputActions.Enable();
        }
        // Включаем EnhancedTouch для поддержки мультикасания
        EnhancedTouchSupport.Enable();
    }

    void Start()
    {
        playerAnimator.SetLayerWeight(1, 0f);
    }

    void OnDisable()
    {
        inputActions.Disable();
        EnhancedTouchSupport.Disable();
    }

    void LateUpdate()
    {
        if (isResetPending)
        {
            ResetPlayerOrientation();
            isResetPending = false;
        }

        visual.localRotation = initialVisualLocalRotation * Quaternion.Inverse(originalPlayerRotation) * transform.rotation;
    }

    void Update()
    {
        if (!isControlEnabled || swipeAction == null) return;

#if UNITY_EDITOR
        HandleEditorInput();
#else
        HandleEnhancedTouchInput();
#endif
    }

    // Режим для редактора (мышь)
    void HandleEditorInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && Mouse.current.position.x.ReadValue() > Screen.width / 2)
        {
            startTouch = Mouse.current.position.ReadValue();
            swipeStarted = true;
            EnableAimCamera(true);
            StopIfRunning();
        }

        if (Mouse.current.leftButton.isPressed && swipeStarted)
        {
            float deltaX = Mouse.current.position.x.ReadValue() - startTouch.x;
            transform.Rotate(Vector3.up, deltaX * Time.deltaTime * aimTurnSpeed);
            startTouch.x = Mouse.current.position.x.ReadValue();

            endTouch = Mouse.current.position.ReadValue();
            DrawTrajectory(startTouch - endTouch);
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && swipeStarted)
        {
            swipeStarted = false;
            EnableAimCamera(false);
            trajectoryLine.positionCount = 0;
            Vector2 swipe = startTouch - endTouch;
            ThrowKnife(swipe);
        }
    }

    // Обработка мультикасания с помощью EnhancedTouch
    void HandleEnhancedTouchInput()
    {
        if (Touch.activeTouches.Count == 0) return;

        foreach (var touch in Touch.activeTouches)
        {
            // Рассматриваем касания справа от центра экрана
            if (touch.screenPosition.x > Screen.width / 2f)
            {
                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    startTouch = touch.screenPosition;
                    swipeStarted = true;
                    EnableAimCamera(true);
                    StopIfRunning();
                }
                else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved && swipeStarted)
                {
                    float deltaX = touch.screenPosition.x - startTouch.x;
                    transform.Rotate(Vector3.up, deltaX * Time.deltaTime * aimTurnSpeed);
                    startTouch.x = touch.screenPosition.x;

                    endTouch = touch.screenPosition;
                    DrawTrajectory(startTouch - endTouch);
                }
                else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended && swipeStarted)
                {
                    endTouch = touch.screenPosition;
                    swipeStarted = false;
                    EnableAimCamera(false);
                    trajectoryLine.positionCount = 0;

                    Vector2 swipe = startTouch - endTouch;
                    ThrowKnife(swipe);
                }
            }
        }
    }

    void StopIfRunning()
    {
        if (rotationResetCoroutine != null)
            StopCoroutine(rotationResetCoroutine);
    }

    void EnableAimCamera(bool state)
    {
        if (aimCamera != null)
            aimCamera.Priority = state ? 20 : 5;
    }

    void ResetPlayerOrientation()
    {
        transform.rotation = originalPlayerRotation;
        visual.localRotation = initialVisualLocalRotation;
    }

    void DrawTrajectory(Vector2 swipe)
    {
        float swipeStrength = Mathf.Clamp(swipe.magnitude, minSwipe, maxSwipe);
        float swipeVerticalFactor = Mathf.InverseLerp(minSwipe, maxSwipe, swipeStrength);

        Vector3 forward = visual.forward;
        Vector3 up = Vector3.up;
        Vector3 dir = Vector3.Lerp(forward, (forward + up).normalized, swipeVerticalFactor);
        Vector3 velocity = dir * throwSpeed;

        Vector3[] points = new Vector3[trajectoryPoints];
        Vector3 currentPosition = throwOrigin.position;
        Vector3 currentVelocity = velocity;

        for (int i = 0; i < trajectoryPoints; i++)
        {
            points[i] = currentPosition;
            currentVelocity += Vector3.down * gravity * timeStep;
            currentPosition += currentVelocity * timeStep;
        }

        trajectoryLine.positionCount = trajectoryPoints;
        trajectoryLine.SetPositions(points);
    }

    void ThrowKnife(Vector2 swipe)
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetLayerWeight(1, 1f);
            playerAnimator.SetTrigger("Throw");
            StartCoroutine(ResetThrowLayer());
        }

        float swipeStrength = Mathf.Clamp(swipe.magnitude, minSwipe, maxSwipe);
        float swipeVerticalFactor = Mathf.InverseLerp(minSwipe, maxSwipe, swipeStrength);

        Vector3 forward = visual.forward;
        Vector3 up = Vector3.up;
        Vector3 dir = Vector3.Lerp(forward, (forward + up).normalized, swipeVerticalFactor);
        Vector3 velocity = dir * throwSpeed;

        GameObject knife = Instantiate(knifePrefab, throwOrigin.position, Quaternion.LookRotation(dir));
        Rigidbody rb = knife.GetComponent<Rigidbody>();
        rb.linearVelocity = velocity;
    }

    IEnumerator ResetThrowLayer()
    {
        yield return new WaitForSeconds(0.6f);
        playerAnimator.SetLayerWeight(1, 0f);
        isResetPending = true;
    }
}
