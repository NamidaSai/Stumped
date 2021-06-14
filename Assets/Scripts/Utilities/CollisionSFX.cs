using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSFX : MonoBehaviour
{
	[SerializeField] string soundName = "BaseCollision";

	private void OnCollisionEnter2D(Collision2D other)
	{
		FindObjectOfType<AudioManager>().Play(soundName);
	}
}
