using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
	[SerializeField] private Transform origo, guns, pistolHand, shotgunHand;
	[SerializeField] private float maxMouseDistance;
	[SerializeField] private float maxHandDistance;
	[SerializeField] private float handSmoothing;
	[SerializeField] private float handRotationSpeed;
	private Quaternion targetRotation;
	private Vector2 targetPosition;
	private Vector2 velocityPosition;

	[Space(7)]
	[SerializeField] private ObjectPool bulletPool;
	[SerializeField] private float fireRate;
	[SerializeField] private float bulletSpeed;
	[SerializeField] private float bulletSpeedVariation;
	[SerializeField] private int bulletAmount;
	[SerializeField] private float bulletSpread;
	[SerializeField] private Vector2 bulletOffset;
	private float fireTimer;
	private bool automatic;
	internal bool shotgunEquipped;

	private void Update()
	{
		Vector2 mouseDistance = Camera.main.ScreenToWorldPoint(Input.mousePosition) - origo.position;
		float aimAngle = Vector2.SignedAngle(Vector2.up, mouseDistance);
		targetRotation = Quaternion.Euler(0, (aimAngle < 0) ? 0 : 180, -Mathf.Abs(aimAngle)); 
		targetPosition = (Vector2)origo.position + Mathf.Clamp(mouseDistance.sqrMagnitude / maxMouseDistance, 0, 1) * maxHandDistance * mouseDistance.normalized;

		pistolHand.rotation = shotgunHand.rotation = Quaternion.RotateTowards(shotgunHand.rotation, targetRotation, handRotationSpeed * Time.deltaTime);
		guns.position = Vector2.SmoothDamp(guns.position, targetPosition, ref velocityPosition, handSmoothing);

		if ((!automatic && Input.GetButtonDown("Fire") && Time.time >= fireTimer) || (automatic && Input.GetButton("Fire") && Time.time >= fireTimer))
		{
			fireTimer = Time.time + 1 / fireRate;
			FireGun(aimAngle);
		}
	}

	private void FireGun(float aimAngle)
	{
		for (int i = 0; i < bulletAmount; i++)
		{
			Bullet bullet = bulletPool.Instantiate(pistolHand.transform.position + pistolHand.transform.TransformVector(bulletOffset), Quaternion.Euler(0, 0, aimAngle)) as Bullet;

			float bulletAngle = -aimAngle + Random.Range(-bulletSpread / 2, bulletSpread / 2);
			Vector2 bulletDirection = new Vector2(Mathf.Sin(bulletAngle * Mathf.Deg2Rad), Mathf.Cos(bulletAngle * Mathf.Deg2Rad));

			bullet.velocity = bulletDirection * (bulletSpeed + Random.Range(-bulletSpeedVariation / 2, bulletSpeedVariation / 2));
		}
	}
}
