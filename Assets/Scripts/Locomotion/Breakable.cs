using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.GetComponentInChildren<Pickup>() != null && other.gameObject.GetComponentInChildren<Pickup>().GetState() == LocomotionState.BB8)
		{
			Destroy(gameObject);
		}
	}
}
