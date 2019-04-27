using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller))]
public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private float moveSpeed;
	[SerializeField] private float gravity;
	[SerializeField] private float maxJumpHeight;
	[SerializeField] private float minJumpHeight;
	private float maxJumpVelocity;
	private float minJumpVelocity;
	internal Vector2 velocity;
	internal Controller controller;

	private void Start()
	{
		controller = GetComponent<Controller>();

		maxJumpVelocity = Mathf.Sqrt(2 * gravity * maxJumpHeight);
		minJumpVelocity = Mathf.Sqrt(2 * gravity * minJumpHeight);
	}

	private void Update()
	{
		float moveInput = Input.GetAxisRaw("Horizontal");
		velocity.x = moveSpeed * moveInput;

		if (controller.info.bottom || controller.info.top)
		{
			velocity.y = 0;
		}

		velocity.y -= gravity * Time.deltaTime;

		if (Input.GetButtonDown("Jump") && controller.info.bottom)
		{
			velocity.y = maxJumpVelocity;
		}
		if (Input.GetButtonUp("Jump") && velocity.y > minJumpVelocity)
		{
			velocity.y = minJumpVelocity;
		}

		controller.Move(velocity * Time.deltaTime);
	}
}
