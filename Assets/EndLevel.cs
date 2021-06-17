using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
	[SerializeField] Transform holeTransform = default;
	[SerializeField] float itemSuckDistance = 0.3f;
	[SerializeField] float suckedInSpeed = 0.65f;
	private GameObject winItem = null;
	private bool levelHasEnded = false;

	private void Update()
	{
		if (winItem != null && levelHasEnded)
		{
			BringItemToHole();
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "WinItem")
		{
			winItem = other.gameObject;
			other.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
			other.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
			other.enabled = false;
		}

		if (other.gameObject.tag == "Player" && winItem != null)
		{
			TriggerEndLevel();
		}
	}

	private void BringItemToHole()
	{
		float distanceToItem = Vector2.Distance(winItem.transform.position, holeTransform.position);

		if (distanceToItem > itemSuckDistance)
		{
			winItem.transform.position = Vector2.MoveTowards(winItem.transform.position, holeTransform.position, Time.deltaTime * suckedInSpeed);
		}
		else
		{
			winItem.GetComponent<Animator>().SetTrigger("SuckedIn");
			Destroy(winItem, 1f);
		}
	}

	private void TriggerEndLevel()
	{
		levelHasEnded = true;
		FindObjectOfType<PlayerController>().GetComponentInChildren<Animator>().SetBool("Success", true);
		FindObjectOfType<PlayerController>().GetComponent<PlayerController>().enabled = false;
	}
}