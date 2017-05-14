using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDamage : MonoBehaviour
{
    [Header("Health")]
    [Tooltip("Number of hits needed to destroy this object")]
    public double Health = 25;

	private double _initHealth;

	private double _otherThanRedRatio = 0.7f;

    [Header("Trigger damage tag filter")]
    [Tooltip("Only objects with this tag trigger damage")]
    public string TagNameFilter = "Player";

    void Start ()
    {
		_initHealth = Health;
		UpdateColor();
	}

	private void UpdateColor()
	{
		foreach (Transform child in transform)
		{
			Renderer rend = child.GetComponent<Renderer>();
			rend.material.shader = Shader.Find("Standard"); //Diffuse/ Specular
			float healthColor = (float) (Health / _initHealth);
			rend.material.SetColor("_Color", new Color(1f, healthColor, healthColor));
		}
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
	        UpdateColor();
        }
	}
}
