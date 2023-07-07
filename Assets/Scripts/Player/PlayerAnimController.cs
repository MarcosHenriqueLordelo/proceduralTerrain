using UnityEngine;

public class PlayerAnimController : MonoBehaviour {

    private Animator animator;

    private CharacterController controller;

    public float speed = 10.0f;
    private float velocityX;
    private float velocityY;

    void Start() {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void Update() {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        bool jump = Input.GetButtonDown("Jump");
        bool isGrounded = controller.isGrounded;

        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput) * speed * Time.deltaTime;

        transform.Translate(movement, Space.Self);

        velocityX = Mathf.Clamp(horizontalInput, -1.0f, 1.0f);
        velocityY = Mathf.Clamp(verticalInput, -1.0f, 1.0f);

        animator.SetFloat("velocityX", velocityX);
        animator.SetFloat("velocityY", velocityY);

        if (jump) animator.SetTrigger("jump");



        animator.SetBool("isGrounded", isGrounded);
    }
}
