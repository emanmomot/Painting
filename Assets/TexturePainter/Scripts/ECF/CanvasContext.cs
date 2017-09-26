using System;
using UnityEngine;

namespace ECF
{
	public class CanvasContext
	{

		public int width, height;
		public CanvasContext ()
		{
		}

		public void BezierCurve(CurveSegment seg) {
			DrawLine (seg.C0, seg.C1);
			DrawLine (seg.C1, seg.C2);
			DrawLine (seg.C2, seg.C3);

		}

		public void DrawLine(Point p1, Point p2) {
			Vector3 v1 = new Vector3 (p1.x, p1.y, 0);
			v1.x = v1.x / Screen.width;
			v1.y = v1.y / Screen.height;

			Vector3 v2 = new Vector3 (p2.x, p2.y, 0);
			v2.x = v2.x / Screen.width;
			v2.y = v2.y / Screen.height;
			Debug.DrawLine (v1, v2, Color.green, Time.deltaTime, false);
		}

		public void Clear() {

		}
	}
}

