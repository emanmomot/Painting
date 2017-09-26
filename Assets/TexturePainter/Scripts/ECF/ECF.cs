using UnityEngine;
using System;
using System.Collections.Generic;

namespace ECF {
	public class ECF {

		const float MAX_ANGLE = Mathf.PI / 6F;
		const float MAX_ERROR = .1f;
		const int MAX_N_ITERATION = 4;
		const int BOX_SIZE = 40;
		const float INCREASE_K = 0.2f;
		const int LINE_D = 50;
		const int N = 50;

		Curve curve;
		List<List<Point>> vectorDistMap;

		RasterBorder rasterBorder;
		int drawingFieldWidth, drawingFieldHeight;
		int iterations = 0;
		bool breakFlag = false;

		List<float> ti = new List<float>();

		CanvasContext o;

		public ECF (int w, int h, CanvasContext o) {
			this.o = o;
			drawingFieldWidth = w;
			drawingFieldHeight = h;
			clearVectorDistMap(drawingFieldWidth, drawingFieldHeight);
			initLocalVariables();
		}

		public float getVectorAngle(Point v1, Point v2) {
			float scalarMult = v1.getScalarMult (v2);
			float d1 = v1.getLength ();
			float d2 = v2.getLength();

			return Mathf.Acos(scalarMult / (d1 * d2));
		}

		public bool testCorner(float x, float y, CurveSegment seg) {
			float angle = getVectorAngle(new Point(seg.C2.x - seg.C3.x, seg.C2.y - seg.C3.y), 
				new Point(x - seg.C3.x, y - seg.C3.y));

			if (angle < MAX_ANGLE) {
				return true;
			} else {
				return false;
			}
		}

		public void drawPixel(Point p) {
			throw new NotImplementedException ("draw baby");
		}

		void drawEdge(Edge e) {
			//o.DrawLine (e.p1, e.p2);
		}



		public List<Point> rasterize(List<Edge> edges) {
			List<Point> points = new List<Point>();

			int i, j, max_i, max_j, index;
			int ymax = 0, ymin = drawingFieldHeight;
			Edge edge;
			Point temp;
			List<Edge> SLB = new List<Edge> ();
			List<Edge>[] ET;
			bool activeFlag;
			int pixelIterator, SLBIterator;

			for (i = 0, max_i = edges.Count; i < max_i; i += 1) {
				edge = edges[i];
				if (edge.p1.y > edge.p2.y) {
					temp = edge.p2;
					edge.p2 = edge.p1;
					edge.p1 = temp;
				}

				if (edge.p1.y < ymin) {
					ymin = (int)edge.p1.y;
				}

				if (edge.p2.y > ymax) {
					ymax = (int)edge.p2.y;
				}

				edge.startY = drawingFieldHeight - Mathf.Floor(drawingFieldHeight - edge.p1.y);
				edge.k = (edge.p2.x - edge.p1.x) / (edge.p2.y - edge.p1.y);
				edge.startX = edge.p1.x + (edge.startY - edge.p1.y) * edge.k;

				edge.stopY = Mathf.Floor(edge.p2.y);

				drawEdge (edge);
			}

			for (i = edges.Count - 1; i >= 0; i -= 1) {
				if (edges[i].p1.y == edges[i].p2.y) {
					edges.Splice (i, 1);
				}
			}

			ymax = Mathf.FloorToInt(ymax);
			ymin = drawingFieldHeight - Mathf.FloorToInt(drawingFieldHeight - ymin);

			ET = new List<Edge>[(int)(ymax - ymin + 1)];

			for (i = 0, max_i = edges.Count; i < max_i; i += 1) {
				index = (int)(edges[i].startY - ymin);
				if (index >= ET.Length) {
					continue;
				}
				if (ET[index] == null) {
					ET [index] = new List<Edge> ();
				}
				ET[index].Add(edges[i]);
			}

			for (i = ymin, max_i = ymax; i <= max_i; i += 1) {
				for (j = SLB.Count - 1; j >= 0; j -= 1) {
					SLB[j].startX += SLB[j].k;
				}

				if ((ET[i - ymin] != null) && (ET[i - ymin].Count > 0)) {
					SLB.AddRange (ET [i - ymin]);
					SLB.Sort (delegate(Edge a, Edge b) {
						if (a.startX - b.startX != 0){
							return (int)Mathf.Sign(a.startX - b.startX);
						} else {
							return (int)Mathf.Sign(a.p2.x - b.p2.x);
						}
					});
				}

				for (j = SLB.Count - 1; j >= 0; j -= 1) {
					if (SLB[j].stopY == i) {
						SLB.Splice(j, 1);
					}
				}

				if (SLB.Count > 1) {
					pixelIterator = Mathf.RoundToInt(SLB[0].startX);
					activeFlag = true;
					SLBIterator = 1;

					do {
						if (activeFlag) {
							points.Add(new Point(pixelIterator, i));
							if (pixelIterator > rasterBorder.maxx) {
								rasterBorder.maxx = pixelIterator;
							}
							if (pixelIterator < rasterBorder.minx) {
								rasterBorder.minx = pixelIterator;
							}
							if (i > rasterBorder.maxy) {
								rasterBorder.maxy = i;
							}
							if (i < rasterBorder.miny) {
								rasterBorder.miny = i;
							}
						}
						pixelIterator++;

						if ((activeFlag) && (pixelIterator > Mathf.Round(SLB[SLBIterator].startX)) ||
							(!activeFlag) && (pixelIterator >= Mathf.Round(SLB[SLBIterator].startX)))  {
							SLBIterator++;
							activeFlag = !activeFlag;
						}
					} while (SLBIterator < SLB.Count);
				}
			}

			return points;
		}

