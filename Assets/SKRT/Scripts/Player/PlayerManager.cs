using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

	public static PlayerManager localPlayer;

	public PlayerInput playerInput;
	public CameraController cameraController;
	public ThirdPersonMouseLook mouseLook;
	public PlayerMovement playerMovement;
	public KeepOnSurface keepOnSurface;

	void Awake() {
		localPlayer = this;
	}

	// Use this for initialization
	void Start () {
		
	}

	void FixedUpdate() {
		playerMovement.UpdateIsGrounded ();
		keepOnSurface.UpdateKeepOnSurface ();
	}
	
	// Update is called once per frame
	void Update () {
		mouseLook.UpdateRotation ();
		playerMovement.UpdateMovement ();
	}
}
