using UnityEngine;
using System.Collections;

public class Container : MonoBehaviour
{
	const float c_timeToDissapear = 20;

	private MeshRenderer[] parts;

	private float dissapearTimer;

	// Use this for initialization
	void Start ()
	{
		parts = GetComponentsInChildren<MeshRenderer> (false);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (dissapearTimer > 0) {
			dissapearTimer -= Time.deltaTime;
			if (dissapearTimer < 0) {
				dissapearTimer = 0;
			}

			SetTransOfAll (dissapearTimer / c_timeToDissapear);
			/*if (mat.HasProperty ("_PaintAlpha")) {
				mat.SetFloat ("_PaintAlpha", dissapearTimer / c_timeToDissapear);
			} else {
				Color c = mat.GetColor ("_Color");
				c.a = dissapearTimer / c_timeToDissapear;
				mat.SetColor ("_Color", c);
//			}*/
		}
	}

	public void StartPaint() {
		SetTransOfAll (1);
		dissapearTimer = 0;
	}

	public void EndPaint() {
		dissapearTimer = c_timeToDissapear;
	}

	private void SetTransOfAll(float trans) {
		foreach (MeshRenderer rend in parts) {
			SetTransparencyOfPart (rend, trans);
		}
	}

	private void SetTransparencyOfPart(MeshRenderer rend, float trans) {
		rend.material.SetFloat ("_PaintAlpha", trans);
		//Color c = rend.material.color;
		//c *= trans;
		//rend.material.
		//c.a = trans;
		//rend.material.color = c;
	}
}