		Point getDistanceVectorLine (Point n, Point p, float xPrev, float yPrev) {
			float distance;
			Point returnV;

			distance = n.getScalarMult(new Point(p.x - xPrev, p.y - yPrev));
			returnV = new Point(n.x * distance, n.y * distance);

			return returnV;
		}

		Vec getDistanceDeffVectorLine (List<Point> points, float x, float y, float xPrev, float yPrev, Point n) {
			Point distance, dy, dx;

			if (points.Count > 1) {
				distance = getDistanceVectorLine(n, points[0], xPrev, yPrev);
				dy = getDistanceVectorLine(n, new Point(points[0].x, points[0].y + 1), xPrev, yPrev);
				dx = getDistanceVectorLine(n, new Point(points[0].x + 1, points[0].y), xPrev, yPrev);

				return new Vec (new Point (dx.x - distance.x, dy.x - distance.x), 
					new Point (dx.y - distance.y, dy.y - distance.y));
			} else {
				return new Vec (
					new Point (),
					new Point ()
				);
			}
		}

		Point getDistanceVectorPoint (float xPrev, float yPrev, Point p) {
			var d = new Point(p.x - xPrev, p.y - yPrev);

			return d;
		}

		Vec getDistanceDeffVectorPoint (List<Point> points, float xPrev, float yPrev) {
			Point distance, dy, dx;

			if (points.Count > 1) {
				distance = getDistanceVectorPoint(xPrev, yPrev, points[0]);
				dy = getDistanceVectorPoint(xPrev, yPrev, new Point(points[0].x, points[0].y + 1));
				dx = getDistanceVectorPoint(xPrev, yPrev, new Point(points[0].x + 1, points[0].y));

				return new Vec(
					new Point(dx.x - distance.x, dy.x - distance.x),
					new Point(dx.y - distance.y, dy.y - distance.y)
				);
			} else {
				return new Vec(
					new Point(),
					new Point()
				);
			}
		}

