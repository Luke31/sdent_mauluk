using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class RopeActiveBehaviour : PlayerBehaviour {

	public float ropeWidth = 0.2f;
	public float ropeSplitEpsilon = 0.2f;
	
	public float ropeFeedSpeed = 0.5f;
	public float ropeSwingForce = 0.6f;
	public float ropeMinLength = 1f;

	Rigidbody2D rb;

	CircleCollider2D playerCollider;

	PhysicsMaterial2D matBouncy;
	PhysicsMaterial2D matStatic;

	DistanceJoint2D hinge;
	private RopeRenderer _ropeRenderer;

	int layerMask;
	
	Vector3[] linePoints;
	Vector2 initHinge;
	private Vector2 ropeDir;


	void resetRope()
	{
		linePoints = new Vector3[2];
		for (int i = 0; i < linePoints.Length; i++)
		{
			linePoints[i] = Vector3.zero;
		}
		
		_ropeRenderer.ResetRope(linePoints);
	}

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Player = GameObject.Find("Player");
		Target = GameObject.Find("Target");
		initHinge.x = animator.GetFloat("InitHingeX");
		initHinge.y = animator.GetFloat("InitHingeY");
		//ropeDir = initRopeDir;

		layerMask = LayerMask.GetMask("Player");
		layerMask = ~layerMask;
		
		_ropeRenderer = new RopeRenderer(Player.GetComponentInChildren<LineRenderer>(), 
			animator.GetFloat("RopeWidth"), animator.GetFloat("RopeMinLength"), 
			layerMask);
		
		playerCollider = Player.GetComponentInChildren<CircleCollider2D>();

		matBouncy = (PhysicsMaterial2D)Resources.Load("PlayerBouncy");
		matStatic = (PhysicsMaterial2D)Resources.Load("PlayerStatic");

		resetRope();

		rb = Player.GetComponent<Rigidbody2D>();

		//Vector3 hitPoint = _ropeRenderer.GetHitPoint(Player.transform.position, initRopeDir);
		//VhitPont = initHinge;
		ActivateHinge(initHinge);
	}

	private void ActivateHinge(Vector3 hitPoint)
	{
		linePoints[1].x = hitPoint.x;
		linePoints[1].y = hitPoint.y;

		hinge = Player.AddComponent(typeof(DistanceJoint2D)) as DistanceJoint2D;
		hinge.enableCollision = true;
		hinge.autoConfigureConnectedAnchor = false;
		hinge.autoConfigureDistance = false;
		hinge.connectedAnchor = linePoints[1];
		hinge.distance = Vector3.Distance(Player.transform.position, linePoints[1]);
	}


	private Vector2 GetLeftForce()
	{
		return -Vector3.Cross(GetRopeDir(), Vector3.forward).normalized * ropeSwingForce;
	}

	private Vector2 GetRightForce()
	{
		return -GetLeftForce();
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		
		//Horizontal
		var inputForceHor = animator.GetFloat("HorizontalForce");
		if (inputForceHor < 0)
		{
			//Left 
			rb.AddForce(GetLeftForce() * Math.Abs(inputForceHor));
		}else if(inputForceHor > 0) { //Right
			rb.AddForce(GetRightForce() * Math.Abs(inputForceHor));
		}
		animator.SetFloat("HorizontalForce", 0);

		//Vertical Rope
		var inputForceRope = animator.GetFloat("VerticalForce");
		if (inputForceRope < 0)
		{
			//In
			RaycastHit2D hitFeed = Physics2D.Raycast(Player.transform.position, ropeDir, ropeFeedSpeed * 3, layerMask);
			if (hitFeed.collider == null && hinge.distance - ropeFeedSpeed > ropeMinLength)
			{
				hinge.distance -= ropeFeedSpeed * Math.Abs(inputForceRope);
			}
		}
		else if (inputForceRope > 0)
		{ //Out
			RaycastHit2D hitFeed = Physics2D.Raycast(Player.transform.position, -ropeDir, ropeFeedSpeed * 3, layerMask);
			if (hitFeed.collider == null)
			{
				hinge.distance += ropeFeedSpeed * Math.Abs(inputForceRope);
			}
		}
		animator.SetFloat("VerticalForce", 0);

		
		FixedUpdate();
		Update();
	}

	private void Update()
	{
		linePoints[0] = Player.transform.position;

		Vector3[] invertedLinePoints = new Vector3[linePoints.Length];
		for (int i = 0; i < linePoints.Length; i++)
		{
			invertedLinePoints[i] = linePoints[linePoints.Length - 1 - i];
		}

		_ropeRenderer.Update(invertedLinePoints);
	}

	private void FixedUpdate()
	{
		linePoints[0] = Player.transform.position;
		ropeDir = linePoints[1] - linePoints[0];

		RaycastHit2D hitSplit;
		RaycastHit2D hitLast;

		hitSplit = Physics2D.Raycast(Player.transform.position, ropeDir, ropeDir.magnitude - 0.1f, layerMask);

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
			hitLast = Physics2D.Raycast(Player.transform.position, lastDir, lastDir.magnitude - 0.1f, layerMask);

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

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Destroy(hinge);
		//lineRenderer.startWidth = 0;
		//lineRenderer.endWidth = 0;
		playerCollider.sharedMaterial = matStatic;
		resetRope();
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

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
