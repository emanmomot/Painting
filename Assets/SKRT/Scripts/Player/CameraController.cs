using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public Camera playerCam;

	public float offsetZ;
	public float offsetY;

	public Transform m_playerTransform;
	private	Transform m_camTransform;

	// Use this for initialization
	void Start () {
		//m_playerTransform = PlayerManager.localPlayer.transform;
		m_camTransform = playerCam.transform;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateCamTransform ();
	}

	void UpdateCamTransform () {
		m_camTransform.position = m_playerTransform.position + 
			(-offsetZ * m_playerTransform.forward + offsetY * m_playerTransform.up);
		m_camTransform.LookAt (m_playerTransform);
	}
}
