using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerManager : Singleton<PlayerManager> {

	public float speedShifted = 1.0f;
	public float speedForward;
	public float ratioBack;
	public float ratioSides;
	public float jumpMin = 3;
	public float jumpOffset = 1;
	public float jumpMoving = 1.2f;
	public float jumpShifted = 1.2f;
	public float jumpMagnitude = 0.4f;
	public PhysicMaterial physicsMoving;
	public PhysicMaterial physicsIdle;
	public Camera playerCamera;
	
	[HideInInspector] public ApplyFriction fric;
	[HideInInspector] public CheckGround checkGround;


	// Use this for initialization
	public override void Start () {
		base.Start();
		fric = GetComponent<ApplyFriction>();
		checkGround = GetComponent<CheckGround>();
	}
	
	// Update is called once per frame
	void Update () {
		bool isForward = InputUtils.HoldAny("up", KeyCode.W);
		bool isLeft = InputUtils.HoldAny("left", KeyCode.A);
		bool isRight = InputUtils.HoldAny("right", KeyCode.D);
		bool isBackward = InputUtils.HoldAny("down", KeyCode.S);
		bool isJump = InputUtils.HoldOrPressed(KeyCode.Space);
		bool isShift = InputUtils.IsShift();
		bool isMoving = false;

		float speedAdapted = (isShift ? speedShifted : 1) * this.speedForward * Time.deltaTime;
		float speedStrafe = speedAdapted * ratioSides;
		float speedBack = speedAdapted * ratioBack;
		float magnitude = rb.velocity.magnitude;
		
		int directionX = 0, directionZ = 0;
		if (isForward) { directionZ++; }
		if (isBackward) { directionZ--; }
		if (isLeft) { directionX--; }
		if (isRight) { directionX++; }
		
		if(directionZ!=0) {
			float speedZ = directionZ > 1 ? speedAdapted : speedBack;
			rb.AddForce(transform.forward * directionZ * speedZ, ForceMode.Impulse);
			isMoving = true;
		} else {
			// ...
		}

		fric.isShifted = isShift;

		if (directionX!=0) {
			isMoving = true;
			rb.AddForce(transform.right * directionX * speedStrafe, ForceMode.Impulse);
		} else {
			// ...
		}

		if(isMoving) {
			col.material = physicsMoving;
		} else {
			col.material = physicsIdle;
		}

		if(isJump && checkGround.isOnGround) Jump(isMoving, isShift);
	}

	public void Jump(bool isMoving, bool isShift) {
		checkGround.wasOnGround = checkGround.isOnGroundNow = false;

		float min = jumpMin * (isMoving ? jumpMoving : 1) * (isShift ? jumpShifted : 1);

		transform.position = transform.position + Vector3.up * jumpOffset * 0.005f;
		Vector3 vel = rb.velocity;
		vel.Set(vel.x, 0, vel.z);
		rb.velocity = vel;
		rb.AddForce(Vector3.up * (min + (rb.velocity.magnitude * jumpMagnitude)), ForceMode.Impulse);
	}
}
