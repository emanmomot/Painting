
using UnityEngine;

namespace ECF {
	public class Point {
		public float x, y;
		public bool nil;

		public Point() { }

		public Point(float x, float y) {
			this.x = x;
			this.y = y;
		}

		public Point(Point p) {
			x = p.x;
			y = p.y;
		}

		public float getLength() {
			return Mathf.Sqrt(this.x * this.x + this.y * this.y);
		}

		public float getScalarMult(Point v2) {
			return this.x * v2.x + this.y * v2.y;
		}

		public void normalize() {
			float vectorLength = getLength ();
			if (vectorLength == 0) {
				x = 1;
				y = 0;
				return;
			}

			x /= vectorLength;
			y /= vectorLength;
		}
	}
}