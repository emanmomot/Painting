using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public float acceleration;
	public float strafeAccel;
	public float topSpeed;
	public float carveCoefficient;

	public float jumpAccel;

	public float airDrag;

	public bool isGrounded { get; private set; }

	private Rigidbody m_rigidbody;
	private ThirdPersonMouseLook m_mouseLook;

	private float m_defaultDrag;

	// Use this for initialization
	void Start () {
		m_rigidbody = GetComponent<Rigidbody> ();
		m_mouseLook = GetComponent<ThirdPersonMouseLook> ();
		m_defaultDrag = m_rigidbody.drag;
	}

	public void UpdateMovement() {

		UpdateIsGrounded ();
		UpdateVelocity ();
		UpdateJump ();

	}

	void UpdateVelocity() {
		// rotate velocity according to rotation
		m_rigidbody.velocity = Quaternion.AngleAxis (m_mouseLook.m_turnAngVel * carveCoefficient, 
			transform.up) * m_rigidbody.velocity;

		Vector2 input = new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
		input.Normalize ();

		// get forward vector
		Vector3 forwardVec = transform.forward;

		// forward velocity
		float currentSpeed = Vector3.Dot (m_rigidbody.velocity, forwardVec);
		currentSpeed *= Mathf.Sign (input.y);

		// only accelerate if we are not at top speed in direction of acceleration
		if (currentSpeed < topSpeed) {
			float forwardForce = acceleration * input.y;
			ApplyForce (forwardVec, forwardForce);
		}

		// horiz velocity
		Vector3 rightVec = Vector3.Cross (Vector3.up, forwardVec);
		float hvel = Vector3.Dot (m_rigidbody.velocity, rightVec);
		hvel *= Mathf.Sign (input.x);

		// strafe
		if (hvel < topSpeed) {
			float strafeForce = input.x * strafeAccel;
			ApplyForce (rightVec, strafeForce);
		}
	}

	void UpdateJump() {
		if (isGrounded && Input.GetButtonDown("Jump")) {
			m_rigidbody.AddForce (transform.up * jumpAccel * m_rigidbody.mass);
		}
	}

	void UpdateIsGrounded() {
		RaycastHit hit;
		if (Physics.Raycast (transform.position, -transform.up, out hit, 1.3f)) {
			isGrounded = true;
			m_rigidbody.drag = m_defaultDrag;
		} else {
			isGrounded = false;
			m_rigidbody.drag = airDrag;
		}
	}

	void ApplyForce(Vector3 direction, float magnitude) {
		Vector3 force = direction * magnitude * Time.deltaTime * m_rigidbody.mass;
		m_rigidbody.AddForce (force);
	}
}
