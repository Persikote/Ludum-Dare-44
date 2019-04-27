using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller))]
public class Bullet : ObjectPoolItem
{
	public override void OnInstantiate() { }

	internal Vector2 velocity;
	private Controller controller;

	private void Start()
	{
		controller = GetComponent<Controller>();
		controller.SetCollisionsMethod(Collisions);
	}

	private void Update()
	{
		controller.Move(velocity * Time.deltaTime);
	}

	private void Collisions(GameObject go, Vector2 dir)
	{
		gameObject.SetActive(false);
	}
}
