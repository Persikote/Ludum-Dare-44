using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Custom/Gun")]
public class Gun : ScriptableObject
{
	public Sprite sprite;
	
	[Space(7)]
	public bool automatic;
	public float fireRate;
	public float fireTimer;
	public float bulletSpeed;
	public float bulletSpeedVariation;
	public int bulletAmount;
	public float bulletSpread;
	public Vector2 muzzleOffset;
}
