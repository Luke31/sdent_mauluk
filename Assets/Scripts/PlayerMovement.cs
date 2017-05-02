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
    public LayerMask RopeLayerMask;
    private PlayerState _state;
	private readonly GamePhysics _physics = new GamePhysics();

	private readonly Dictionary<GameStates, PlayerState> _states = new Dictionary<GameStates, PlayerState>();

	// Use this for initialization
	void Start()
	{
		_physics.Player = GameObject.Find("Player");
		_physics.Target = GameObject.Find("Target");
		_physics.HingeObject = GameObject.Find("Hinge");
		_physics.LayerMaskWithoutPlayer = RopeLayerMask;
		_physics._ropeRenderer = new RopeRenderer(_physics.Player.GetComponentInChildren<LineRenderer>(), _physics.ropeWidth, _physics.ropeMinLength, _physics.LayerMaskWithoutPlayer);
		_physics.playerCollider = _physics.Player.GetComponentInChildren<CircleCollider2D>();

		_physics.matBouncy = (PhysicsMaterial2D)Resources.Load("PlayerBouncy");
		_physics.matStatic = (PhysicsMaterial2D)Resources.Load("PlayerStatic");

		_physics.rb = _physics.Player.GetComponent<Rigidbody2D>();

		_states.Add(GameStates.Inactive, new PlayerStateInactive(_physics, this));
		_states.Add(GameStates.Expanding, new PlayerStateExpanding(_physics, this));
		_states.Add(GameStates.Active, new PlayerStateActive(_physics, this));
		_states.Add(GameStates.Collapsing, new PlayerStateCollapsing(_physics, this));
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
		switch (other.tag) {
			case "Finish":
				gameState.state = GameState.State.Finished;
				break;
			case "Death":
				gameState.state = GameState.State.Dead;
				break;
		}
	}
}


