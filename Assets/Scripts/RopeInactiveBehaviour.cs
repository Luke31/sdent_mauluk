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

	override public void Enter()
	{
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
  
  public override void Update(){
    UpdateAimTarget();
  }

	override public void Exit()
	{
		Target.GetComponent<MeshRenderer>().enabled = false;
	}
  
  public override void AimLeft(float inputForce)
	{    
		aimTemp = Quaternion.AngleAxis(aimSpeed * Mathf.Abs(inputForce), Vector3.forward) * aimDirection;
    if (Vector2.Angle(aimTemp, Vector2.up) < aimMaxAngle)
    {
      aimDirection.Set(aimTemp.x, aimTemp.y);
      aimDirection.Normalize();
    }
	}

	public override void AimRight(float inputForce)
	{
		aimTemp = Quaternion.AngleAxis(-aimSpeed * Mathf.Abs(inputForce), Vector3.forward) * aimDirection;
    if (Vector2.Angle(aimTemp, Vector2.up) < aimMaxAngle)
    {
      aimDirection.Set(aimTemp.x, aimTemp.y);
      aimDirection.Normalize();
    }
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
    //TODO: Set State Rope Expanding
	}
}
