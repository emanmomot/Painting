using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepOnSurface : MonoBehaviour {

	private PlayerMovement m_playerMovement;

	// Use this for initialization
	void Start () {
		m_playerMovement = PlayerManager.localPlayer.playerMovement;
	}

	public void UpdateKeepOnSurface() {
		if (m_playerMovement.isGrounded) {
			Vector3 desiredForward = Vector3.Cross (transform.right, m_playerMovement.surfaceNormal);
			desiredForward.Normalize ();
			Debug.DrawRay (transform.position, desiredForward, Color.green, Time.fixedDeltaTime, false);
			float dtheta = Vector3.Angle (transform.forward, desiredForward);
			//Debug.Log (dtheta);
			transform.rotation *= Quaternion.AngleAxis (.1f * dtheta, transform.right);
			//transform.up = m_playerMovement.surfaceNormal;
		}
	}

}
