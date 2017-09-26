using System;

namespace ECF {
	public class Edge {

		public Point p1, p2;
		public float startX, startY, k, stopY;
		
		public Edge (Point p1, Point p2) {
			this.p1 = p1;
			this.p2 = p2;
		}

		public Edge (float a, float b, float c, float d) {
			p1 = new Point (a, b);
			p2 = new Point (c, d);
		}
	}
}