		void processPoints(List<Point> points, Vec dV, Point oV, List<List<Point>> vectorDistMap, Point n, float xPrev, float yPrev) {
			int i, max_i;
			Point v = new Point ();
			//v2,
			//l;

			for (i = 0, max_i = points.Count - 1; i < max_i; i += 1) {
				v = new Point(oV.x + (points[i + 1].x - points[0].x) * dV.fx.x +
					(points[i + 1].y - points[0].y) * dV.fx.y, 
					oV.y + (points[i + 1].x - points[0].x) * dV.fy.x +
					(points[i + 1].y - points[0].y) * dV.fy.y);

				try {
					if (vectorDistMap[(int)points[i + 1].y][(int)points[i + 1].x].nil == true) {
						vectorDistMap[(int)points[i + 1].y][(int)points[i + 1].x] = v;
						vectorDistMap[(int)points[i + 1].y][(int)points[i + 1].x].nil = false;
					} else {
						if (vectorDistMap[(int)points[i + 1].y][(int)points[i + 1].x].getLength() > v.getLength()) {
							vectorDistMap[(int)points[i + 1].y][(int)points[i + 1].x] = v;
						}
					}
				} catch (Exception e) {

				}
			}
		}

		void renderLineCell(float xPrev, float yPrev, float x, float y, float fieldRadius, List<List<Point>> vectorDistMap) {
			List<Edge> edges = new List<Edge> ();
			Point n = new Point (y - yPrev, -(x - xPrev));
			Point nF = new Point ();
			List<Point> points;
			Point oV = new Point ();
			Vec dV;

			n.normalize();
			nF = new Point(n.x * fieldRadius, n.y * fieldRadius);

			edges.Add(new Edge(new Point(x + nF.x, y + nF.y), new Point(xPrev + nF.x, yPrev + nF.y)));
			edges.Add(new Edge(new Point(xPrev + nF.x, yPrev + nF.y), new Point(xPrev - nF.x, yPrev - nF.y)));
			edges.Add(new Edge(new Point(xPrev - nF.x, yPrev - nF.y), new Point(x - nF.x, y - nF.y)));
			edges.Add(new Edge(new Point(x - nF.x, y - nF.y), new Point(x + nF.x, y + nF.y)));

			points = rasterize(edges);

			if (points.Count > 0) {
				dV = getDistanceDeffVectorLine(points, x, y, xPrev, yPrev, n);
				oV = getDistanceVectorLine(n, points[0], xPrev, yPrev);
				processPoints(points, dV, oV, vectorDistMap, n, xPrev, yPrev);
			}
		}

		void renderPointCell(float xPrev, float yPrev, float fieldRadius, List<List<Point>> vectorDistMap) {
			List<Edge> edges = new List<Edge> ();
			List<Point> points;
			Point oV;
			Vec dV;

			edges.Add(new Edge(new Point(xPrev - fieldRadius, yPrev + fieldRadius), new Point(xPrev + fieldRadius, yPrev + fieldRadius)));
			edges.Add(new Edge(new Point(xPrev + fieldRadius, yPrev + fieldRadius), new Point(xPrev + fieldRadius, yPrev - fieldRadius)));
			edges.Add(new Edge(new Point(xPrev + fieldRadius, yPrev - fieldRadius), new Point(xPrev - fieldRadius, yPrev - fieldRadius)));
			edges.Add(new Edge(new Point(xPrev - fieldRadius, yPrev - fieldRadius), new Point(xPrev - fieldRadius, yPrev + fieldRadius)));

			points = rasterize(edges);

			if (points.Count > 0) {
				dV = getDistanceDeffVectorPoint(points, xPrev, yPrev);
				oV = getDistanceVectorPoint(xPrev, yPrev, points[0]);
				processPoints(points, dV, oV, vectorDistMap, null, xPrev, yPrev);
			}
		}


		void drawArrow(float s, float f, float c) {
			throw new NotImplementedException("draw arrow");
		}

		void drawCurve(CurveSegment seg, bool noClear) {

			if (!noClear) {
				o.Clear ();
			}

			o.BezierCurve (seg);

		}

