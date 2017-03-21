using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IGameControlTarget
{
	private Animator animator;
	
	
	// Use this for initialization
	void Start ()
	{	
		animator = GetComponent<Animator>();
	}

	

	void FixedUpdate()
	{
		// check for splits in the rope

	}
	
	// Update is called once per frame
	void Update ()
	{
		
		
	}

	

	
	public void AimLeft(float inputForce)
	{
		animator.SetFloat("HorizontalForce", -inputForce);
	}

	public void AimRight(float inputForce)
	{
		animator.SetFloat("HorizontalForce", +inputForce);
	}

	public void RopeIn(float inputForce)
	{
		animator.SetFloat("VerticalForce", -inputForce);
	}

	public void RopeOut(float inputForce)
	{
		animator.SetFloat("VerticalForce", +inputForce);
	}

	public void Jump()
	{
		animator.SetTrigger("Jump");
	}

	
}
