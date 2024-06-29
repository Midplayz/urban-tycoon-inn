using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 720.0f;
    public SimpleTouchController joystickController;

    private Rigidbody rb;
    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector2 joystickInput = joystickController.GetTouchPosition;

        Vector3 movement = new Vector3(-moveHorizontal - joystickInput.x, 0.0f, -moveVertical - joystickInput.y).normalized;

        bool isMoving = movement.magnitude > 0;
        animator.SetBool("isWalking", isMoving);

        if (isMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            rb.angularVelocity = Vector3.zero;
        }

        rb.velocity = movement * moveSpeed;
    }
}
