using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float jumpForce = 1;
	public float aimDistance = 1;
	public float aimSpeed = 10;

	Rigidbody2D rb;

	GameObject target;
	Vector2 aimDirection;
	Vector3 aimPosition;
	Vector2 aimTemp;

	DistanceJoint2D hinge;

	bool ropeActive = false;

	LineRenderer lineRenderer;
	Vector3[] linePoints;

	// Use this for initialization
	void Start ()
	{
		rb = GetComponent<Rigidbody2D> ();
		target = transform.GetChild (1).gameObject;

		lineRenderer = GetComponentInChildren<LineRenderer> ();

		linePoints = new Vector3[2];
		for (int i = 0; i < linePoints.Length; i++) {
			linePoints [i] = Vector3.one;
		}

		aimDirection = new Vector2 (1, 1).normalized;
		aimPosition = new Vector3 (1, 1, 0);
		target.transform.localPosition = aimPosition;
		target.transform.SetParent (null);
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

		if (ropeActive && Input.GetButton ("RopeIn")) {
			hinge.distance -= 0.2f;
		}

		if (ropeActive && Input.GetButton ("RopeOut")) {
			hinge.distance += 0.2f;
		}


		// update aim target
		aimPosition.Set (transform.position.x + (aimDirection.x * aimDistance),
			transform.position.y + (aimDirection.y * aimDistance), 0);
		target.transform.position = aimPosition;

		linePoints [0] = transform.position;

		lineRenderer.SetPositions (linePoints);

		// Jump (testing)
		if (Input.GetButtonDown ("Jump")) {
			if (ropeActive) {
				ropeActive = false;
				Destroy (hinge);
			} else {
				ropeActive = true;
				RaycastHit2D hit;
				hit = Physics2D.Raycast (aimPosition, aimDirection);

				if (hit.collider != null) {
					GameObject.Find ("rayTarget").transform.position = hit.point;
					linePoints [1].x = hit.point.x;
					linePoints [1].y = hit.point.y;
				}
					
				hinge = gameObject.AddComponent (typeof(DistanceJoint2D)) as DistanceJoint2D;
				hinge.enableCollision = true;
				hinge.autoConfigureConnectedAnchor = false;
				hinge.autoConfigureDistance = false;
				hinge.distance = Vector3.Distance (transform.position, linePoints [1]);
			}
		}

		if (ropeActive) {
			hinge.connectedAnchor = linePoints [1];
		}
	}
}
