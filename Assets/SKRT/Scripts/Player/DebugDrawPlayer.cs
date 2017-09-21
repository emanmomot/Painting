using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDrawPlayer : MonoBehaviour {

	public GameObject groundedSymbol;

	private Rigidbody m_rigidbody;
	private PlayerManager m_playerManager;

	// Use this for initialization
	void Start () {
		m_rigidbody = GetComponent<Rigidbody> ();
		m_playerManager = GetComponent<PlayerManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		Debug.DrawRay (transform.position,  m_rigidbody.velocity, Color.green, Time.deltaTime, false);
		if (m_playerManager.playerMovement.isGrounded) {
			groundedSymbol.SetActive (true);
		} else {
			groundedSymbol.SetActive (false);
		}
	}
}
