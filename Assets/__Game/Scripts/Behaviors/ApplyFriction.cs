using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyFriction : Physics_Base {

	public float speedMaxLow = 3f;
	public float speedMaxHigh = 5f;
	public float frictionAbove = 0.9f;
	public float frictionUnder = 0.97f;

	public bool isShifted = false;
	
	// Update is called once per frame
	void Update () {
		//if(!checkGround.isOnGround) return;

		Vector2 speedHorizontal = new Vector2(rb.velocity.x, rb.velocity.z);
		float friction = frictionUnder;
		float max = isShifted ? speedMaxHigh : speedMaxLow;

		if (speedHorizontal.magnitude > max) {
			friction = frictionAbove;
		}

		speedHorizontal.x *= friction;
		speedHorizontal.y *= friction;

		rb.velocity = new Vector3(speedHorizontal.x, rb.velocity.y, speedHorizontal.y);
	}
}
