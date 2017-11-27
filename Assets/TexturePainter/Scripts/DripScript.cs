using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DripScript : MonoBehaviour {

	private Vector2 m_coords;
	private PaintableObject m_obj;

	private Material mat;
	private float m_amt;

	void Awake() {
		mat = GetComponentInChildren<Renderer> ().material;
		mat.SetFloat ("_Seed", Random.value * 50.0f);
		mat.SetFloat ("_Amt", 0);
	}

	// Use this for initialization
	void Start () {
		
	}

	public void Init(Vector2 coords, PaintableObject obj) {
		m_coords = coords;
		m_obj = obj;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetAmt(float amt) {
		this.m_amt = amt;
		mat.SetFloat ("_Amt", m_amt);
	}

	public void IncAmt() {
		m_amt += .07f;
		if (m_amt > 1) {
			m_amt = 1;
		}
		SetAmt (m_amt);
	}

	public bool IsInRange(Vector2 currCoords) {
		return IsInRange (m_coords, currCoords, m_obj.texScale);
	}

	public static bool IsInRange(Vector2 a, Vector2 b, Vector2 texScale) {
		Vector2 d = a - b;
		d.x /= texScale.x;
		d.y /= texScale.y;
		float dist = d.sqrMagnitude;
		return dist < .1f;
	}
}
