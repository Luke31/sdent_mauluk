﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeExpandingBehaviour : PlayerBehaviour
{
	public float RopeShootSpeed = 2f;

	private Vector3 _ropeDir;
	private Vector3 _ropeEnd;
	private float _ropeLength;

	public RopeExpandingBehaviour(GamePhysics p, PlayerMovement c) : base(p, c)
	{
	}

	public override void Enter()
	{
		Physics._ropeRenderer.ResetRope(Physics.linePoints);
		Physics.Player = GameObject.Find("Player");
		Physics.Target = GameObject.Find("Target");
		_ropeDir = (Physics.Target.transform.position - Physics.Player.transform.position).normalized;
		_ropeEnd = Physics.Target.transform.position;
		_ropeLength = (_ropeEnd - Physics.Player.transform.position).magnitude;
		Physics._ropeRenderer.GetHitPoint(Physics.Player.transform.position, _ropeDir);
	}

	public override void Update()
	{
		Physics.linePoints[0] = Physics.Player.transform.position;
		Physics._ropeRenderer.Update(Physics.linePoints);
	}

	public override void FixedUpdate()
	{
		Vector3 _originPos = Physics.Player.transform.position;
		Physics.linePoints[0] = _originPos;
		var hitPoint = Physics._ropeRenderer.GetHitPoint(_originPos, _ropeDir);
		var dist = (hitPoint - _originPos).magnitude;

		if (_ropeLength <= dist)
		{
			_ropeLength += RopeShootSpeed;
			_ropeEnd = Physics.Player.transform.position + _ropeDir * _ropeLength;
			Physics.linePoints[1] = _ropeEnd;
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
	{}

	public override void Jump()
	{
		Context.SetState(GameStates.Collapsing);
	}

	public override void AimLeft(float inputForce)
	{}

	public override void AimRight(float inputForce)
	{}

	public override void RopeIn(float inputForce)
	{}

	public override void RopeOut(float inputForce)
	{}

	

	
}
