﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts;
using UnityEngine;


public abstract class PlayerState : IGameControlTarget
{
	public abstract void Update();
	public abstract void FixedUpdate();
	public abstract void Enter();
	public abstract void Exit();
	public abstract void AimLeft(float inputForce);
	public abstract void AimRight(float inputForce);
	public abstract void RopeIn(float inputForce);
	public abstract void RopeOut(float inputForce);
	public abstract void DirectionForce(float inputForce, Vector2 direction);
	public abstract void AimShootAt(Vector2 direction);
	public abstract void Jump();

	protected GamePhysics Physics;
	protected PlayerMovement Context;

	protected PlayerState(GamePhysics p, PlayerMovement c)
	{
		Physics = p;
		Context = c;
	}
}

