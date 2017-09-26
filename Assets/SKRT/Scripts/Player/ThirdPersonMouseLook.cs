using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMouseLook : MonoBehaviour {

	public float sensitivity = 7F;
	public Transform model;

	/// angular velocity about transform.up
	public float m_turnAngVel { get; private set; }
	private Rigidbody m_modelRigidbody;

	// Use this for initialization
	void Start () {
		m_modelRigidbody = model.GetComponent<Rigidbody> ();
	}

	public void UpdateRotation() {
		Vector3 turnTorque = transform.up * sensitivity * Input.GetAxis ("Mouse X");
		ApplyTorque (turnTorque);

		// lean model
		m_turnAngVel = Vector3.Dot(m_modelRigidbody.angularVelocity, Vector3.up);
		//model.localRotation = Quaternion.AngleAxis(-m_turnAngVel * 3F, Vector3.forward);
	}

	void ApplyTorque(Vector3 torque) {
		torque = torque * Time.deltaTime * m_modelRigidbody.mass;
		m_modelRigidbody.AddTorque (torque);
	}
}
