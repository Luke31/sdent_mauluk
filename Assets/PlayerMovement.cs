using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

	public int maxJumps = 2;
	public float jumpForce = 1;
	public float aimDistance = 1;
	public float aimSpeed = 10;

	Rigidbody2D rb;
	int jumps;

	GameObject target;
	Vector2 aimDirection;
	Vector3 aimPosition;
	Vector2 aimTemp;

	bool castRay = false;


	// Use this for initialization
	void Start ()
	{
		rb = GetComponent<Rigidbody2D> ();
		jumps = 0;
		target = transform.GetChild (1).gameObject;
		aimDirection = new Vector2 (1, 1).normalized;
		aimPosition = new Vector3 (1, 1, 0);
		target.transform.localPosition = aimPosition;
		target.transform.SetParent (null);
	}

	void FixedUpdate ()
	{
		if (castRay) {

			RaycastHit2D hit;
			hit = Physics2D.Raycast (aimPosition, aimDirection);

			if (hit.collider != null) {
				GameObject.Find ("rayTarget").transform.position = hit.point;
			}

			castRay = false;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Update Aim
		if (Input.GetButton ("AimLeft")) {
			aimTemp = Quaternion.AngleAxis (aimSpeed, Vector3.forward) * aimDirection;
			aimDirection.Set (aimTemp.x, aimTemp.y);
			aimDirection.Normalize ();
		}

		if (Input.GetButton ("AimRight")) {
			aimTemp = Quaternion.AngleAxis (-aimSpeed, Vector3.forward) * aimDirection;
			aimDirection.Set (aimTemp.x, aimTemp.y);
			aimDirection.Normalize ();
		}

		// Jump (testing)
		if (jumps < maxJumps && Input.GetButtonDown ("Jump")) {
			rb.AddForce (aimDirection * jumpForce, ForceMode2D.Impulse);
			jumps++;
			castRay = true;
		}

		// update aim target
		aimPosition.Set (transform.position.x + (aimDirection.x * aimDistance),
			transform.position.y + (aimDirection.y * aimDistance), 0);
		target.transform.position = aimPosition;
	}

	void OnCollisionEnter2D (Collision2D collision)
	{
		jumps = 0;
	}


}
