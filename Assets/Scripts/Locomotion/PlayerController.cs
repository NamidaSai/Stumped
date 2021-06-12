using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	private Vector2 moveInput;

	private void FixedUpdate()
	{
		if (GetComponentInChildren<IMover>() == null) { return; }

		GetComponentInChildren<IMover>().Move(moveInput);

		if (moveInput.y > 0.5f)
		{
			GetComponentInChildren<IMover>().Jump();
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
