using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "WinItem")
		{
			TriggerEndLevel();
		}
	}

	private void TriggerEndLevel()
	{
		Debug.Log("Level ended!");
		FindObjectOfType<PlayerController>().GetComponentInChildren<Animator>().SetTrigger("Success");
		FindObjectOfType<PlayerController>().GetComponent<PlayerController>().enabled = false;
	}
}
