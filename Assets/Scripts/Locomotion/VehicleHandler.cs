using UnityEngine;

public class VehicleHandler : MonoBehaviour
{
	[SerializeField] LocomotionState currentState = LocomotionState.BASIC; //serialized for debugging only
	[SerializeField] float pickupRadius = 3f;
	[SerializeField] Vector2 dropOffset = default;
	[SerializeField] LayerMask pickupLayer = default;
	private GameObject currentVehicle = null;

	public void TryPickup()
	{
		GameObject nearestPickup = GetClosestPickup();

		if (currentVehicle != null)
		{
			DropVehicle();
		}

		if (nearestPickup != null)
		{
			PickupVehicle(nearestPickup);
		}
	}

	private void DropVehicle()
	{
		SwitchState(LocomotionState.BASIC);
		currentVehicle.SetActive(true);
		Vector2 targetPosition = (Vector2)transform.position + dropOffset;
		currentVehicle.transform.position = targetPosition;
		currentVehicle = null;
	}

	private void PickupVehicle(GameObject pickup)
	{
		currentVehicle = pickup;
		SwitchState(currentVehicle.GetComponent<Pickup>().GetState());
		currentVehicle.SetActive(false);
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
	}
}