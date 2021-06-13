using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
	[SerializeField] bool followsVertical = false;
	private float length, startpos, startYPos;
	public GameObject cam;
	public float parallaxEffect;

	void Start()
	{
		startpos = transform.position.x;
		startYPos = transform.position.y;
		length = GetComponent<SpriteRenderer>().bounds.size.x;
	}

	void Update()
	{
		float temp = (cam.transform.position.x * (1 - parallaxEffect));
		float dist = (cam.transform.position.x * parallaxEffect);

		if (followsVertical)
		{
			transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);
		}
		else
		{
			transform.position = new Vector3(startpos + dist, startYPos, transform.position.z);
		}

		if (temp > startpos + length) startpos += length;
		else if (temp < startpos - length) startpos -= length;
	}
}
