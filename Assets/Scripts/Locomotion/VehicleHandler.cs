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

	private void Start()
	{
		InitVehicle();
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
		}
	}

	public void TryPickup()
	{
		Debug.Log("Called Try");
		GameObject nearestPickup = GetClosestPickup();

		if (currentPickup != null)
		{
			DropVehicle();
			Debug.Log("Called Drop");
		}

		if (nearestPickup != null)
		{
			PickupVehicle(nearestPickup);
			Debug.Log("Called Pickup");
		}
	}

	private void DropVehicle()
	{
		SwitchState(LocomotionState.BASE);
		currentPickup.SetActive(true);
		Vector2 targetPosition = (Vector2)transform.position + dropOffset;
		currentPickup.transform.position = targetPosition;
		currentPickup = null;
	}

	private void PickupVehicle(GameObject pickup)
	{
		currentPickup = pickup;
		SwitchState(currentPickup.GetComponent<Pickup>().GetState());
		currentPickup.SetActive(false);
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
			else
			{
				Debug.LogWarning(currentState + " does not have an assigned vehicle.");
			}
		}
	}
}