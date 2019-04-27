using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
	[SerializeField] private Animator anim;
	[SerializeField] private PlayerMovement playerMovement;
	[SerializeField] private PlayerShooting playerShooting;

	private void Update()
	{
		anim.SetBool("Airborne", !playerMovement.controller.info.bottom);
		anim.SetBool("Moving", playerMovement.velocity.x != 0);
		anim.SetBool("Shotgun", playerShooting.shotgunEquipped);
	}
}
