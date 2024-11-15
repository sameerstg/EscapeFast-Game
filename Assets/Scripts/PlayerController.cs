using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public InputSystem input;

    private Rigidbody rb;
    public float jumpDelta = 2.0f;          // Height of the jump
    public float laneDelta = 2.0f;          // Horizontal distance between lanes
    public float forwardSpeed = 5.0f;       // Initial forward speed
    public float acceleration = 0.1f;       // Rate at which forward speed increases
    private bool isMoving = false;

    public PlayerState playerPosition = PlayerState.Middle;
    public event Action<PlayerState> OnPlayerStateChange;  // Event to notify about state changes


    private void Awake()
    {
        input = new();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        input.Enable();
        input.ActionMap.Swipe.performed += OnSwipe;
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Update()
    {
        forwardSpeed += acceleration * Time.deltaTime;
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
    }

    void ChangePlayerState(PlayerState newState)
    {
        playerPosition = newState;
        OnPlayerStateChange?.Invoke(playerPosition);  // Trigger event
    }

    private void OnSwipe(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (isMoving) return;

        Vector2 swipeValue = context.ReadValue<Vector2>();
        Vector2 direction = GetDirection(swipeValue);
        Move(direction);
    }

    void Move(Vector2 direction)
    {
        if (direction == Vector2.up)
        {
            ChangePlayerState(PlayerState.Up);
            Jump();
        }
        else if (direction == Vector2.down)
        {
            ChangePlayerState(PlayerState.Down);
        }
        else if (direction == Vector2.left || direction == Vector2.right)
        {
            SwitchLane(direction);
        }
    }

    void Jump()
    {
        if (!isMoving)
        {
            isMoving = true;
            float startY = transform.position.y;
            LeanTween.moveY(gameObject, startY + jumpDelta, 0.3f).setOnComplete(() =>
            {
                LeanTween.moveY(gameObject, startY, 0.3f).setOnComplete(() =>
                {
                    ChangePlayerState(PlayerState.Middle);
                    isMoving = false;
                });
            });
        }
    }

    void SwitchLane(Vector2 direction)
    {
        if (direction == Vector2.left && playerPosition != PlayerState.Left)
        {
            MoveToLane(playerPosition == PlayerState.Middle ? PlayerState.Left : PlayerState.Middle);
        }
        else if (direction == Vector2.right && playerPosition != PlayerState.Right)
        {
            MoveToLane(playerPosition == PlayerState.Middle ? PlayerState.Right : PlayerState.Middle);
        }
    }

    void MoveToLane(PlayerState targetLane)
    {
        isMoving = true;
        float targetX = 0;

        if (targetLane == PlayerState.Left)
        {
            targetX = -laneDelta;
            ChangePlayerState(PlayerState.Left);
        }
        else if (targetLane == PlayerState.Right)
        {
            targetX = laneDelta;
            ChangePlayerState(PlayerState.Right);
        }
        else
        {
            targetX = 0;
            ChangePlayerState(PlayerState.Middle);
        }

        LeanTween.moveX(gameObject, targetX, 0.3f).setOnComplete(() =>
        {
            isMoving = false;
        });
    }

    private Vector2 GetDirection(Vector2 swipeValue)
    {
        if (Mathf.Abs(swipeValue.x) > Mathf.Abs(swipeValue.y))
        {
            return swipeValue.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            return swipeValue.y > 0 ? Vector2.up : Vector2.down;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlatTrigger"))
        {
            PlatformGenerator._instance.PlatformPool(other.gameObject);
        }
    }
}

    public enum PlayerState
    {
        Left,
        Middle,
        Right,
        Up,
        Down
    }
