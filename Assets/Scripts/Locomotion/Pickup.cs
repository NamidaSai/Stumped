using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
	[SerializeField] LocomotionState state;

	public LocomotionState GetState()
	{
		return state;
	}
}