		string updateCurveSegment(float x, float y, CurveSegment seg) {
			Point prev = new Point (seg.C3);
			float error = 0;
			int nIteration = 0;
			Point f1, f2;
			int i, max_i;
			Point p = new Point (),
			dp = new Point ();
			float d;
			float projection = 0;
			Point v = new Point((seg.C3.x - seg.C0.x) / 3, (seg.C3.y - seg.C0.y) / 3);


			if (testCorner(x, y, seg)) {
				return "CORNER";
			}

			saveControlVertices(seg);

			seg.C3 = new Point(x, y);
			seg.C2 = new Point(seg.C2.x + (x - prev.x), seg.C2.y + (y - prev.y));

			if (v.getLength() < LINE_D) {
				seg.C2.x = seg.C3.x - v.x;
				seg.C2.y = seg.C3.y - v.y;

				if(seg.constrained == true) {
					projection = seg.tan.getScalarMult(v);
					v = new Point(projection * seg.tan.x, projection * seg.tan.y);
				}

				seg.C1.x = seg.C0.x + v.x;
				seg.C1.y = seg.C0.y + v.y;
			}

			renderLineCell(prev.x, prev.y, x, y, BOX_SIZE, vectorDistMap);
			renderPointCell(prev.x, prev.y, BOX_SIZE, vectorDistMap);

			//drawCurve(o, seg);

			do {
				f1 = new Point(); f2 = new Point();
				error = 0;

				for (i = 0, max_i = N; i < max_i; i += 1) {
					p = calc(seg, ti[i]);
					dp = interpVectorDist(vectorDistMap, p.x, p.y);
					d = dp.getLength();


					if (d > error) {
						error = d;
					}

					error += d;

					f1.x += 6 * ti[i] * (1 - ti[i]) * (1 - ti[i]) * d * dp.x / N;
					f1.y += 6 * ti[i] * (1 - ti[i]) * (1 - ti[i]) * d * dp.y / N;

					f2.x += 6 * ti[i] * ti[i] * (1 - ti[i]) * d * dp.x / N;
					f2.y += 6 * ti[i] * ti[i] * (1 - ti[i]) * d * dp.y / N;
				}

				error /= N;

				if (seg.constrained == true) {
					projection = seg.tan.getScalarMult(f1);
					f1 = new Point( projection * seg.tan.x, projection * seg.tan.y);
				}

				seg.C1.x = seg.C1.x - INCREASE_K * f1.x;
				seg.C1.y = seg.C1.y - INCREASE_K * f1.y;

				seg.C2.x = seg.C2.x - INCREASE_K * f2.x;
				seg.C2.y = seg.C2.y - INCREASE_K * f2.y;

				drawCurve(seg, false);

				nIteration++;
			} while ((nIteration < MAX_N_ITERATION));

			/*
        if (breakFlag) {
            breakFlag = false;
            resetControlVertices(seg);
            return 'FAILURE';
        }
        drawCurve(o, seg);
        breakFlag = false;
        return 'SUCCESS';*/

			if (error < MAX_ERROR) { 
				drawCurve(seg, false);
				breakFlag = false;
				return "SUCCESS";
			} else {
				resetControlVertices(seg);
				return "FAILURE";
			}
		}

		void saveControlVertices(CurveSegment seg) {
			if (seg.save == null) {
				seg.save = new CurveSegment ();
			}

			seg.save.C1 = seg.C1;
			seg.save.C2 = seg.C2;
			seg.save.C3 = seg.C3;
		}

		void resetControlVertices(CurveSegment seg) {
			if (seg.save != null) {
				seg.C1 = seg.save.C1;
				seg.C2 = seg.save.C2;
				seg.C3 = seg.save.C3;
			}
		}


