using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
	[SerializeField] private Transform arm;

	[Space(7)]
	[SerializeField] private ObjectPool bulletPool;
	[SerializeField] private Gun gun;
	[SerializeField] private SpriteRenderer gunRenderer;

	private void Start()
	{
		SetGun(gun);
	}

	private void Update()
	{
		Vector2 mouseDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - arm.position;
		float aimAngle = -Vector2.Angle(Vector2.up, mouseDirection);
		arm.rotation = Quaternion.Euler(0, 0, aimAngle);

		if ((!gun.automatic && Input.GetButtonDown("Fire") && Time.time >= gun.fireTimer) || (gun.automatic && Input.GetButton("Fire") && Time.time >= gun.fireTimer))
		{
			gun.fireTimer = Time.time + 1 / gun.fireRate;
			FireGun(aimAngle);
		}
	}

	private void FireGun(float aimAngle)
	{
		for (int i = 0; i < gun.bulletAmount; i++)
		{
			Bullet bullet = bulletPool.Instantiate(gunRenderer.transform.position + gunRenderer.transform.TransformVector(gun.muzzleOffset), Quaternion.Euler(0, 0, aimAngle)) as Bullet;

			float bulletAngle = -aimAngle + Random.Range(-gun.bulletSpread / 2, gun.bulletSpread / 2);
			Vector2 bulletDirection = new Vector2(Mathf.Sin(bulletAngle * Mathf.Deg2Rad), Mathf.Cos(bulletAngle * Mathf.Deg2Rad));

			bullet.velocity = bulletDirection * (gun.bulletSpeed + Random.Range(-gun.bulletSpeedVariation / 2, gun.bulletSpeedVariation / 2));
		}
	}

	public void SetGun(Gun newGun)
	{
		gun = newGun;
		gunRenderer.sprite = gun.sprite;
		gun.fireTimer = 0;
	}
}
