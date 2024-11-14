using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public InputSystem input;

    private Rigidbody rb;
    public float jumpDelta = 2.0f;          // Height of the jump
    public float laneDelta = 2.0f;          // Horizontal distance between lanes
    public float forwardSpeed = 5.0f;       // Initial forward speed
    public float acceleration = 0.1f;       // Rate at which forward speed increases
    private bool isMoving = false;

    public PlayerPosition playerPosition = PlayerPosition.Middle;
    public enum PlayerPosition
    {
        Left,
        Middle,
        Right
    }

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
        // Gradually increase the forward speed over time for natural acceleration
        forwardSpeed += acceleration * Time.deltaTime;

        // Move the player forward based on the current speed
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
    }

    private void OnSwipe(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (isMoving) return;  // Ignore input if already moving

        Vector2 swipeValue = context.ReadValue<Vector2>();
        Vector2 direction = GetDirection(swipeValue);
        Move(direction);
    }

    void Move(Vector2 direction)
    {
        if (direction == Vector2.up)
        {
            Jump();
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
                    isMoving = false;
                });
            });
        }
    }

    void SwitchLane(Vector2 direction)
    {
        if (direction == Vector2.left && playerPosition != PlayerPosition.Left)
        {
            // Move left only if not already at the leftmost lane
            MoveToLane(playerPosition == PlayerPosition.Middle ? PlayerPosition.Left : PlayerPosition.Middle);
        }
        else if (direction == Vector2.right && playerPosition != PlayerPosition.Right)
        {
            // Move right only if not already at the rightmost lane
            MoveToLane(playerPosition == PlayerPosition.Middle ? PlayerPosition.Right : PlayerPosition.Middle);
        }
    }

    void MoveToLane(PlayerPosition targetLane)
    {
        isMoving = true;
        float targetX = 0;

        if (targetLane == PlayerPosition.Left)
        {
            targetX = -laneDelta;
            playerPosition = PlayerPosition.Left;
        }
        else if (targetLane == PlayerPosition.Right)
        {
            targetX = laneDelta;
            playerPosition = PlayerPosition.Right;
        }
        else
        {
            targetX = 0;  // Middle lane
            playerPosition = PlayerPosition.Middle;
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
