using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	private Vector2 moveInput;

	private void FixedUpdate()
	{
		GetComponent<PlayerMover>().Move(moveInput);

		if (moveInput.y > 0.5f)
		{
			GetComponent<PlayerMover>().Jump();
		}
	}

	private void OnMove(InputValue value)
	{
		moveInput = value.Get<Vector2>();
	}

	private void OnPickup()
	{
		GetComponent<VehicleHandler>().TryPickup();
	}
}
