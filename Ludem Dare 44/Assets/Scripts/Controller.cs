using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Controller : MonoBehaviour
{
	private const float skinWidth = 0.02f;

	private new Collider2D collider;
	[SerializeField] private LayerMask rayMask;
	[SerializeField] private float rayInterval;
	private float horizontalRaySpacing;
	private float verticalRaySpacing;
	private int horizontalRayCount;
	private int verticalRayCount;

	[SerializeField] private float maxClimbAngle;
	[SerializeField] private float maxDescendAngle;

	public delegate void Collisions(GameObject go, Vector2 dir);
	private Collisions collisions;
	private List<Transform> alreadyCollided;

	private RaycastOrigins raycastOrigins;
	public Information info;

	private void Start()
	{
		collider = GetComponent<Collider2D>();

		alreadyCollided = new List<Transform>();

		Bounds bounds = collider.bounds;
		bounds.Expand(-2 * skinWidth);

		rayInterval = Mathf.Clamp(rayInterval, Mathf.Min(bounds.size.x, bounds.size.y), float.MaxValue);

		horizontalRayCount = (int)Mathf.Ceil(bounds.size.y / rayInterval) + 1;
		verticalRayCount = (int)Mathf.Ceil(bounds.size.x / rayInterval) + 1;

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	public void SetCollisionsMethod(Collisions method)
	{
		collisions = method;
	}

	private void CalculateRaycastOrigins()
	{
		Bounds bounds = collider.bounds;
		bounds.Expand(-2 * skinWidth);

		raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
		raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
	}

	private struct RaycastOrigins
	{
		public Vector2 topLeft;
		public Vector2 topRight;
		public Vector2 bottomLeft;
		public Vector2 bottomRight;
	}

	public void Move(Vector2 velocity)
	{
		alreadyCollided.Clear();
		info.Reset();
		CalculateRaycastOrigins();

		if (velocity.y < 0)
		{
			DescendSlope(ref velocity);
		}

		if (velocity.x != 0)
		{
			HorizontalRaycasting(ref velocity);
		}
		if (velocity.y != 0)
		{
			VerticalRaycasting(ref velocity);
		}

		transform.Translate(velocity, Space.World);
	}

	private void HorizontalRaycasting(ref Vector2 velocity)
	{
		float direction = Mathf.Sign(velocity.x);
		float distance = Mathf.Abs(velocity.x) + skinWidth;

		for (int i = 0; i < horizontalRayCount; i++)
		{
			Vector2 rayOrigin = (direction == 1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
			rayOrigin += Vector2.up * horizontalRaySpacing * i;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * direction, distance, rayMask);

			if (hit)
			{
				float slopeAngle = -Vector2.SignedAngle(hit.normal, Vector2.up);

				if (i == 0 && slopeAngle <= maxClimbAngle && slopeAngle >= -maxClimbAngle)
				{
					float distanceToSlope = 0;

					if (slopeAngle != info.slopeAngleOld)
					{
						distanceToSlope = hit.distance - skinWidth;
						velocity.x -= distanceToSlope * direction;
					}

					ClimbSlope(ref velocity, slopeAngle);
					velocity.x += distanceToSlope * direction;
				}

				if (!info.climbingSlope || slopeAngle > maxClimbAngle && slopeAngle < -maxClimbAngle)
				{
					distance = hit.distance;
					velocity.x = (hit.distance - skinWidth) * direction;

					if (info.climbingSlope)
					{
						velocity.y = Mathf.Tan(info.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
					}

					if (!alreadyCollided.Contains(hit.transform) && collisions != null)
					{
						alreadyCollided.Add(hit.transform);
						collisions.Invoke(hit.transform.gameObject, Vector2.right * direction);
					}

					info.right = direction == 1;
					info.left = direction == -1;
				}
			}
		}
	}

	private void VerticalRaycasting(ref Vector2 velocity)
	{
		float direction = Mathf.Sign(velocity.y);
		float distance = Mathf.Abs(velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++)
		{
			Vector2 rayOrigin = (direction == 1) ? raycastOrigins.topLeft : raycastOrigins.bottomLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * direction, distance + 0.01f, rayMask);

			if (hit)
			{
				distance = hit.distance;
				velocity.y = (hit.distance - skinWidth) * direction;

				if (!alreadyCollided.Contains(hit.transform) && collisions != null)
				{
					alreadyCollided.Add(hit.transform);
					collisions.Invoke(hit.transform.gameObject, Vector2.up * direction);
				}

				if (direction == -1)
				{
					info.platform = hit.collider;
				}

				info.top = direction == 1;
				info.bottom = direction == -1;
			}
		}
	}

	private void ClimbSlope(ref Vector2 velocity, float slopeAngle)
	{
		float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * velocity.x;

		if (velocity.y <= climbVelocityY)
		{
			velocity.y = climbVelocityY;
			velocity.x *= Mathf.Cos(slopeAngle * Mathf.Deg2Rad);

			info.bottom = true;
			info.climbingSlope = true;

			info.slopeAngle = slopeAngle;
		}
	}

	private void DescendSlope(ref Vector2 velocity)
	{
		float dir = Mathf.Sign(velocity.x);
		Vector2 rayOrigin = (dir == 1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, rayMask);

		if (hit)
		{
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

			if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
			{
				if (Mathf.Sign(hit.normal.x) == dir)
				{
					if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
					{
						float moveDistance = Mathf.Abs(velocity.x);
						float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

						velocity.y -= descendVelocityY;
						velocity.x *= Mathf.Cos(slopeAngle * Mathf.Deg2Rad);

						info.bottom = true;
						info.descendingSlope = true;

						info.slopeAngle = slopeAngle;
					}
				}
			}
		}
	}

	public struct Information
	{
		public bool top, bottom;
		public bool right, left;

		public bool climbingSlope, descendingSlope;
		public float slopeAngle, slopeAngleOld;

		public Collider2D platform;

		public void Reset()
		{
			top = bottom = false;
			right = left = false;

			climbingSlope = descendingSlope = false;
			slopeAngleOld = slopeAngle;
			slopeAngle = 0;

			platform = null;
		}
	}
}
