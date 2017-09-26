using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothBrush {

	const int CP_AFFECT_RANGE = 1;

	private List<Vector2> cps;
	private List<float> sizes;

	private List<Vector2> ms;

	private List<Transform> dots;

	private PaintableObject obj;

	private float stepSize;

	private int numFinalDots;

	public SmoothBrush(Vector2 start, float size, float step, PaintableObject obj) {
		cps = new List<Vector2> ();
		ms = new List<Vector2> ();
		sizes = new List<float> ();

		dots = new List<Transform> ();

		this.stepSize = step;
		this.obj = obj;

		cps.Add (start);
		// dummy cp
		cps.Add (start + new Vector2(.001f,0));
		ms.Add (SplineUtil.finiteDiffSlope2D (0, cps));
		ms.Add (SplineUtil.finiteDiffSlope2D (1, cps));
		sizes.Add (size);
		sizes.Add (size);
	}

	public void AddCP(Vector2 p, float size) {
		cps.Add (p);
		ms.Add (SplineUtil.finiteDiffSlope2D (cps.Count-1, cps));
		sizes.Add (size);
	}

	public void UpdateCursorPos(Vector2 p, float size) {
		cps [cps.Count - 1] = p;
		sizes [sizes.Count - 1] = size;
	}

	public void UpdateStroke() {
		// draw the finalized dots to the texture
		if (cps.Count > 3) {
			for (int i = numFinalDots; i < dots.Count; i++) {
				FreeDot (dots [i].gameObject);
			}
			TexturePainter.singleton.RenderCanvas (obj, false);
			TexturePainter.singleton.SaveTexture (obj);

			cps.RemoveAt (0);
			ms.RemoveAt (0);

			dots.Clear ();
		} else {
			for (int i = 0; i < dots.Count; i++) {
				FreeDot (dots [i].gameObject);
			}
			dots.Clear ();
		}

		RecomputeSlopes ();

		for (int i = 0; i < cps.Count - 1; i++) {
			Vector2 cp0 = cps [i];
			Vector2 cp1 = cps [i + 1];
			Vector2 m0 = ms [i];
			Vector2 m1 = ms [i + 1];

			Vector2 d = cp1 - cp0;
			Vector2 p = cp0;

			float s0 = sizes [i];
			float s1 = sizes [i + 1];

			float ss = stepSize * Mathf.Min (s0, s1);

			float texScaleMult = 1F /Mathf.Min (obj.texScale.x, obj.texScale.y);

			int numSteps = Mathf.FloorToInt (texScaleMult * d.magnitude / ss);
			d = ss * d.normalized / texScaleMult;

			float t = 0;
			float dt = 1.0f / numSteps;

			if (i == 0) {
				numFinalDots = numSteps;
			}

			// first add dot at cp0
			dots.Add (TexturePainter.singleton.MakeDot (obj, cp0, s0).transform);

			// start at 1 bc we already did cp0
			for(int j = 1; j < numSteps; j++) {
				p += d;
				t += dt;

				Vector2 pos = SplineUtil.evalSpline2D (p, cp0, cp1, m0, m1);
				float size = SplineUtil.lerp1D (t, s0, s1);
				GameObject newDot = TexturePainter.singleton.MakeDot (obj, pos, size);
				dots.Add (newDot.transform);
			}
		}
	}

	private void RecomputeSlopes() {
		if (cps.Count == 3) {
			for (int i = 1; i < cps.Count; i++) {
				ms[i] = (SplineUtil.finiteDiffSlope2D (i, cps));
			}
		} else {
			for (int i = 0; i < cps.Count; i++) {
				ms[i] = (SplineUtil.finiteDiffSlope2D (i, cps));
			}
		}
	}

	private void FreeDot(GameObject dot) {
		TexturePainter.singleton.dotPool.FreeDot (dot);
	}
}
