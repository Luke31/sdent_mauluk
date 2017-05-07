using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyInput : MonoBehaviour
{
	public GameState gameState;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (gameState.state != GameState.State.Running)
			return;

		if (Input.GetButton("AimLeft"))
		{
			ExecuteEvents.Execute<IGameControlTarget>(target:this.gameObject, eventData:null, functor:(x, y) => x.AimLeft(1f)); 
		}
		if (Input.GetButton("AimRight"))
		{
			ExecuteEvents.Execute<IGameControlTarget>(target: this.gameObject, eventData: null, functor: (x, y) => x.AimRight(1f));
		}
		if (Input.GetButton("RopeIn"))
		{
			ExecuteEvents.Execute<IGameControlTarget>(target: this.gameObject, eventData: null, functor: (x, y) => x.RopeIn(1f));
		}
		if (Input.GetButton("RopeOut"))
		{
			ExecuteEvents.Execute<IGameControlTarget>(target: this.gameObject, eventData: null, functor: (x, y) => x.RopeOut(1f));
		}
		if (Input.GetButtonDown("Jump"))
		{
			ExecuteEvents.Execute<IGameControlTarget>(target: this.gameObject, eventData: null, functor: (x, y) => x.Jump());
		}
	}

}