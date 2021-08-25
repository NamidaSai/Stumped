using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
	[SerializeField] GameObject selfPrefab = default;
	[SerializeField] float respawnDelay = 3f;
	[SerializeField] LocomotionState state;
	[SerializeField] public Vector2 offsetDrop = default;
	[SerializeField] public bool hasRandomRotationDrop = false;
	[SerializeField] public bool hasOffSetPickUp = true;

	private Vector2 startPosition;
	private Sprite originalSprite;

	private void Start()
	{
		startPosition = transform.position;
		originalSprite = GetComponentInChildren<SpriteRenderer>().sprite;
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
			newPickup.GetComponentInChildren<SpriteRenderer>().sprite = originalSprite;
		}

		VanishFX();
	}

	private void VanishFX()
	{
		if (gameObject == null) { return; }
		Destroy(gameObject);
	}
}
