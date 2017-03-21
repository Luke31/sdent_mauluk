﻿using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class RopeExpandingBehaviour : PlayerBehaviour {
	private RopeRenderer _ropeRenderer;
	public float RopeShootSpeed = 0.5f;

	private Vector3[] linePoints
	{
		get { return new[] { originPos, ropeEnd }; }
	}
	private Vector3[] invertedLinePoints
	{
		get { return new[] { ropeEnd, originPos }; }
	}
	private Vector3 originPos;
	private Vector3 ropeEnd;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Player = GameObject.Find("Player");
		Target = GameObject.Find("Target");
		int layerMask = LayerMask.GetMask("Player");
		layerMask = ~layerMask;
		
		_ropeRenderer = new RopeRenderer(Player.GetComponentInChildren<LineRenderer>(),
			animator.GetFloat("RopeWidth"), animator.GetFloat("RopeMinLength"),
			layerMask);
		_ropeRenderer.ResetRope(linePoints);
		ropeEnd = Target.transform.position;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		var hitPoint = _ropeRenderer.GetHitPoint(Player.transform.position, Target.transform.position);
		originPos = Player.transform.position;

		if (Vector3.Distance(ropeEnd, hitPoint) > 5.0)
		{
			ropeEnd += (hitPoint - originPos).normalized * RopeShootSpeed;
			_ropeRenderer.Update(linePoints);
		}
		else
		{
			animator.SetTrigger("RopeHit");
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		_ropeRenderer.ResetRope(linePoints);
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
