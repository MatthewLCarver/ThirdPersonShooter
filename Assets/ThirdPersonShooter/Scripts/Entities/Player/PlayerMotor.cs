using System;

using UnityEngine;
using UnityEngine.InputSystem;

namespace ThirdPersonShooter.Entities.Player
{
	public class PlayerMotor : MonoBehaviour
	{
		private static readonly int speedID = Animator.StringToHash("Speed");

		private InputAction MoveAction => moveActionReference.action;
		
		[SerializeField, Range(0, 1)] private float alignmentDamping = .1f;
		[SerializeField] private InputActionReference moveActionReference;

		[Header("Components")] 
		[SerializeField] private new Transform camera;
		[SerializeField] private Transform render;
		[SerializeField] private Animator animator;

		private new Rigidbody rigidBody;
		private PlayerEntity player;
		private Vector2 input;
		private Vector3 dampingVelocity;

		private void OnEnable()
		{
			rigidBody = gameObject.GetComponentInParent<Rigidbody>();
			player = gameObject.GetComponentInParent<PlayerEntity>();

			MoveAction.performed += OnMovePerformed;
			MoveAction.canceled += OnMoveCanceled;
		}

		private void OnDisable()
		{
			MoveAction.performed -= OnMovePerformed;
			MoveAction.canceled -= OnMoveCanceled;
		}

		private void FixedUpdate()
		{
			// Movement Handling
			rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, player.Stats.Speed);
			
			if(input.normalized.magnitude != 0)
				rigidBody.AddForce(((camera.forward * input.y) + (camera.right * input.x)) * player.Stats.Speed, ForceMode.Impulse);
			
			// Rendering Handling
			if(animator)
				animator.SetFloat(speedID, Mathf.Clamp01(rigidBody.velocity.magnitude));
			
			if(Mathf.Approximately(input.normalized.magnitude, 1))
				render.forward = Vector3.SmoothDamp(render.forward, camera.forward, ref dampingVelocity, alignmentDamping);
		}
		
		private void OnMovePerformed(InputAction.CallbackContext _context) => input = _context.ReadValue<Vector2>();

		private void OnMoveCanceled(InputAction.CallbackContext _context) => input = Vector2.zero;
	}
}