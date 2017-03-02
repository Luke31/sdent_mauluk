using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInput : MonoBehaviour
{
	private readonly Dictionary<int, TouchState> touches = new Dictionary<int, TouchState>();
	public float inputForceMinThreshold = 0.01f;
	public float sensitivity = 6f;

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
			if (t.position.x < 2.0 / 3.0 * Screen.width)
			{
				touches[t.fingerId] = new TouchLeftArea (t.position, inputForceMinThreshold);
			}
			else
			{
				touches[t.fingerId] = new TouchRightArea(t.position, inputForceMinThreshold);
			}
		}

		//Behaviour depending on state/place on screen
		touches[t.fingerId].Update(this.gameObject, t, sensitivity);

		if (t.phase == TouchPhase.Ended)
		{
			touches.Remove(t.fingerId);
		}
	}

	abstract class TouchState
	{
		protected readonly Vector2 initPos;
		protected readonly float inputForceMinThreshold;

		protected TouchState(Vector2 initPos, float inputForceMinThreshold)
		{
			this.initPos = initPos;
			this.inputForceMinThreshold = inputForceMinThreshold;
		}

		public abstract void Update(GameObject player, Touch t, float sensitivity);
	}

	private class TouchLeftArea : TouchState
	{
		public override void Update(GameObject player, Touch t, float sensitivity)
		{
			if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
			{
				Vector2 deltaInitPosition = t.position - initPos; //px
				deltaInitPosition.Scale(new Vector2(1.0f / Screen.width, 1.0f / Screen.height)); //[0,1]
				deltaInitPosition.Scale(new Vector2(sensitivity, sensitivity));

				float powForceX = deltaInitPosition.x * deltaInitPosition.x;
				float powForceY = deltaInitPosition.y * deltaInitPosition.y;
				if (deltaInitPosition.x > inputForceMinThreshold)
				{
					ExecuteEvents.Execute<IGameControlTarget>(target: player, eventData: null, functor: (x, y) => x.AimRight(powForceX));
				}
				else if (deltaInitPosition.x < -inputForceMinThreshold)
				{
					ExecuteEvents.Execute<IGameControlTarget>(target: player, eventData: null, functor: (x, y) => x.AimLeft(powForceX));
				}
				if (deltaInitPosition.y > inputForceMinThreshold)
				{
					ExecuteEvents.Execute<IGameControlTarget>(target: player, eventData: null, functor: (x, y) => x.RopeIn(powForceY));
				}
				else if (deltaInitPosition.y < -inputForceMinThreshold)
				{
					ExecuteEvents.Execute<IGameControlTarget>(target: player, eventData: null, functor: (x, y) => x.RopeOut(powForceY));
				}
			}
		}

		public TouchLeftArea(Vector2 initPos, float inputForceMinThreshold) : base(initPos, inputForceMinThreshold)
		{
		}
	}

	private class TouchRightArea : TouchState
	{
		public override void Update(GameObject player, Touch t, float sensitivity)
		{
			if(t.phase == TouchPhase.Ended) { 
				ExecuteEvents.Execute<IGameControlTarget>(target: player, eventData: null, functor: (x, y) => x.Jump());
			}

		}

		public TouchRightArea(Vector2 initPos, float inputForceMinThreshold) : base(initPos, inputForceMinThreshold)
		{
		}
	}

}



