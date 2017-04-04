using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class RopeCollapsingBehaviour : PlayerBehaviour
{
	public RopeCollapsingBehaviour(GamePhysics p, PlayerMovement c) : base(p, c)
	{
	}

	public override void AimLeft(float inputForce)
	{
		//None
	}

	public override void AimRight(float inputForce)
	{
		//None
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
	{
		//None
	}

	public override void Jump()
	{
		//None
	}

	public override void RopeIn(float inputForce)
	{
		//None
	}

	public override void RopeOut(float inputForce)
	{
		//None
	}

	public override void Update()
	{
		//None
	}
}
