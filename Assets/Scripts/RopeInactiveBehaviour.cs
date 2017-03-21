using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeInactiveBehaviour : PlayerBehaviour {

	public float aimDistance = 4;
	public float aimSpeed = 3;
	public float aimMaxAngle = 90f;
	
	Vector2 aimDirection;
	Vector3 aimPosition;
	Vector2 aimTemp;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Player = GameObject.Find("Player");
		Target = GameObject.Find("Target");

		if(aimDirection == Vector2.zero) { 
			aimDirection = new Vector2(1, 1).normalized;
			aimPosition = (Vector2)Player.transform.position + aimDirection * aimDistance;
			Target.transform.position = aimPosition;
		}
		UpdateAimTarget();
		Target.GetComponent<MeshRenderer>().enabled = true;
	}

	private void UpdateAimTarget()
	{
		// update aim target
		aimPosition.Set(Player.transform.position.x + (aimDirection.x * aimDistance),
			Player.transform.position.y + (aimDirection.y * aimDistance), 0);
		Target.transform.position = aimPosition;
	}

	//OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		UpdateAimTarget();

		var inputForce = animator.GetFloat("HorizontalForce");
		if (inputForce < 0) { //Left 
			aimTemp = Quaternion.AngleAxis(aimSpeed * Mathf.Abs(inputForce), Vector3.forward) * aimDirection;
			if (Vector2.Angle(aimTemp, Vector2.up) < aimMaxAngle)
			{
				aimDirection.Set(aimTemp.x, aimTemp.y);
				aimDirection.Normalize();
			}
		}else if (inputForce > 0)
		{ //Right
			aimTemp = Quaternion.AngleAxis(-aimSpeed * Mathf.Abs(inputForce), Vector3.forward) * aimDirection;

			if (Vector2.Angle(aimTemp, Vector2.up) < aimMaxAngle)
			{
				aimDirection.Set(aimTemp.x, aimTemp.y);
				aimDirection.Normalize();
			}
		}
		animator.SetFloat("HorizontalForce", 0);
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Target.GetComponent<MeshRenderer>().enabled = false;
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
