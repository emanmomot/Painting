using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SplineUtil {

	public static Vector2 evalSpline2D(Vector2 p, Vector2 cp0, Vector2 cp1, Vector2 m0, Vector2 m1) {
		return new Vector2 (evalSpline1D (p [0], cp0 [0], cp1 [0], m0 [0], m1 [0]), 
							evalSpline1D (p [1], cp0 [1], cp1 [1], m0 [1], m1 [1]));
	}

	/// <param name="coord">0 for x, 1 for y, 2 for z</param>
	public static float evalSpline1D(float x, float xk, float xk1, float mk, float mk1) {
		//float xk = links [k].position [coord];
		//float xk1 = links [k + 1].position [coord];
		float interval = xk1 - xk;

		if (interval == 0) {
			return xk;
		}

		float t = (x - xk) / interval;

		float h00 = (1 + 2 * t) * (1 - t) * (1 - t);
		float h10 = t * (1 - t) * (1 - t);
		float h01 = t * t * (3 - 2 * t);
		float h11 = t * t * (t - 1);

		return h00 * xk + h10  * mk + h01 * xk1 + h11  * mk1;
	}

	public static float lerp1D(float t, float x0, float x1) {
		return (x1 - x0) * t + x0;
	}

	// is tk+1 - t necessarily 1??
	public static float finiteDiffSlope1D(int index, List<float> xs) {
		float m;
		if (index == 0) {
			m = xs [1] - xs [0];
		} else if (index == xs.Count - 1) {
			m = xs [xs.Count - 1] - xs [xs.Count - 2];
		} else {
			m = .5f * ((xs [index + 1] - xs [index])
				+ (xs [index] - xs [index - 1]));
		}
		return m;
	}

	public static Vector2 finiteDiffSlope2D(int index, List<Vector2> xs) {
		Vector2 m;
		if (index == 0) {
			m = xs [1] - xs [0];
		} else if (index == xs.Count - 1) {
			m = xs [xs.Count - 1] - xs [xs.Count - 2];
		} else {
			m = .5f * ((xs [index + 1] - xs [index])
				+ (xs [index] - xs [index - 1]));
		}
		return m;
	}
}
