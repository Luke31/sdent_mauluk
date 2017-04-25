using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

		switch (_state) {
		case State.Paused:
			Pause();
			break;
		case State.Start:
			state = State.Running;
			Continue();
			break;
		case State.Running: 
			_timer += Time.deltaTime;
			break;
		case State.Finished:
			Pause();
			SceneManager.LoadScene("dev_fmauro", LoadSceneMode.Single); //finished
			break;
		case State.Dead:
			Pause();
			break;
		}
	}

	private void Pause()
	{
		Time.timeScale = 0; //Time.realtimeSinceStartup not affected!
	}

	private void Continue()
	{
		Time.timeScale = 1; //Time.realtimeSinceStartup not affected!
	}

}
