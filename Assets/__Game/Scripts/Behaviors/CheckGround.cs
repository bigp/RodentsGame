using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround : Physics_Base {

	private float distToGround;
	[HideInInspector] public bool isOnGroundNow;
	[HideInInspector] public bool wasOnGround;

	public bool isOnGround { get { return isOnGroundNow || wasOnGround; } }
	// Use this for initialization
	public override void Start () {
		base.Start();

		distToGround = col.bounds.extents.y;
	}

	public void Update() {
		wasOnGround = isOnGroundNow;
		isOnGroundNow = IsGrounded();
		if(isOnGroundNow) wasOnGround = true;
	}

	private bool IsGrounded() {
		if(rb.velocity.y>0) return false;
		return Physics.Raycast(transform.position, Vector3.down, distToGround + 0.01f); //rb.velocity.y
	}
	
}
