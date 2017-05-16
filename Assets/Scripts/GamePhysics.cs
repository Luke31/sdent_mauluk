using System;
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
		public int LayerMaskWithoutPlayer;

		public float ropeWidth = 0.18f;
		public float ropeSplitEpsilon = 0.2f;

		public float ropeFeedSpeed = 30f; //calc use * Time.deltaTime
		public float ropeSwingForce = 36f; //calc use * Time.deltaTime
		public float ropeMinLength = 1f;

		public Rigidbody2D rb;

		public CircleCollider2D playerCollider;

		public PhysicsMaterial2D matBouncy;
		public PhysicsMaterial2D matStatic;


		public RopeRenderer _ropeRenderer;

		public Vector3[] linePoints = new Vector3[2];
		public Vector2 initHinge;
		public Vector2 ropeDir;
	}
}
