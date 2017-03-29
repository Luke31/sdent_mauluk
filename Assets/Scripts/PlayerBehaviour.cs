using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public abstract class PlayerBehaviour : IGameControlTarget
{
  abstract void Update();
  abstract void FixedUpdate();
  abstract void Enter();
  abstract void Exit();
  
	protected GameObject Player;
	protected GameObject Target;
  protected GameObject Hinge;
  
  PlayerBehaviour(GameObject p, GameObject t, GameObject h){
    Player = p;
    Target = t;
    Hinge = h;
  }
}

