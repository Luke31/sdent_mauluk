using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IGameControlTarget : IEventSystemHandler
{
	void AimLeft(float inputForce);
	void AimRight(float inputForce);
	void RopeIn(float inputForce);
	void RopeOut(float inputForce);
	void Jump();
}