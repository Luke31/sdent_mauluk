using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IGameControlTarget : IEventSystemHandler
{
	void AimLeft();
	void AimRight();
	void RopeIn();
	void RopeOut();
	void Jump();
}