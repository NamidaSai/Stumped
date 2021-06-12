using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
	[SerializeField] GameObject selfPrefab = default;
	[SerializeField] float respawnDelay = 3f;
	[SerializeField] LocomotionState state;
	[SerializeField] public bool isOneTimeOnly = false;

	private Vector2 startPosition;

	private void Start()
	{
		startPosition = transform.position;
	}

	public bool isUsed = false;

	public LocomotionState GetState()
	{
		return state;
	}

	public IEnumerator Respawn()
	{
		yield return new WaitForSeconds(respawnDelay);

		if (selfPrefab != null)
		{
			GameObject newPickup = Instantiate(selfPrefab, startPosition, Quaternion.identity);
			newPickup.GetComponent<Pickup>().isUsed = false;
		}

		VanishFX();
	}

	private void VanishFX()
	{
		if (gameObject == null) { return; }
		Destroy(gameObject);
	}
}
