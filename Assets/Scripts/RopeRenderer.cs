using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
	class RopeRenderer
	{
		private readonly float _ropeWidth;
		private readonly float _ropeMinLength;
		private readonly int _layerMask;
		private readonly LineRenderer _lineRenderer;
		public float ropeMinLength = 1f;

		internal RopeRenderer(LineRenderer lineRenderer, float ropeWidth, float ropeMinLength, int layerMask)
		{
			_lineRenderer = lineRenderer;
			_ropeWidth = ropeWidth;
			_ropeMinLength = ropeMinLength;

			_layerMask = layerMask; ;
		}

		internal void ResetRope(Vector3[] linePoints)
		{
			//TODO: Ok?
			_lineRenderer.startWidth = 0;
			_lineRenderer.endWidth = 0;

			_lineRenderer.numPositions = linePoints.Length;
			_lineRenderer.SetPositions(linePoints);
		}

		internal void Update(Vector3[] invertedLinePoints)
		{
			_lineRenderer.numPositions = invertedLinePoints.Length;
			_lineRenderer.SetPositions(invertedLinePoints);
			_lineRenderer.startWidth = _ropeWidth;
			_lineRenderer.endWidth = _ropeWidth;
		}

		//internal Vector3 GetHitPoint(Vector3 originPos, Vector3 aimPosition)
		//{
		//	return GetHitPointByDir(originPos, (aimPosition - originPos).normalized);
		//}

		internal Vector3 GetHitPoint(Vector3 originPos, Vector3 aimDirection)
		{
			RaycastHit2D hit;
			hit = Physics2D.Raycast(originPos, aimDirection, int.MaxValue, _layerMask);
			if (hit.collider != null)
			{

				Vector2 hullPoint = hit.collider.bounds.ClosestPoint(hit.point);
				Vector3 hitPoint = new Vector3(hullPoint.x, hullPoint.y);

				if (Vector2.Distance(hitPoint, originPos) > ropeMinLength)
				{
					//_linePoints[1].x = hitPoint.x;
					//_linePoints[1].y = hitPoint.y;
					//_lineRenderer.startWidth = _ropeWidth;
					//_lineRenderer.endWidth = _ropeWidth;
					//playerCollider.sharedMaterial = matBouncy;

					return hitPoint;
					//Rope activated
				}
			}
			return Vector3.zero;
		}
	}
}
