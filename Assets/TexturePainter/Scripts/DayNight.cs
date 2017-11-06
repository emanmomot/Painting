using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DayNight : MonoBehaviour {

	public float lengthOfCycle;
	public float height;
	public Transform sun;
	public Transform moon;
	public Transform center;
	public Transform sunLight;

	public Light light;


	public List<Color> topColors;
	public List<Color> bottomColors;

	[Range(0,.99f)]
	public float cycleTimer;

	public Material skybox;

	// Use this for initialization

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (Application.isPlaying) {
			cycleTimer += Time.deltaTime / lengthOfCycle;
			if (cycleTimer > 1) {
				cycleTimer -= 1;
			}
		}
		float val = 2.0f * cycleTimer * Mathf.PI;
		float sinTimer = Mathf.Sin (val);
		float cosTimer = Mathf.Cos (val);

		float t = cycleTimer * 4;
		int colorIndex = Mathf.FloorToInt(cycleTimer * 4);
		int ind2 = (colorIndex + 1) % 4;
		t -= colorIndex;
		Color topColor = InterpolateColor (topColors [colorIndex], topColors [ind2], t);
		Color bottomColor = InterpolateColor (bottomColors [colorIndex], bottomColors [ind2], t);
		skybox.SetColor ("_Color2", topColor);
		skybox.SetColor ("_Color1", bottomColor);
		//light.color = topColor;
		light.intensity = 1.2f * (cosTimer);
		//skybox.Set

		RenderSettings.ambientLight = topColor;
		//RenderSettings.ambientIntensity = (cosTimer);


		sun.position = center.position + Vector3.up * height * cosTimer + center.right * height * sinTimer;
		sunLight.LookAt (center);
	}

	Color InterpolateColor(Color c1,Color c2,float t){
		Vector3 c1HSV,c2HSV;
		Color.RGBToHSV (c1, out c1HSV.x, out  c1HSV.y, out c1HSV.z);
		Color.RGBToHSV (c2, out c2HSV.x, out  c2HSV.y, out c2HSV.z);
		c1HSV.x = c1HSV.x * (1 - t) + c2HSV.x * t;
		c1HSV.y = c1HSV.y * (1 - t) + c2HSV.y * t;
		c1HSV.z = c1HSV.z * (1 - t) + c2HSV.z * t;
		return Color.HSVToRGB (c1HSV.x,c1HSV.y,c1HSV.z);

	}

}
