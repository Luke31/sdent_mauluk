using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public enum GameStates
{
	Inactive,
	Expanding,
	Active,
	Collapsing
}

public class PlayerMovement : MonoBehaviour, IGameControlTarget
{
	public GameState gameState;
	private PlayerBehaviour _state;
	private readonly GamePhysics _physics = new GamePhysics();

	private readonly Dictionary<GameStates, PlayerBehaviour> _states = new Dictionary<GameStates, PlayerBehaviour>();

	// Use this for initialization
	void Start()
	{
		_physics.Player = GameObject.Find("Player");
		_physics.Target = GameObject.Find("Target");
		_physics.HingeObject = GameObject.Find("Hinge");
		_states.Add(GameStates.Inactive, new RopeInactiveBehaviour(_physics, this));
		_states.Add(GameStates.Expanding, new RopeExpandingBehaviour(_physics, this));
		_states.Add(GameStates.Active, new RopeActiveBehaviour(_physics, this));
		_states.Add(GameStates.Collapsing, new RopeCollapsingBehaviour(_physics, this));
		SetState(GameStates.Inactive);
	}

	public void SetState(GameStates newState)
	{
		if(_state != null) _state.Exit();
		_state = _states[newState];
		_state.Enter();
	}


	void FixedUpdate()
	{
		_state.FixedUpdate();
	}

	// Update is called once per frame
	void Update()
	{
		_state.Update();
	}

	public void AimLeft(float inputForce)
	{
		_state.AimLeft(inputForce);
	}

	public void AimRight(float inputForce)
	{
		_state.AimRight(inputForce);
	}

	public void RopeIn(float inputForce)
	{
		_state.RopeIn(inputForce);
	}

	public void RopeOut(float inputForce)
	{
		_state.RopeOut(inputForce);
	}

	public void Jump()
	{
		_state.Jump();
	}
		
	void OnTriggerEnter2D(Collider2D other) {
		gameState.state = GameState.State.Finished;
	}


}
