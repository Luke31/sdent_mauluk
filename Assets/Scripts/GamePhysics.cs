﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
	public class GamePhysics
	{
		public GameObject Player;
		public GameObject Target;
		public GameObject HingeObject;
		public DistanceJoint2D Hinge;


		public float ropeWidth = 0.2f;
		public float ropeSplitEpsilon = 0.2f;

		public float ropeFeedSpeed = 0.5f;
		public float ropeSwingForce = 0.6f;
		public float ropeMinLength = 1f;

		public Rigidbody2D rb;

		public CircleCollider2D playerCollider;

		public PhysicsMaterial2D matBouncy;
		public PhysicsMaterial2D matStatic;


		public RopeRenderer _ropeRenderer;

		public int layerMask;

		public Vector3[] linePoints = new Vector3[2];
		public Vector2 initHinge;
		public Vector2 ropeDir;
	}
}