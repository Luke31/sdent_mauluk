using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInput : MonoBehaviour
{
	private Dictionary<int, TouchState> touch = new Dictionary<int, TouchState>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		Touch a = Input.GetTouch(0);

	}
}

abstract class TouchState
{
	public abstract void Update();
}

class TouchLeft : TouchState
{
	public override void Update()
	{
		
		throw new NotImplementedException();
	}
}

class TouchRight : TouchState
{
	public override void Update()
	{
		throw new NotImplementedException();
	}
}


