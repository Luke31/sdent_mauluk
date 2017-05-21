using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class PlayerStateCollapsing : PlayerState
{
	public PlayerStateCollapsing(GamePhysics p, PlayerMovement c) : base(p, c)
	{
	}

	public override void Enter()
	{
		Context.SetState(GameStates.Inactive);
	}

	public override void Exit()
	{
		ResetRope();
	}

	void ResetRope()
	{
		Physics.linePoints = new Vector3[2];

		Physics._ropeRenderer.ResetRope(Physics.linePoints);
	}

	public override void FixedUpdate()
	{}

	public override void Jump()
	{}
	public override void AimShootAt(Vector2 direction)
	{}
	public override void AimLeft(float inputForce)
	{}
	public override void AimRight(float inputForce)
	{}
	public override void RopeIn(float inputForce)
	{}
	public override void RopeOut(float inputForce)
	{}
	public override void DirectionForce(float inputForce, Vector2 direction)
	{}
	public override void Update()
	{}
}
