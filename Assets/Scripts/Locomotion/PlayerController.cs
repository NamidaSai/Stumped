using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	private bool usingTethers = false;
	private Vector2 moveInput;

	private void Start()
	{
		if (GetComponent<PlayerTethering>() != null)
		{
			Debug.Log("Using tether controls");
			usingTethers = true;
		}
	}

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
		if (usingTethers) { return; }

		GetComponent<VehicleHandler>().TryPickup();
	}
}
