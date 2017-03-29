using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeActiveBehaviour : PlayerBehaviour {

	public float ropeWidth = 0.2f;
	public float ropeSplitEpsilon = 0.2f;
	public float ropeMinLength = 1f;
	public float ropeFeedSpeed = 0.5f;
	public float ropeSwingForce = 0.6f;

	public LayerMask ropeLayerMask = -1;

	Rigidbody2D rb;

	CircleCollider2D playerCollider;

	PhysicsMaterial2D matBouncy;
	PhysicsMaterial2D matStatic;

	DistanceJoint2D hinge;

	LineRenderer lineRenderer;
	Vector3[] linePoints;
	Vector2 ropeDir;


	void resetRope()
	{
		linePoints = new Vector3[2];
		for (int i = 0; i < linePoints.Length; i++)
		{
			linePoints[i] = Vector3.zero;
		}

		lineRenderer.numPositions = linePoints.Length;
		lineRenderer.SetPositions(linePoints);
	}

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	public override void Enter()
	{
		//initHinge.x = animator.GetFloat("InitHingeX");
		//initHinge.y = animator.GetFloat("InitHingeY");
		//ropeDir = initRopeDir;

		layerMask = LayerMask.GetMask("Player");
		layerMask = ~layerMask;
		
		_ropeRenderer = new RopeRenderer(Player.GetComponentInChildren<LineRenderer>(), ropeWidth,ropeMinLength, layerMask);
		
		playerCollider = Player.GetComponentInChildren<CircleCollider2D>();

		matBouncy = (PhysicsMaterial2D)Resources.Load("PlayerBouncy");
		matStatic = (PhysicsMaterial2D)Resources.Load("PlayerStatic");

		resetRope();

		rb = Player.GetComponent<Rigidbody2D>();

		ActivateRope();
	}
  
  private void ActivateHinge(Vector3 hitPoint)
	{
		linePoints[1].x = hitPoint.x;
		linePoints[1].y = hitPoint.y;
	}

	private Vector2 GetLeftForce()
	{
		return -Vector3.Cross(GetRopeDir(), Vector3.forward).normalized * ropeSwingForce;
	}

	private Vector2 GetRightFoce()
	{
		return -GetLeftFoce();
	}
  
  public override void AimLeft(float inputForce)
	{    
		rb.AddForce(GetLeftForce() * Math.Abs(inputForce));
	}

	public override void AimRight(float inputForce)
	{
		rb.AddForce(GetRightForce() * Math.Abs(inputForceHor));
	}

	public override void RopeIn(float inputForce)
	{
		RaycastHit2D hitFeed = Physics2D.Raycast(Player.transform.position, ropeDir, ropeFeedSpeed * 3, layerMask);
    if (hitFeed.collider == null && hinge.distance - ropeFeedSpeed > ropeMinLength)
    {
      hinge.distance -= ropeFeedSpeed * Math.Abs(inputForce);
    }
	}
  
  public override void Jump(){
    //TODO: Set State Rope Inactive
  }

	public override void RopeOut(float inputForce)
	{
		RaycastHit2D hitFeed = Physics2D.Raycast(Player.transform.position, -ropeDir, ropeFeedSpeed * 3, layerMask);
			if (hitFeed.collider == null)
			{
				hinge.distance += ropeFeedSpeed * Math.Abs(inputForce);
			}
	}

	public override void Update()
	{
		linePoints[0] = Player.transform.position;

		Vector3[] invertedLinePoints = new Vector3[linePoints.Length];
		for (int i = 0; i < linePoints.Length; i++)
		{
			invertedLinePoints[i] = linePoints[linePoints.Length - 1 - i];
		}
		lineRenderer.numPositions = invertedLinePoints.Length;
		lineRenderer.SetPositions(invertedLinePoints);
	}

	public override void FixedUpdate()
	{
		linePoints[0] = Player.transform.position;
		ropeDir = linePoints[1] - linePoints[0];

		RaycastHit2D hitSplit;
		RaycastHit2D hitLast;

		hitSplit = Physics2D.Raycast(Player.transform.position, ropeDir, ropeDir.magnitude - 0.1f, ropeLayerMask.value);

		if (hitSplit.collider != null)
		{
			Vector2 hullPoint = GetClosestPointOnBoundHull(hitSplit.collider, hitSplit.point);
			Vector3 hitPoint = new Vector3(hullPoint.x, hullPoint.y);
			float newRopeLength = Vector2.Distance(linePoints[0], hitPoint);
			float newSplitLength = Vector2.Distance(linePoints[1], hitPoint);

			// A new point in between (far away, enough to warrant a split in the rope)
			if (newRopeLength > ropeMinLength && newSplitLength > ropeSplitEpsilon)
			{
				Vector3[] newPoints = new Vector3[linePoints.Length + 1];

				newPoints[0] = linePoints[0]; // player
				newPoints[1] = hitPoint;        // new anchor
				newPoints[2] = linePoints[1];   // last anchor

				for (int i = 2; i < linePoints.Length; i++)
				{
					newPoints[i + 1] = linePoints[i];
				}

				linePoints = newPoints;

				hinge.connectedAnchor = linePoints[1];
				hinge.distance = Vector3.Distance(Player.transform.position, linePoints[1]);
			}
		}
		else if (linePoints.Length > 2)
		{
			Vector2 lastDir = linePoints[2] - linePoints[0];
			hitLast = Physics2D.Raycast(Player.transform.position, lastDir, lastDir.magnitude - 0.1f, ropeLayerMask.value);

			Vector2 ab = linePoints[1] - linePoints[0];
			Vector2 bc = linePoints[2] - linePoints[1];
			Vector2 ac = linePoints[2] - linePoints[0];

			// We see both last points, maybe release one
			if (hitLast.collider == null && Mathf.Abs(ab.magnitude + bc.magnitude - ac.magnitude) < ropeSplitEpsilon)
			{
				Vector3[] newPoints = new Vector3[linePoints.Length - 1];

				newPoints[0] = linePoints[0]; // player
				newPoints[1] = linePoints[2];   // new anchor = lastAnchor

				for (int i = 2; i < linePoints.Length - 1; i++)
				{
					newPoints[i] = linePoints[i + 1];
				}

				linePoints = newPoints;

				hinge.connectedAnchor = linePoints[1];
				hinge.distance = Vector3.Distance(Player.transform.position, linePoints[1]);
			}
		}
	}

	private Vector2 GetRopeDir()
	{
		return linePoints[1] - linePoints[0];
	}

	public override void Exit()
	{
		Destroy(hinge);
		lineRenderer.startWidth = 0;
		lineRenderer.endWidth = 0;
		playerCollider.sharedMaterial = matStatic;
		resetRope();
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
