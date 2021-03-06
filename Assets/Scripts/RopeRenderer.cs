﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
	public class RopeRenderer
	{
		private readonly float _ropeWidth;
		private readonly float _ropeMinLength;
		public int LayerMaskWithoutPlayer { get; set; }
		private readonly LineRenderer _lineRenderer;
		public float ropeMinLength = 1f;

		internal RopeRenderer(LineRenderer lineRenderer, float ropeWidth, float ropeMinLength, int layerMaskWithoutPlayer)
		{
			_lineRenderer = lineRenderer;
			var renderer = _lineRenderer.GetComponent<Renderer>();
			renderer.sortingOrder = 49; //Player is 50
			_ropeWidth = ropeWidth;
			_ropeMinLength = ropeMinLength;

			LayerMaskWithoutPlayer = layerMaskWithoutPlayer;
		}

		internal void ResetRope(Vector3[] linePoints)
		{
			//TODO: Ok?
			_lineRenderer.startWidth = 0;
			_lineRenderer.endWidth = 0;

			_lineRenderer.positionCount = linePoints.Length;
			_lineRenderer.SetPositions(linePoints);
		}

		internal void Update(Vector3[] invertedLinePoints)
		{
			_lineRenderer.positionCount = invertedLinePoints.Length;
			_lineRenderer.SetPositions(invertedLinePoints);
			_lineRenderer.startWidth = _ropeWidth;
			_lineRenderer.endWidth = _ropeWidth;
		}

		internal Vector3 GetHitPoint(Vector3 originPos, Vector3 aimDirection)
		{
			RaycastHit2D hit;
			hit = Physics2D.Raycast(originPos, aimDirection, int.MaxValue, LayerMaskWithoutPlayer);
			if (hit.collider != null)
			{

				Vector2 hullPoint = hit.collider.bounds.ClosestPoint(hit.point);
				Vector3 hitPoint = new Vector3(hullPoint.x, hullPoint.y);

				if (Vector2.Distance(hitPoint, originPos) > ropeMinLength)
				{
					return hitPoint;
					//Rope activated
				}
			}
			return Vector3.zero;
		}
	}
}
