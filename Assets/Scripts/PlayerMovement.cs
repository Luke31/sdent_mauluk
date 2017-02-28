using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float aimDistance = 1;
	public float aimSpeed = 10;
	public float aimMaxAngle = 90f;
	public float ropeWidth = 0.1f;
	public float ropeSplitEpsilon = 0.9f;
	public float ropeMinLength = 0.5f;
	public float ropeFeedSpeed = 0.1f;
	public float ropeSwingForce = 0.6f;

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
	int layerMask;

	LineRenderer lineRenderer;
	Vector3[] linePoints;
	Vector2 ropeDir;

	// Use this for initialization
	void Start ()
	{
		rb = GetComponent<Rigidbody2D> ();
		target = GameObject.Find ("Target");

		lineRenderer = GetComponentInChildren<LineRenderer> ();
		playerCollider = GetComponentInChildren<CircleCollider2D> ();

		matBouncy = (PhysicsMaterial2D)Resources.Load ("PlayerBouncy");
		matStatic = (PhysicsMaterial2D)Resources.Load ("PlayerStatic");

		layerMask = LayerMask.GetMask ("Player");
		layerMask = ~layerMask;

		resetRope ();

		aimDirection = new Vector2 (1, 1).normalized;
		aimPosition = new Vector3 (1, 1, 0);
		target.transform.localPosition = aimPosition;
	}

	void resetRope ()
	{
		linePoints = new Vector3[2];
		for (int i = 0; i < linePoints.Length; i++) {
			linePoints [i] = Vector3.zero;
		}
			
		lineRenderer.numPositions = linePoints.Length;
		lineRenderer.SetPositions (linePoints);
	}

	void FixedUpdate()
	{
		// check for splits in the rope
		if (ropeActive)
		{
			linePoints[0] = transform.position;
			ropeDir = linePoints[1] - linePoints[0];

			RaycastHit2D hitSplit;
			RaycastHit2D hitLast;

			hitSplit = Physics2D.Raycast(transform.position, ropeDir, ropeDir.magnitude - 0.1f, layerMask);

			if (hitSplit.collider != null)
			{
				Vector2 hullPoint = GetClosestPointOnBoundHull(hitSplit.collider, hitSplit.point);
				Vector3 hitPoint = new Vector3(hullPoint.x, hullPoint.y);
				float newRopeLength = Vector2.Distance(linePoints[0], hitPoint);
				float newSplitLength = Vector2.Distance(linePoints[1], hitPoint);

				// A new point in between (far away, enough to warrant a split in the rope)
				if (newRopeLength > ropeMinLength && newSplitLength > ropeSplitEpsilon)
				{
					Vector3[] newPoints = new Vector3[linePoints.Length + 1];

					newPoints[0] = linePoints[0]; // player
					newPoints[1] = hitPoint;        // new anchor
					newPoints[2] = linePoints[1];   // last anchor

					for (int i = 2; i < linePoints.Length; i++)
					{
						newPoints[i + 1] = linePoints[i];
					}

					linePoints = newPoints;

					hinge.connectedAnchor = linePoints[1];
					hinge.distance = Vector3.Distance(transform.position, linePoints[1]);
				}
			}
			else if (linePoints.Length > 2)
			{
				Vector2 lastDir = linePoints[2] - linePoints[0];
				hitLast = Physics2D.Raycast(transform.position, lastDir, lastDir.magnitude - 0.1f, layerMask);

				Vector2 ab = linePoints[1] - linePoints[0];
				Vector2 bc = linePoints[2] - linePoints[1];
				Vector2 ac = linePoints[2] - linePoints[0];

				// We see both last points, maybe release one
				if (hitLast.collider == null && Mathf.Abs(ab.magnitude + bc.magnitude - ac.magnitude) < ropeSplitEpsilon)
				{
					Vector3[] newPoints = new Vector3[linePoints.Length - 1];

					newPoints[0] = linePoints[0]; // player
					newPoints[1] = linePoints[2];   // new anchor = lastAnchor

					for (int i = 2; i < linePoints.Length - 1; i++)
					{
						newPoints[i] = linePoints[i + 1];
					}

					linePoints = newPoints;

					hinge.connectedAnchor = linePoints[1];
					hinge.distance = Vector3.Distance(transform.position, linePoints[1]);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{


		// Update Aim
		if (!ropeActive && Input.GetButton ("AimLeft")) {
			aimTemp = Quaternion.AngleAxis (aimSpeed, Vector3.forward) * aimDirection;

			if (Vector2.Angle (aimTemp, Vector2.up) < aimMaxAngle) {
				aimDirection.Set (aimTemp.x, aimTemp.y);
				aimDirection.Normalize ();
			}
		}

		if (!ropeActive && Input.GetButton ("AimRight")) {
			aimTemp = Quaternion.AngleAxis (-aimSpeed, Vector3.forward) * aimDirection;

			if (Vector2.Angle (aimTemp, Vector2.up) < aimMaxAngle) {
				aimDirection.Set (aimTemp.x, aimTemp.y);
				aimDirection.Normalize ();
			}
		}

		// update aim target
		aimPosition.Set (transform.position.x + (aimDirection.x * aimDistance),
			transform.position.y + (aimDirection.y * aimDistance), 0);
		target.transform.position = aimPosition;

		linePoints[0] = transform.position;
		ropeDir = linePoints[1] - linePoints[0];
		Vector2 left = -Vector3.Cross (ropeDir, Vector3.forward).normalized * ropeSwingForce;
		Vector2 right = -left;

		// Rope Swing
		if (ropeActive && Input.GetButton ("AimLeft")) {
			rb.AddForce (left);
		}

		if (ropeActive && Input.GetButton ("AimRight")) {
			rb.AddForce (right);
		}
			
		// Manage Rope
		if (ropeActive && Input.GetButton ("RopeIn")) {
			RaycastHit2D hitFeed = Physics2D.Raycast (transform.position, ropeDir, ropeFeedSpeed * 3, layerMask);
			if (hitFeed.collider == null && hinge.distance - ropeFeedSpeed > ropeMinLength) {
				hinge.distance -= ropeFeedSpeed;
			}
		}

		if (ropeActive && Input.GetButton ("RopeOut")) {
			RaycastHit2D hitFeed = Physics2D.Raycast (transform.position, -ropeDir, ropeFeedSpeed * 3, layerMask);
			if (hitFeed.collider == null) {
				hinge.distance += ropeFeedSpeed;
			}
		}

		// rope physics

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

					Vector2 hullPoint = hit.collider.bounds.ClosestPoint (hit.point);
					Vector3 hitPoint = new Vector3 (hullPoint.x, hullPoint.y);

					if (Vector2.Distance (hitPoint, transform.position) > ropeMinLength) {
						linePoints [1].x = hitPoint.x;
						linePoints [1].y = hitPoint.y;

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
		}

		Vector3[] invertedLinePoints = new Vector3[linePoints.Length];
		for (int i = 0; i < linePoints.Length; i++) {
			invertedLinePoints [i] = linePoints [linePoints.Length - 1 - i];
		}
		lineRenderer.numPositions = invertedLinePoints.Length;
		lineRenderer.SetPositions (invertedLinePoints);
	}

	private Vector2 GetClosestPointOnBoundHull(Collider2D collider, Vector2 hitPoint) {
		Vector2 result = new Vector2();
		Vector2 tmp;

		Vector2 center = collider.bounds.center;
		Vector2 extent = collider.bounds.extents;

		float minDist = float.MaxValue;
		float dist;

		tmp = center + extent;
		dist = Vector2.Distance (hitPoint, tmp);
		if (dist < minDist) {
			minDist = dist;
			result = tmp;
		}

		extent.Set (extent.x, -extent.y);

		tmp = center + extent;
		dist = Vector2.Distance (hitPoint, tmp);
		if (dist < minDist) {
			minDist = dist;
			result = tmp;
		}

		extent.Set (-extent.x, extent.y);

		tmp = center + extent;
		dist = Vector2.Distance (hitPoint, tmp);
		if (dist < minDist) {
			minDist = dist;
			result = tmp;
		}

		extent.Set (extent.x, -extent.y);

		tmp = center + extent;
		dist = Vector2.Distance (hitPoint, tmp);
		if (dist < minDist) {
			minDist = dist;
			result = tmp;
		}

		return result;
	}

}
