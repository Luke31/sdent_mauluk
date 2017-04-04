using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeExpandingBehaviour : PlayerBehaviour
{
	private RopeRenderer _ropeRenderer;
	public float RopeShootSpeed = 2f;

	//private Vector3[] LinePoints
	//{
	//	get { return new[] { _originPos, _ropeEnd }; }
	//}
	private Vector3 _originPos;
	private Vector3 _ropeDir;
	private Vector3 _ropeEnd;

	public RopeExpandingBehaviour(GamePhysics p, PlayerMovement c) : base(p, c)
	{
	}

	public override void Enter()
	{
		Physics.Player = GameObject.Find("Player");
		Physics.Target = GameObject.Find("Target");
		_ropeDir = (Physics.Target.transform.position - Physics.Player.transform.position).normalized;
		_ropeEnd = Physics.Target.transform.position;

		int layerMask = LayerMask.GetMask("Player");
		layerMask = ~layerMask;

		_ropeRenderer = new RopeRenderer(Physics.Player.GetComponentInChildren<LineRenderer>(), Physics.ropeWidth, Physics.ropeMinLength, layerMask);
		_ropeRenderer.ResetRope(Physics.linePoints);

		_ropeRenderer.GetHitPoint(_originPos, _ropeDir);
	}

	public override void Update()
	{
		_originPos = Physics.Player.transform.position;
		Physics.linePoints[0] = _originPos;
		var hitPoint = _ropeRenderer.GetHitPoint(_originPos, _ropeDir);
		Plane hitPlane = new Plane(_ropeDir, hitPoint);

		if (!hitPlane.GetSide(_ropeEnd))
		{
			_ropeEnd += _ropeDir * RopeShootSpeed;
			Physics.linePoints[1] = _ropeEnd;
			_ropeRenderer.Update(Physics.linePoints);
		}
		else
		{
			Physics.linePoints[1].x = hitPoint.x;
			Physics.linePoints[1].y = hitPoint.y;
			ActivateHinge();
			Context.SetState(GameStates.Active);
		}
	}

	private void ActivateHinge()
	{
		Physics.Hinge = Physics.Player.AddComponent(typeof(DistanceJoint2D)) as DistanceJoint2D;
		Physics.Hinge.enableCollision = true;
		Physics.Hinge.autoConfigureConnectedAnchor = false;
		Physics.Hinge.autoConfigureDistance = false;
		Physics.Hinge.connectedAnchor = Physics.linePoints[1];
		Physics.Hinge.distance = Vector3.Distance(Physics.Player.transform.position, Physics.linePoints[1]);
	}

	public override void Exit()
	{
		_ropeRenderer.ResetRope(Physics.linePoints);
	}

	public override void AimLeft(float inputForce)
	{
		// None
	}

	public override void AimRight(float inputForce)
	{
		// None
	}

	public override void RopeIn(float inputForce)
	{
		// None
	}

	public override void RopeOut(float inputForce)
	{
		// None
	}

	public override void Jump()
	{
		Context.SetState(GameStates.Inactive);
	}

	public override void FixedUpdate()
	{
		//None
	}
}
