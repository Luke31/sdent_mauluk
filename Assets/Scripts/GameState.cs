﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour {
	public State initialState;
	public Text debugState;

	public State state {
		get {
			return _state;
		}
		set {
			_state = value;
		}
	}

	public enum State {
		Start,
		Running,
		Paused,
		Finished,
		Dead
	}

	private State _state;
	private double _timer;



	// Use this for initialization
	void Start () {
		state = initialState;
	}

	// Update is called once per frame
	void Update () {
		debugState.text = state.ToString () + "\n" + _timer.ToString();

		if (Input.GetKeyDown("p")) {
			if (state == State.Paused) {
				state = State.Running;
			}
			else if (state == State.Running) {
				state = State.Paused;
			}
    }

		switch (_state) {
		case State.Paused:
			Time.timeScale = 0;
		case State.Start:
			state = State.Running;
			break;
		case State.Running:
			if (Time.timeScale == 0) {
				Time.timeScale = 1;
			}
			_timer += Time.deltaTime;
			break;
		case State.Finished:
		case State.Dead:
			break;
		}
	}



}
