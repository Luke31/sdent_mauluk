using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float jumpForce = 1;
	public float aimDistance = 1;
	public float aimSpeed = 10;
	public float ropeWidth = 0.1f;
	public float ropeSplitEpsilon = 0.1f;

	Rigidbody2D rb;
	CircleCollider2D playerCollider;

	PhysicsMaterial2D matBouncy;
	PhysicsMaterial2D matStatic;

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
		target = GameObject.Find("target");

		lineRenderer = GetComponentInChildren<LineRenderer> ();
		playerCollider = GetComponentInChildren<CircleCollider2D> ();

		matBouncy = (PhysicsMaterial2D) Resources.Load ("PlayerBouncy");
		matStatic = (PhysicsMaterial2D) Resources.Load ("PlayerStatic");

		resetRope ();

		aimDirection = new Vector2 (1, 1).normalized;
		aimPosition = new Vector3 (1, 1, 0);
		target.transform.localPosition = aimPosition;
		target.transform.SetParent (null);
	}

	void resetRope() {
		linePoints = new Vector3[2];
		for (int i = 0; i < linePoints.Length; i++) {
			linePoints [i] = Vector3.zero;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Update Aim
		if (!ropeActive && Input.GetButton ("AimLeft")) {
			aimTemp = Quaternion.AngleAxis (aimSpeed, Vector3.forward) * aimDirection;
			aimDirection.Set (aimTemp.x, aimTemp.y);
			aimDirection.Normalize ();
		}

		if (!ropeActive && Input.GetButton ("AimRight")) {
			aimTemp = Quaternion.AngleAxis (-aimSpeed, Vector3.forward) * aimDirection;
			aimDirection.Set (aimTemp.x, aimTemp.y);
			aimDirection.Normalize ();
		}

		// Rope Swing
		if (ropeActive && Input.GetButton ("AimLeft")) {
			rb.AddForce (Vector2.left);
		}

		if (ropeActive && Input.GetButton ("AimRight")) {
			rb.AddForce (Vector2.right);
		}

		// Manage Rope
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

		// Jump (testing)
		if (Input.GetButtonDown ("Jump")) {
			if (ropeActive) {
				Destroy (hinge);
				ropeActive = false;
				target.GetComponent<MeshRenderer> ().enabled = true;
				lineRenderer.startWidth = 0;
				lineRenderer.endWidth = 0;
				playerCollider.sharedMaterial = matStatic;
				resetRope ();
			} else {
				RaycastHit2D hit;
				hit = Physics2D.Raycast (aimPosition, aimDirection);

				if (hit.collider != null) {
					GameObject.Find ("rayTarget").transform.position = hit.point;
					linePoints [1].x = hit.point.x;
					linePoints [1].y = hit.point.y;

					ropeActive = true;
					target.GetComponent<MeshRenderer> ().enabled = false;
					lineRenderer.startWidth = ropeWidth;
					lineRenderer.endWidth = ropeWidth;
					playerCollider.sharedMaterial = matBouncy;

					hinge = gameObject.AddComponent (typeof(DistanceJoint2D)) as DistanceJoint2D;
					hinge.enableCollision = true;
					hinge.autoConfigureConnectedAnchor = false;
					hinge.autoConfigureDistance = false;
					hinge.connectedAnchor = linePoints [1];
					hinge.distance = Vector3.Distance (transform.position, linePoints [1]);
				}
			}
		}

		// check for splits in the rope
		if (ropeActive) {
			RaycastHit2D hit;
			Vector2 ropeDir = linePoints [1] - linePoints [0];

			int layerMask = 1 << 8;
			layerMask = ~layerMask;

			hit = Physics2D.Raycast (transform.position, ropeDir, ropeDir.magnitude, layerMask);

			if(hit.collider != null) {
				Vector3 hitPoint = new Vector3 (hit.point.x, hit.point.y);
				GameObject.Find ("rayTarget").transform.position = hit.point;
				// A new point in between (far away, enough to warrant a split in the rope)
				if ((linePoints [1] - hitPoint).magnitude > ropeSplitEpsilon) {
					Vector3[] newPoints = new Vector3[linePoints.Length + 1];

					newPoints [0] = linePoints [0]; // player
					newPoints [1] = hitPoint; 		// new anchor
					newPoints [2] = linePoints [1];	// last anchor

					for (int i = 2; i < linePoints.Length; i++) {
						newPoints [i + 1] = linePoints [i];
					}

					linePoints = newPoints;

					hinge.connectedAnchor = linePoints [1];
					hinge.distance = Vector3.Distance (transform.position, hitPoint);
				}
			}

			lineRenderer.SetPositions (linePoints);
		}
	}
}
