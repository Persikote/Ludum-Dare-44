using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
	[SerializeField] private float flipSmoothing;
	private float flip;
	internal float flipTarget;
	private float flipVelocity;

	[Space(7)]
	[SerializeField] private Animator anim;
	[SerializeField] private PlayerMovement playerMovement;
	[SerializeField] private PlayerShooting playerShooting;

	private void Start()
	{
		flip = flipTarget = 1;
	}

	private void Update()
	{
		if (playerMovement.velocity.x != 0)
		{
			flipTarget = Mathf.Sign(playerMovement.velocity.x);
		}

		flip = Mathf.SmoothDamp(flip, flipTarget, ref flipVelocity, flipSmoothing);

		anim.SetBool("Airborne", !playerMovement.controller.info.bottom);
		anim.SetBool("Moving", playerMovement.velocity.x != 0);
		anim.SetBool("Shotgun", playerShooting.shotgunEquipped);
		anim.SetFloat("Direction", flip);
	}
}
