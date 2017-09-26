using System;
using System.Collections.Generic;

namespace ECF
{
	public class Curve
	{
		public List<CurveSegment> curveSeg;
		public Curve ()
		{
			curveSeg = new List<CurveSegment> ();
			curveSeg.Add (new CurveSegment ());
		}
	}
}

