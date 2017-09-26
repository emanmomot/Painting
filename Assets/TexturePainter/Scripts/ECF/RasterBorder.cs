using System;

namespace ECF
{
	public struct RasterBorder
	{
		public int maxx, maxy, minx, miny;
		public RasterBorder (int maxx, int maxy, int minx, int miny)
		{
			this.maxx = maxx;
			this.maxy = maxy;
			this.minx = minx;
			this.miny = miny;
		}
	}
}

