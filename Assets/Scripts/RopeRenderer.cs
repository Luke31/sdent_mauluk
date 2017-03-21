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
		private readonly float _ropeFeedSpeed;

		private readonly LineRenderer _lineRenderer;

		internal RopeRenderer(LineRenderer lineRenderer, float ropeWidth, float ropeFeedSpeed)
		{
			_lineRenderer = lineRenderer;
			_ropeWidth = ropeWidth;
			_ropeFeedSpeed = ropeFeedSpeed;
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
		}

		internal void ActivateRope()
		{
			_lineRenderer.startWidth = _ropeWidth;
			_lineRenderer.endWidth = _ropeWidth;
		}
	}
}
