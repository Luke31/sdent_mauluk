using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDamage : MonoBehaviour
{
    [Header("Health")]
    [Tooltip("Number of hits needed to destroy this object")]
    public double Health = 100;

    [Header("Trigger damage tag filter")]
    [Tooltip("Only objects with this tag trigger damage")]
    public string TagNameFilter = "Player";

    private ParticleSystem _psystem;

    void Start ()
	{
	}
	
	void Update () {
        if (Health <= 0) { 
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == TagNameFilter) { 
            Health -= coll.relativeVelocity.magnitude; //TODO: Include mass for impact
        }
    }
}
