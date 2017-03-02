using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInput : MonoBehaviour
{
	private readonly Dictionary<int, TouchState> touches = new Dictionary<int, TouchState>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
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
				touches[t.fingerId] = new TouchLeftArea();
			}
			else
			{
				touches[t.fingerId] = new TouchRightArea();
			}
		}

		touches[t.fingerId].Update(this.gameObject, t);

		if (t.phase == TouchPhase.Ended)
		{
			touches.Remove(t.fingerId);
		}
	}

	abstract class TouchState
	{
		public virtual void Update(GameObject player, Touch t)
		{
			if (t.tapCount == 1)
			{
				ExecuteEvents.Execute<IGameControlTarget>(target: player, eventData: null, functor: (x, y) => x.Jump());
			}
		}
	}

	private class TouchLeftArea : TouchState
	{
		public override void Update(GameObject player, Touch t)
		{
			base.Update(player, t);

			if (t.deltaPosition.x > 0)
			{
				ExecuteEvents.Execute<IGameControlTarget>(target: player, eventData: null, functor: (x, y) => x.AimRight());
			}
			else
			{
				ExecuteEvents.Execute<IGameControlTarget>(target: player, eventData: null, functor: (x, y) => x.AimLeft());
			}
		}
	}

	private class TouchRightArea : TouchState
	{
		public override void Update(GameObject player, Touch t)
		{
			base.Update(player, t);

			if (t.deltaPosition.y > 0)
			{
				ExecuteEvents.Execute<IGameControlTarget>(target: player, eventData: null, functor: (x, y) => x.RopeIn());
			}
			else
			{
				ExecuteEvents.Execute<IGameControlTarget>(target: player, eventData: null, functor: (x, y) => x.RopeOut());
			}
		}
	}

}



