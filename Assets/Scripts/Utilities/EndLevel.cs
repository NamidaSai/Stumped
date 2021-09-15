using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
	[SerializeField] Transform holeTransform = default;
	[SerializeField] float itemSuckDistance = 0.3f;
	[SerializeField] float suckedInSpeed = 0.65f;
	[SerializeField] bool isEndScreen = false;
	[SerializeField] string nextLevelMusicTrack = "level_1";
	private GameObject winItem = null;
	private bool levelHasEnded = false;

	private void Update()
	{
		if (isEndScreen && !levelHasEnded) { TriggerEndLevel(); }

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
		}

		if (other.gameObject.tag == "Player" && winItem != null)
		{
			TriggerEndLevel();
		}
		else if (other.gameObject.tag == "Player" && other.GetComponentInParent<VehicleHandler>().currentState == LocomotionState.WIN)
		{
			other.GetComponentInParent<VehicleHandler>().TryPickup();
		}
	}

	private void BringItemToHole()
	{
		if (isEndScreen) { return; }

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
		if (winItem != null)
		{
			winItem.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
			winItem.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
			winItem.GetComponent<Collider2D>().enabled = false;
		}

		FindObjectOfType<PlayerController>().GetComponentInChildren<Animator>().SetBool("Success", true);
		FindObjectOfType<PlayerController>().GetComponent<PlayerController>().enabled = false;
		FindObjectOfType<AudioManager>().Stop("FLYJump");

		if (isEndScreen) { return; }

		StartCoroutine(LoadNextLevel());
	}

	private IEnumerator LoadNextLevel()
	{
		yield return new WaitForSeconds(4f);
		FindObjectOfType<MusicPlayer>().Play(nextLevelMusicTrack);
		FindObjectOfType<SceneLoader>().LoadNextScene();
	}

	public void ResetLevel()
	{
		FindObjectOfType<SceneLoader>().ResetScene();
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
