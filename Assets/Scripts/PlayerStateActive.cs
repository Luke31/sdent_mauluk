using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class PlayerStateActive : PlayerState
{

	public PlayerStateActive(GamePhysics p, PlayerMovement c) : base(p, c)
	{
	}

	public override void Enter()
	{
		Context.PlayAudioRopeImpact();
	}

	private Vector2 GetLeftForce()
	{
		return -Vector3.Cross(GetRopeDir(), Vector3.forward).normalized * Physics.ropeSwingForce;
	}

	private Vector2 GetRightForce()
	{
		return -GetLeftForce();
	}

	public override void AimLeft(float inputForce)
	{
		Physics.rb.AddForce(GetLeftForce() * Math.Abs(inputForce));
	}

	public override void AimRight(float inputForce)
	{
		Physics.rb.AddForce(GetRightForce() * Math.Abs(inputForce));
	}

	public override void RopeIn(float inputForce)
	{
		RaycastHit2D hitFeed = Physics2D.Raycast(Physics.Player.transform.position, Physics.ropeDir, Physics.ropeFeedSpeed * 3, Physics.LayerMaskWithoutPlayer);
		if (hitFeed.collider == null && Physics.Hinge.distance - Physics.ropeFeedSpeed > Physics.ropeMinLength)
		{
			Physics.Hinge.distance -= Physics.ropeFeedSpeed * Math.Abs(inputForce);
		}
	}

	public override void Jump()
	{
		Context.SetState(GameStates.Collapsing);
	}

	public override void RopeOut(float inputForce)
	{
		RaycastHit2D hitFeed = Physics2D.Raycast(Physics.Player.transform.position, -Physics.ropeDir, Physics.ropeFeedSpeed * 3, Physics.LayerMaskWithoutPlayer);
		if (hitFeed.collider == null)
		{
			Physics.Hinge.distance += Physics.ropeFeedSpeed * Math.Abs(inputForce);
		}
	}

	public override void Update()
	{
		Physics.linePoints[0] = Physics.Player.transform.position;
		Physics._ropeRenderer.Update(Physics.linePoints);
	}

	public override void FixedUpdate()
	{
		Physics.linePoints[0] = Physics.Player.transform.position;
		Physics.ropeDir = Physics.linePoints[1] - Physics.linePoints[0];

		RaycastHit2D hitSplit;
		RaycastHit2D hitLast;

		hitSplit = Physics2D.Raycast(Physics.Player.transform.position, Physics.ropeDir, Physics.ropeDir.magnitude - 0.1f, Physics.LayerMaskWithoutPlayer);

		if (hitSplit.collider != null)
		{
			Vector2 hullPoint = GetClosestPointOnBoundHull(hitSplit.collider, hitSplit.point);
			Vector3 hitPoint = new Vector3(hullPoint.x, hullPoint.y);
			float newRopeLength = Vector2.Distance(Physics.linePoints[0], hitPoint);
			float newSplitLength = Vector2.Distance(Physics.linePoints[1], hitPoint);

			// A new point in between (far away, enough to warrant a split in the rope)
			if (newRopeLength > Physics.ropeMinLength && newSplitLength > Physics.ropeSplitEpsilon)
			{
				Vector3[] newPoints = new Vector3[Physics.linePoints.Length + 1];

				newPoints[0] = Physics.linePoints[0]; // player
				newPoints[1] = hitPoint;        // new anchor
				newPoints[2] = Physics.linePoints[1];   // last anchor

				for (int i = 2; i < Physics.linePoints.Length; i++)
				{
					newPoints[i + 1] = Physics.linePoints[i];
				}

				Physics.linePoints = newPoints;

				Physics.Hinge.connectedAnchor = Physics.linePoints[1];
				Physics.Hinge.distance = Vector3.Distance(Physics.Player.transform.position, Physics.linePoints[1]);
			}
		}
		else if (Physics.linePoints.Length > 2)
		{
			Vector2 lastDir = Physics.linePoints[2] - Physics.linePoints[0];
			hitLast = Physics2D.Raycast(Physics.Player.transform.position, lastDir, lastDir.magnitude - 0.1f, Physics.LayerMaskWithoutPlayer);

			Vector2 ab = Physics.linePoints[1] - Physics.linePoints[0];
			Vector2 bc = Physics.linePoints[2] - Physics.linePoints[1];
			Vector2 ac = Physics.linePoints[2] - Physics.linePoints[0];

			// We see both last points, maybe release one
			if (hitLast.collider == null && Mathf.Abs(ab.magnitude + bc.magnitude - ac.magnitude) < Physics.ropeSplitEpsilon)
			{
				Vector3[] newPoints = new Vector3[Physics.linePoints.Length - 1];

				newPoints[0] = Physics.linePoints[0]; // player
				newPoints[1] = Physics.linePoints[2];   // new anchor = lastAnchor

				for (int i = 2; i < Physics.linePoints.Length - 1; i++)
				{
					newPoints[i] = Physics.linePoints[i + 1];
				}

				Physics.linePoints = newPoints;

				Physics.Hinge.connectedAnchor = Physics.linePoints[1];
				Physics.Hinge.distance = Vector3.Distance(Physics.Player.transform.position, Physics.linePoints[1]);
			}
		}
	}

	private Vector2 GetRopeDir()
	{
		return Physics.linePoints[1] - Physics.linePoints[0];
	}

	public override void Exit()
	{
		Physics.Hinge.enabled = false;
		Physics.playerCollider.sharedMaterial = Physics.matStatic;
	}

	

	private Vector2 GetClosestPointOnBoundHull(Collider2D collider, Vector2 hitPoint)
	{
		Vector2 result = new Vector2();
		Vector2 tmp;

		Vector2 center = collider.bounds.center;
		Vector2 extent = collider.bounds.extents;

		float minDist = float.MaxValue;
		float dist;

		tmp = center + extent;
		dist = Vector2.Distance(hitPoint, tmp);
		if (dist < minDist)
		{
			minDist = dist;
			result = tmp;
		}

		extent.Set(extent.x, -extent.y);

		tmp = center + extent;
		dist = Vector2.Distance(hitPoint, tmp);
		if (dist < minDist)
		{
			minDist = dist;
			result = tmp;
		}

		extent.Set(-extent.x, extent.y);

		tmp = center + extent;
		dist = Vector2.Distance(hitPoint, tmp);
		if (dist < minDist)
		{
			minDist = dist;
			result = tmp;
		}

		extent.Set(extent.x, -extent.y);

		tmp = center + extent;
		dist = Vector2.Distance(hitPoint, tmp);
		if (dist < minDist)
		{
			minDist = dist;
			result = tmp;
		}

		return result;
	}
}

