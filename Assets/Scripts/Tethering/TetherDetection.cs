using UnityEngine;

public class TetherDetection : MonoBehaviour
{
	[SerializeField] BasicMover baseMover = default;
	[SerializeField] Tether attachedTether = default;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.GetComponentInParent<Pickup>() != null)
		{
			baseMover.currentTarget = other.gameObject.transform.root;
		}
		else
		{
			attachedTether.ToggleGrab();
		}
	}
}