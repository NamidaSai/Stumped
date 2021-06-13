using UnityEngine;

public class VehicleHandler : MonoBehaviour
{
	[SerializeField] LocomotionState currentState = LocomotionState.BASE; //serialized for debugging only
	[SerializeField] float pickupRadius = 3f;
	[SerializeField] Vector2 dropOffset = default;
	[SerializeField] LayerMask pickupLayer = default;
	[SerializeField] GameObject[] allVehicles = default;
	private GameObject currentVehicle = null;
	private GameObject currentPickup = null;

	private AudioManager audioManager;

	private void Start()
	{
		InitVehicle();
		audioManager = FindObjectOfType<AudioManager>();
	}

	private void InitVehicle()
	{
		foreach (GameObject vehicle in allVehicles)
		{
			if (vehicle.activeSelf && currentVehicle == null)
			{
				currentVehicle = vehicle;
			}
			else
			{
				vehicle.SetActive(false);
			}
		}

		if (currentVehicle == null)
		{
			currentVehicle = allVehicles[0];
			currentVehicle.SetActive(true);
		}
	}

	public void TryPickup()
	{
		GameObject nearestPickup = GetClosestPickup();

		if (currentPickup != null)
		{
			DropVehicle();
		}

		if (nearestPickup != null && !nearestPickup.GetComponent<Pickup>().isUsed)
		{
			PickupVehicle(nearestPickup);
		}
	}

	public void DropVehicle()
	{
		currentPickup.SetActive(true);

		if (currentPickup.GetComponent<Pickup>().isOneTimeOnly)
		{
			currentPickup.GetComponent<Pickup>().isUsed = true;
			StartCoroutine(currentPickup.GetComponent<Pickup>().Respawn());
		}

		SwitchState(LocomotionState.BASE);

		Vector2 targetPosition = (Vector2)transform.position + dropOffset;
		float targetRotation = Random.Range(-180f, 180f);
		currentPickup.transform.position = targetPosition;
		currentPickup.transform.eulerAngles = new Vector3(currentPickup.transform.eulerAngles.x,
    							currentPickup.transform.eulerAngles.y,
    							currentPickup.transform.eulerAngles.z + targetRotation);

		currentPickup = null;
	}

	private void PickupVehicle(GameObject pickup)
	{
		currentPickup = pickup;
		SwitchState(currentPickup.GetComponent<Pickup>().GetState());
		PlayStateFX(currentPickup.GetComponent<Pickup>().GetState());
		currentPickup.SetActive(false);
	}

	private void PlayStateFX(LocomotionState state)
	{
		switch (state)
		{
			case LocomotionState.BB8:
				audioManager.Play("BB8Pickup");
				break;
			case LocomotionState.POGO:
				audioManager.Play("POGOPickup");
				break;
			case LocomotionState.FLY:
				audioManager.Play("FLYPickup");
				break;
			default:
				audioManager.Play("BASEPickup");
				break;
		}
	}

	private GameObject GetClosestPickup()
	{
		Collider2D[] nearbyPickups = Physics2D.OverlapCircleAll(transform.position, pickupRadius, pickupLayer);

		GameObject closestPickup = null;

		foreach (Collider2D pickup in nearbyPickups)
		{
			float distanceToPickup = Vector2.Distance(transform.position, pickup.gameObject.transform.position);

			if (closestPickup != null && DistanceTo(pickup.gameObject) >= DistanceTo(closestPickup))
			{
				continue;
			}

			closestPickup = pickup.gameObject;
		}

		return closestPickup;
	}

	private float DistanceTo(GameObject target)
	{
		float distance = Vector2.Distance(transform.position, target.transform.position);
		return distance;
	}

	private void SwitchState(LocomotionState targetState)
	{
		currentState = targetState;
		SetActiveVehicle();
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.position, pickupRadius);
	}

	private void SetActiveVehicle()
	{
		currentVehicle.SetActive(false);

		foreach (GameObject vehicle in allVehicles)
		{
			if (currentState == vehicle.GetComponent<Pickup>().GetState())
			{
				currentVehicle = vehicle;
				currentVehicle.SetActive(true);
				return;
			}
		}

		Debug.LogWarning(currentState + " does not have an assigned vehicle.");
	}
}