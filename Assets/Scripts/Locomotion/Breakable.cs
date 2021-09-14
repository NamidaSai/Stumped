using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
	[SerializeField] GameObject particleFX = default;
	[SerializeField] float fxDuration = 2f;

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.GetComponentInChildren<Pickup>() != null && other.gameObject.GetComponentInChildren<Pickup>().GetState() == LocomotionState.BB8)
		{
			TriggerFX();
			Destroy(gameObject);
		}
	}

	private void TriggerFX()
	{
		GameObject vfx = Instantiate(particleFX, transform.position, particleFX.transform.rotation) as GameObject;
		
		Destroy(vfx, fxDuration);
	}
}
