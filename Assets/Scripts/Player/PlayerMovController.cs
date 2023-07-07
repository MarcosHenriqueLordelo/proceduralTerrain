using UnityEngine;

public class PlayerMovController : MonoBehaviour {
    // Movement variables
    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    private Vector3 moveDirection = Vector3.zero;

    // References
    private CharacterController controller;
    private Animator animator;

    // Start is called before the first frame update
    void Start() {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update() {
        // Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool jump = Input.GetButtonDown("Jump");

        // Calculate movement direction
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        Vector3 desiredMoveDirection = forward * vertical + right * horizontal;

        // Apply movement
        if (controller.isGrounded) {
            moveDirection = desiredMoveDirection * speed;
            if (jump) {
                moveDirection.y = jumpSpeed;
            }
        } else {
            moveDirection.x = desiredMoveDirection.x * speed;
            moveDirection.z = desiredMoveDirection.z * speed;
        }

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        controller.Move(moveDirection * Time.deltaTime);

        if (horizontal != 0 || vertical != 0) {
            Quaternion targetRotation = Quaternion.LookRotation(forward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 360.0f);
        }
    }
}
