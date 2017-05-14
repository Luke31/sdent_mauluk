using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuScript : MonoBehaviour {
	public GameState gameState;

	public GameObject menu;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		menu.SetActive(gameState.state == GameState.State.Paused);
	}

	public void Quit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	public void ContinueGame()
	{
		gameState.state = GameState.State.Start;
	}

	public void RestartLevel()
	{
		gameState.state = GameState.State.Dead;
	}
}
