using System;

namespace ECF {
	public class CurveSegment {
		
		public Point C0, C1, C2, C3;
		public Point tan;
		public bool constrained;
		public CurveSegment save;
		public float length;

		public CurveSegment () {
			C0 = new Point ();
			C1 = new Point ();
			C2 = new Point ();
			C3 = new Point ();
		}

		public CurveSegment (float x, float y) {
			C0 = new Point (x, y);
			C1 = new Point (x, y);
			C2 = new Point (x, y);
			C3 = new Point (x, y);
		}
	}
}

