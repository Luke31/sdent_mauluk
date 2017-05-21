using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class PlayerStateInactive : PlayerState
{

	public float aimDistance = 4;
	public float aimSpeed = 180; //calc use * Time.deltaTime
	public float aimMaxAngle = 90f;
	private bool _showTarget;

	Vector2 aimDirection;
	Vector3 aimPosition;
	Vector2 aimTemp;

	public PlayerStateInactive(GamePhysics p, PlayerMovement c) : base(p, c)
	{
	}

	public override void Enter()
	{
		if (aimDirection == Vector2.zero)
		{
			aimDirection = new Vector2(1, 1).normalized;
			aimPosition = (Vector2)Physics.Player.transform.position + aimDirection * aimDistance;
			Physics.Target.transform.position = aimPosition;
		}
		UpdateAimTarget();
		Physics.Target.GetComponent<MeshRenderer>().enabled = _showTarget;
	}

	private void UpdateAimTarget()
	{
		// update aim target
		aimPosition.Set(Physics.Player.transform.position.x + (aimDirection.x * aimDistance),
			Physics.Player.transform.position.y + (aimDirection.y * aimDistance), 0);
		Physics.Target.transform.position = aimPosition;
	}

	public override void Update()
	{
		UpdateAimTarget();
	}

	public override void Exit()
	{
		Physics.Target.GetComponent<MeshRenderer>().enabled = false;
	}

	private void EnableShowTarget()
	{
		if (!_showTarget)
		{
			_showTarget = true;
			Physics.Target.GetComponent<MeshRenderer>().enabled = _showTarget;
		}
	}

	public override void AimLeft(float inputForce)
	{
		EnableShowTarget();

		aimTemp = Quaternion.AngleAxis(aimSpeed * Time.deltaTime * Mathf.Abs(inputForce), Vector3.forward) * aimDirection;
		if (Vector2.Angle(aimTemp, Vector2.up) < aimMaxAngle)
		{
			aimDirection.Set(aimTemp.x, aimTemp.y);
			aimDirection.Normalize();
		}
	}

	public override void AimRight(float inputForce)
	{
		EnableShowTarget();

		aimTemp = Quaternion.AngleAxis(-aimSpeed * Time.deltaTime * Mathf.Abs(inputForce), Vector3.forward) * aimDirection;
		if (Vector2.Angle(aimTemp, Vector2.up) < aimMaxAngle)
		{
			aimDirection.Set(aimTemp.x, aimTemp.y);
			aimDirection.Normalize();
		}
	}

	public override void DirectionForce(float inputForce, Vector2 direction)
	{
		// Aim here

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
		Context.SetState(GameStates.Expanding);
	}
	public override void AimShootAt(Vector2 direction)
	{
		if (Vector2.Angle(direction, Vector2.up) < aimMaxAngle)
		{
			aimDirection.Set(direction.x, direction.y);
			aimDirection.Normalize();
			UpdateAimTarget();
			Context.SetState(GameStates.Expanding);
		}
	}

	public override void FixedUpdate()
	{
		//None
	}
}
