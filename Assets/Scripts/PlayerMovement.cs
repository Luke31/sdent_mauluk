using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IGameControlTarget
{
	public GameState gameState;

	private Animator animator;
	private PlayerBehaviour state;
	
	
	// Use this for initialization
	void Start ()
	{	
		animator = GetComponent<Animator>();
    	SetState(new RopeInactiveBehaviour);
	}

  public void SetState(PlayerBehaviour newState){
    state?.Exit();
    state = newState;
    state.Enter();
  }
	

	void FixedUpdate()
	{
		state.FixedUpdate();
	}
	
	// Update is called once per frame
	void Update ()
	{
		state.Update();
	}
	
	public void AimLeft(float inputForce)
	{   
    state.AimLeft(inputForce);
	}

	public void AimRight(float inputForce)
	{
    state.AimRight(inputForce);
	}

	public void RopeIn(float inputForce)
	{
    state.RopeIn(inputForce);
	}

	public void RopeOut(float inputForce)
	{
		state.RopeOut(inputForce);
	}

	public void Jump()
	{
    state.Jump();
	}
		
	void OnTriggerEnter2D(Collider2D other) {
		gameState.state = GameState.State.Finished;
	}

	
}
