using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyInput : MonoBehaviour
{
	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetButton("AimLeft"))
		{
			ExecuteEvents.Execute<IGameControlTarget>(target:this.gameObject, eventData:null, functor:(x, y) => x.AimLeft()); 
		}
		if (Input.GetButton("AimRight"))
		{
			ExecuteEvents.Execute<IGameControlTarget>(target: this.gameObject, eventData: null, functor: (x, y) => x.AimRight());
		}
		if (Input.GetButton("RopeIn"))
		{
			ExecuteEvents.Execute<IGameControlTarget>(target: this.gameObject, eventData: null, functor: (x, y) => x.RopeIn());
		}
		if (Input.GetButton("RopeOut"))
		{
			ExecuteEvents.Execute<IGameControlTarget>(target: this.gameObject, eventData: null, functor: (x, y) => x.RopeOut());
		}
		if (Input.GetButtonDown("Jump"))
		{
			ExecuteEvents.Execute<IGameControlTarget>(target: this.gameObject, eventData: null, functor: (x, y) => x.Jump());
		}
	}

}