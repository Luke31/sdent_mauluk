using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInput : MonoBehaviour
{
	private readonly Dictionary<int, TouchState> touches = new Dictionary<int, TouchState>();
	public float inputForceMinThreshold = 0.01f;
	public float sensitivity = 3f;
	public GameState gameState;
	public GameObject player;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (gameState.state != GameState.State.Running)
			return;

		for (int i = 0; i < Input.touchCount; i++)
		{
			HandleTouch(Input.GetTouch(i));
		}
		
	}

	private void HandleTouch(Touch t)
	{
		if (!touches.ContainsKey(t.fingerId))
		{
			//if (t.position.x < 2.0 / 3.0 * Screen.width)
			//{
			touches[t.fingerId] = new TouchLeftArea (t.position, inputForceMinThreshold);
			//}
			//else
			//{
			//	touches[t.fingerId] = new TouchRightArea(t.position, inputForceMinThreshold);
			//}
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
		protected readonly Vector2 InitTouchPos;
		protected readonly float InputForceMinThreshold;

		protected TouchState(Vector2 initTouchPos, float inputForceMinThreshold)
		{
			InitTouchPos = initTouchPos;
			InputForceMinThreshold = inputForceMinThreshold;
		}

		public abstract void Update(GameObject player, Touch t, float sensitivity);
	}

	private class TouchLeftArea : TouchState
	{
		private TouchPhase _phasePrev;
		private float _time; //seconds? probably...
		public override void Update(GameObject player, Touch t, float sensitivity)
		{
			if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
			{
				//var centerScreen = new Vector2(Screen.width/2f, Screen.height/2f);
				//TODO: Threshold!
				//Vector2 playerPosition = player.transform.position;
				Vector2 deltaInitPosition = t.position - InitTouchPos; //px
				deltaInitPosition.Scale(new Vector2(1.0f / Screen.width, 1.0f / Screen.height)); //[0,1]
				deltaInitPosition.Scale(new Vector2(sensitivity, sensitivity));

				//float powForce = deltaInitPosition.magnitude * deltaInitPosition.magnitude;
				Debug.Log("Move");
				//Debug.Log(powForce);
				Debug.Log(deltaInitPosition.normalized);
				//float powForceY = deltaInitPosition.y * deltaInitPosition.y;
				float force = deltaInitPosition.magnitude;
				if (force > InputForceMinThreshold) { 
					ExecuteEvents.Execute<IGameControlTarget>(target: player, eventData: null, functor: (x, y) => x.DirectionForce(force, deltaInitPosition.normalized));
				}
			//if (deltaInitPosition.x > InputForceMinThreshold)
				//{
				//	ExecuteEvents.Execute<IGameControlTarget>(target: player, eventData: null, functor: (x, y) => x.AimRight(powForceX));
				//}
				//else if (deltaInitPosition.x < -InputForceMinThreshold)
				//{
				//	ExecuteEvents.Execute<IGameControlTarget>(target: player, eventData: null, functor: (x, y) => x.AimLeft(powForceX));
				//}
				//if (deltaInitPosition.y > InputForceMinThreshold)
				//{
				//	ExecuteEvents.Execute<IGameControlTarget>(target: player, eventData: null, functor: (x, y) => x.RopeIn(powForceY));
				//}
				//else if (deltaInitPosition.y < -InputForceMinThreshold)
				//{
				//	ExecuteEvents.Execute<IGameControlTarget>(target: player, eventData: null, functor: (x, y) => x.RopeOut(powForceY));
				//}
			}
			else if (t.phase == TouchPhase.Ended && t.tapCount == 1 && _time < 0.15) // && _phasePrev == TouchPhase.Began   sometimes TouchPhase.Moved, TouchPhase.Stationary
			{
				var centerScreen = new Vector2(Screen.width / 2f, Screen.height / 2f);
				Vector2 playerPosition = player.transform.position;
				Vector2 deltaPlayerPosition = t.position - centerScreen; //px
				Debug.Log("Tap");
				Debug.Log(deltaPlayerPosition.normalized);
				ExecuteEvents.Execute<IGameControlTarget>(target: player, eventData: null, functor: (x, y) => x.AimShootAt(deltaPlayerPosition.normalized));
			}
			if (t.phase == TouchPhase.Ended)
			{
				Debug.Log(_phasePrev);
				Debug.Log(_time);
			}
			_phasePrev = t.phase;
			_time += t.deltaTime;
		}

		public TouchLeftArea(Vector2 initTouchPos, float inputForceMinThreshold) : base(initTouchPos, inputForceMinThreshold)
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

		public TouchRightArea(Vector2 initTouchPos, float inputForceMinThreshold) : base(initTouchPos, inputForceMinThreshold)
		{
		}
	}

}



