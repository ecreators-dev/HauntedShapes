using Assets.MiniFirstPersonController.Scripts;

using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
		[SerializeField] private Transform character;
    [SerializeField] private float sensitivity = 2;
    [SerializeField] private float smoothing = 1.5f;
    [SerializeField] private float limitDown = -90;
    [SerializeField] private float limitUp = 0;

    private Vector2 velocity;
    private Vector2 frameVelocity;

		private Transform Transform { get; set; }

		private void Reset()
    {
        // Get the character from the FirstPersonMovement in parents.
        character = GetComponentInParent<PlayerMovement>().transform;
    }

		private void Awake()
		{
				Transform = transform;
		}

		private void Start()
    {
        // Lock the mouse cursor to the game screen.
        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Get smooth velocity.
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, limitDown, limitUp);

        // Rotate camera up-down and controller left-right from velocity.
        Transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.rotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
    }
}
