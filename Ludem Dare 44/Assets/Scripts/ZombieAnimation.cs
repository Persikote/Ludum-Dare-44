using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAnimation : MonoBehaviour
{
	[SerializeField] private float flipSmoothing;
	private float flip;
	internal float flipTarget;
	private float flipVelocity;

	[Space(7)]
	[SerializeField] private Animator anim;
	[SerializeField] private ZombieMovement zombieMovement;

	private void Start()
	{
		flip = flipTarget = 1;
	}

	private void Update()
	{
		if (zombieMovement.velocity.x != 0)
		{
			flipTarget = Mathf.Sign(zombieMovement.velocity.x);
		}

		flip = Mathf.SmoothDamp(flip, flipTarget, ref flipVelocity, flipSmoothing);

		anim.SetBool("Airborne", !zombieMovement.controller.info.bottom);
		anim.SetBool("Moving", zombieMovement.velocity.x != 0);
		anim.SetFloat("Direction", flip);
	}
}
