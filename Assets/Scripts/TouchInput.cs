using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInput : MonoBehaviour
{
	private Dictionary<int, TouchState> touches = new Dictionary<int, TouchState>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.touchCount == 2)
		{
			Touch a = Input.GetTouch(0);
			
		}
		for (int i = 0; i < Input.touchCount; i++)
		{
			HandleTouch(Input.GetTouch(i));
		}
		
	}

	private void HandleTouch(Touch t)
	{
		if (!touches.ContainsKey(t.fingerId))
		{
			if (t.position.x < Screen.width/2.0)
			{
				touches[t.fingerId] = new TouchLeft();
			}
			else
			{
				touches[t.fingerId] = new TouchRight();
			}
		}

		touches[t.fingerId].Update(t);

		if (t.phase == TouchPhase.Ended)
		{
			touches.Remove(t.fingerId);
		}
	}

	abstract class TouchState
	{
		public abstract void Update(Touch t);
	}

	class TouchLeft : TouchState
	{
		public override void Update(Touch t)
		{
			if (t.deltaPosition.x > 0)
			{

			}
		}
	}

	class TouchRight : TouchState
	{
		public override void Update(Touch t)
		{
			throw new NotImplementedException();
		}
	}

}



