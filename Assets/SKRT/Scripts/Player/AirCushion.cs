using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirCushion : MonoBehaviour {

	public Transform[] cushions;
	public float hoverHeight;
	public float maxForce;

	private Rigidbody m_rigidBody;

	//private float forceFuncSlope;

	// Use this for initialization
	void Start () {
		m_rigidBody = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		RaycastHit hit;
		//float mg = m_rigidBody.mass * Physics.gravity.y;
		foreach (Transform t in cushions) {
			Ray ray = new Ray (t.position, -t.up);
			if (Physics.Raycast (ray, out hit, hoverHeight)) {
				float f = ((maxForce - Physics.gravity.y) / hoverHeight) * hit.distance + maxForce;
				f *= m_rigidBody.mass  / cushions.Length;

				m_rigidBody.AddForceAtPosition (f * t.up, t.position);

				Debug.DrawRay (t.position,  -f * t.up, Color.blue, Time.fixedDeltaTime, false);
			}
		}
	}


}