		void clearVectorDistMap(int w, int h) {
			int i, max_i, j, max_j;
			List<Point> newArr;
			Point nullPoint;

			w = Mathf.FloorToInt(w);
			h = Mathf.FloorToInt(h);

			max_i = h; max_j = w;
			vectorDistMap = new List<List<Point>> ();

			for( i = 0; i < h; i += 1) {
				newArr = new List<Point>();

				for( j = 0; j < w; j += 1) {
					nullPoint = new Point();
					nullPoint.nil = true;
					newArr.Add(nullPoint);
				}

				vectorDistMap.Add(newArr);
			}

			rasterBorder.minx = w;
			rasterBorder.maxx = 0;

			rasterBorder.miny = h;
			rasterBorder.maxy = 0;
		}

		void clearRaster (RasterBorder rasterBorder, List<List<Point>> vectorDistMap, CanvasContext o) {
			int i, j,
			max_i, max_j;

			for (i = rasterBorder.miny, max_i = rasterBorder.maxy; i < max_i; i += 1) {
				for (j = rasterBorder.minx, max_j = rasterBorder.maxx; j < max_j; j += 1) {
					if (i > 0 && j > 0 && vectorDistMap.Count > i && vectorDistMap [i].Count > j) {
						
						vectorDistMap [i] [j] = new Point ();
						vectorDistMap [i] [j].nil = true;
					}
				}
			}

			rasterBorder.minx = o.width;
			rasterBorder.maxx = 0;

			rasterBorder.miny = o.height;
			rasterBorder.maxy = 0;
		}

		void initParametricCurve() {
			curve = new Curve ();
		}

		void initCurveSegment(float x, float y, CurveSegment seg) {
			seg.C0 = new Point(x, y);
			seg.C1 = new Point(x, y);
			seg.C2 = new Point(x, y);
			seg.C3 = new Point(x, y);
		}

		void initLocalVariables() {
			int i, max_i;
			int TRASHOLD_K = 0,
			TRASHOLD_V = Mathf.FloorToInt(N * TRASHOLD_K);

			ti = new List<float> ();

			for (i = 0, max_i = N; i < max_i; i += 1) {
				ti.Add((i + 1 + TRASHOLD_V) / (N + 2 * TRASHOLD_V));
			}
		}

		Point calc(CurveSegment seg, float t) {
			float x, y;

			//t = Math.pow(t, 1 / 2);

			x = seg.C0.x * (1 - t) * (1 - t) * (1 - t) + 
				3 * seg.C1.x * t * (1 - t) * (1 - t) + 
				3 * seg.C2.x * t * t * (1 - t) + 
				seg.C3.x * t * t * t;

			y = seg.C0.y * (1 - t) * (1 - t) * (1 - t) + 
				3 * seg.C1.y * t * (1 - t) * (1 - t) + 
				3 * seg.C2.y * t * t * (1 - t) + 
				seg.C3.y * t * t * t;

			return new Point(x, y);
		}

		public void DrawAll() {
			drawAllCurves (curve);
		}


		public void drawAllCurves(Curve curve) {
			int i, max_i;

			for (i = 0, max_i = curve.curveSeg.Count; i < max_i; i += 1) {
				drawCurve(curve.curveSeg[i], true);
			}
		}

		Point interpVectorDist(List<List<Point>> vectorDistMap, float x, float y) {
			float x1 = Mathf.Floor (x);
			float y1 = Mathf.Floor (y);
			float x2 = x1 + 1, y2 = y1 + 1;
			Point Q11 = vectorDistMap[(int)y1][(int)x1],
			Q21 = vectorDistMap[(int)y1][(int)x2],
			Q12 = vectorDistMap[(int)y2][(int)x1],
			Q22 = vectorDistMap[(int)y2][(int)x2];

			return new Point(Q11.x * (x2 - x) * (y2 - y) + 
				Q21.x * (x - x1) * (y2 - y) +
				Q12.x * (x2 - x) * (y - y1) +
				Q22.x * (x - x1) * (y - y1), 
				Q11.y * (x2 - x) * (y2 - y) + 
				Q21.y * (x - x1) * (y2 - y) +
				Q12.y * (x2 - x) * (y - y1) +
				Q22.y * (x - x1) * (y - y1));
		}


