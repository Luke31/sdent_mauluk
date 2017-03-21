using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class RopeExpandingBehaviour : PlayerBehaviour {
	private RopeRenderer _ropeRenderer;
	public float RopeShootSpeed = 2f;

	private Vector3[] LinePoints
	{
		get { return new[] { _originPos, _ropeEnd }; }
	}
	private Vector3 _originPos;
	private Vector3 _ropeDir;
	private Vector3 _ropeEnd;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Player = GameObject.Find("Player");
		Target = GameObject.Find("Target");
		_ropeDir = (Target.transform.position - Player.transform.position).normalized;
		_ropeEnd = Target.transform.position;

		int layerMask = LayerMask.GetMask("Player");
		layerMask = ~layerMask;
		
		_ropeRenderer = new RopeRenderer(Player.GetComponentInChildren<LineRenderer>(),
			animator.GetFloat("RopeWidth"), animator.GetFloat("RopeMinLength"),
			layerMask);
		_ropeRenderer.ResetRope(LinePoints);
		
		_ropeRenderer.GetHitPoint(_originPos, _ropeDir);
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		_originPos = Player.transform.position;
		var hitPoint = _ropeRenderer.GetHitPoint(_originPos, _ropeDir);
		Plane hitPlane = new Plane(_ropeDir, hitPoint);
		
		if (!hitPlane.GetSide(_ropeEnd))
		{
			_ropeEnd += _ropeDir * RopeShootSpeed;
			_ropeRenderer.Update(LinePoints);
		}
		else
		{
			animator.SetFloat("InitHingeX", hitPoint.x);
			animator.SetFloat("InitHingeY", hitPoint.y);
			animator.SetTrigger("RopeHit");
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		_ropeRenderer.ResetRope(LinePoints);
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
