using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doorway : MonoBehaviour
{
	[SerializeField] private Doorway linkedDoor;

	[Space(7)]
	[SerializeField] private GameObject buttonGuide;
	[SerializeField] private new Collider2D collider;
	private Transform player;

	private bool activated;

	private void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}

	private void Update()
	{
		if (activated && Input.GetButtonDown("Interact"))
		{
			player.position += linkedDoor.transform.position - transform.position;
		}

		activated = collider.OverlapPoint(player.transform.position);
		buttonGuide.SetActive(activated);
	}
}
