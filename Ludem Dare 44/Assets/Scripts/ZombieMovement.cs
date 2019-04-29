using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMovement : MonoBehaviour
{
	[SerializeField] private float moveSpeed;
	[SerializeField] private float gravity;
	[SerializeField] private float playerJumpDistance;
	[SerializeField] private float jumpDelay;
	[SerializeField] private float jumpHeight;
	private float jumpTimer;
	private float jumpVelocity;
	private bool jumpInitialized;
	internal Vector2 velocity;
	internal Controller controller;

	private Transform player;

	private void Start()
	{
		controller = GetComponent<Controller>();
		player = GameObject.FindGameObjectWithTag("Player").transform;

		jumpVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);
	}

	private void Update()
	{
		float playerDistance = player.position.x - transform.position.x;
		float moveInput = Mathf.Sign(playerDistance);

		velocity.x = moveSpeed * moveInput;

		if (controller.info.bottom || controller.info.top)
		{
			velocity.y = 0;
		}

		velocity.y -= gravity * Time.deltaTime;

		if (!jumpInitialized && controller.info.bottom && Input.GetButtonDown("Jump") && controller.info.bottom && Mathf.Abs(playerDistance) <= playerJumpDistance)
		{
			jumpInitialized = true;
			jumpTimer = Time.time + jumpDelay;
		}

		if (jumpInitialized && Time.time >= jumpTimer)
		{
			jumpInitialized = false;
			velocity.y = jumpVelocity;
		}

		controller.Move(velocity * Time.deltaTime);
	}
}