		public void mouseDownCallback(float x, float y) {
			//o.pctx.clearRect(0, 0, o.c.width(), o.c.height());
			//o.ctx.clearRect(0, 0, o.c.width(), o.c.height());
			//o.octx.clearRect(0, 0, o.c.width(), o.c.height());
			initParametricCurve();
			initCurveSegment(x, y, curve.curveSeg[0]);
			clearVectorDistMap(drawingFieldWidth, drawingFieldHeight);
		}

		public CurveSegment mouseMoveCallback (float x, float y) {
			CurveSegment currentSeg = curve.curveSeg [curve.curveSeg.Count - 1];
			Point prev = new Point (currentSeg.C3);
			string updateResult;
			CurveSegment nextSeg;
			float tanLength;

			updateResult = updateCurveSegment(x, y, currentSeg);

			for (int i = 0; i < curve.curveSeg.Count - 1; i++) {
				o.BezierCurve (curve.curveSeg [i]);
			}

			if (updateResult != "SUCCESS") {
				nextSeg = new CurveSegment();

				initCurveSegment(prev.x, prev.y, nextSeg);
				clearRaster(rasterBorder, vectorDistMap, o);

				o.Clear ();
				o.BezierCurve (currentSeg);

				/*o.pctx.clearRect (0, 0, o.c.width(), o.c.height());
				o.pctx.lineWidth = 3;
				o.pctx.moveTo(currentSeg.C0.x, currentSeg.C0.y);
				o.pctx.bezierCurveTo(currentSeg.C1.x, currentSeg.C1.y,
					currentSeg.C2.x, currentSeg.C2.y,
					currentSeg.C3.x, currentSeg.C3.y);
				o.pctx.stroke();*/

				if (updateResult == "FAILURE") {
					nextSeg.constrained = true;
					nextSeg.tan = new Point(currentSeg.C3.x - currentSeg.C2.x, currentSeg.C3.y - currentSeg.C2.y);
					nextSeg.tan.normalize();
					nextSeg.C2.x = nextSeg.C0.x - nextSeg.tan.x;
					nextSeg.C2.y = nextSeg.C0.y - nextSeg.tan.y;

				} else {
					if (updateResult != "CORNER") {
						nextSeg.constrained = false;
					}
				}

				updateCurveSegment(x, y, nextSeg);
				curve.curveSeg.Add(nextSeg);
			}

			/*
            o.ctx.clearRect (0, 0, o.c.width(), o.c.height());
            o.ctx.moveTo(currentSeg.C0.x, currentSeg.C0.y);
            o.ctx.bezierCurveTo(currentSeg.C1.x, currentSeg.C1.y,
                                currentSeg.C2.x, currentSeg.C2.y,
                                currentSeg.C3.x, currentSeg.C3.y);
            o.ctx.stroke();*/

			return currentSeg;
		}

		public void mouseUpCallback() {
			CurveSegment currentSeg = curve.curveSeg[curve.curveSeg.Count - 1];
			//drawAllCurves(o, curve, o.pctx);
			//o.pctx.clearRect(0, 0, o.c.width(), o.c.height());
			o.Clear();
			o.BezierCurve (currentSeg);
			/*o.pctx.clearRect (0, 0, o.c.width(), o.c.height());
			o.pctx.lineWidth = 3;
			o.pctx.moveTo(currentSeg.C0.x, currentSeg.C0.y);
			o.pctx.bezierCurveTo(currentSeg.C1.x, currentSeg.C1.y,
				currentSeg.C2.x, currentSeg.C2.y,
				currentSeg.C3.x, currentSeg.C3.y);
			o.pctx.stroke();*/
		}

	}
}

