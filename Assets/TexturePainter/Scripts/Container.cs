using UnityEngine;
using System.Collections;

public class Container : MonoBehaviour
{
	const float c_timeToDissapear = 200;
	const float c_timeToReappear = 3.0f;

	private MeshRenderer[] parts;

	private float dissapearTimer;
	private float reappearTimer;

	// Use this for initialization
	void Start ()
	{
		parts = GetComponentsInChildren<MeshRenderer> (false);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (reappearTimer > 0) {
			reappearTimer -= Time.deltaTime;
			if (reappearTimer < 0) {
				reappearTimer = 0;
			}
			SetTransOfAll (1.0f - reappearTimer / c_timeToReappear);
		} else if (dissapearTimer > 0) {
			dissapearTimer -= Time.deltaTime;
			if (dissapearTimer < 0) {
				dissapearTimer = 0;
			}

			SetTransOfAll (dissapearTimer / c_timeToDissapear);
		}
	}

	public void StartPaint() {
		reappearTimer = c_timeToReappear * (1 - GetTransparency());
		//SetTransOfAll (1);
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

	private float GetTransparency() {
		return parts [0].material.GetFloat ("_PaintAlpha");
	}
}

