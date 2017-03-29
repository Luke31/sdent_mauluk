using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeExpandingBehaviour : PlayerBehaviour {
	private RopeRenderer _ropeRenderer;
	public float RopeShootSpeed = 2f;
  public float ropeMinLength = 1f;
  public float ropeWidth = 0.2f;

	private Vector3[] LinePoints
	{
		get { return new[] { _originPos, _ropeEnd }; }
	}
	private Vector3 _originPos;
	private Vector3 _ropeDir;
	private Vector3 _ropeEnd;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	public override void Enter()
	{
		Player = GameObject.Find("Player");

		int layerMask = LayerMask.GetMask("Player");
		layerMask = ~layerMask;
		
		_ropeRenderer = new RopeRenderer(Player.GetComponentInChildren<LineRenderer>(), ropeWidth,ropeMinLength, layerMask);
		_ropeRenderer.ResetRope(LinePoints);
		
		_ropeRenderer.GetHitPoint(_originPos, _ropeDir);
	}

	public override void Update()
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
      //TODO: Set State Rope Active
		}
	}
  
  private void ActivateHinge(){
    Hinge = Player.AddComponent(typeof(DistanceJoint2D)) as DistanceJoint2D;
		Hinge.enableCollision = true;
		Hinge.autoConfigureConnectedAnchor = false;
		Hinge.autoConfigureDistance = false;
		Hinge.connectedAnchor = linePoints[1];
		Hinge.distance = Vector3.Distance(Player.transform.position, linePoints[1]);
  }

	public override void Exit()
	{
		_ropeRenderer.ResetRope(LinePoints);
	}

  public override void AimLeft(float inputForce)
	{    
		// None
	}

	public override void AimRight(float inputForce)
	{
		// None
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
		//TODO: Set State Rope Inactive
	}
}
