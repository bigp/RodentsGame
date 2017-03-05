using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround : Physics_Base {

	private float _distToGround;
	private Vector3[] _offsetsToCheck;

	[HideInInspector] public bool isOnGroundNow;
	[HideInInspector] public bool wasOnGround;

	public bool isOnGround { get { return isOnGroundNow || wasOnGround; } }
	// Use this for initialization
	public override void Start () {
		base.Start();

		Bounds b = col.bounds;
		float radius = b.extents.magnitude;
		_distToGround = b.extents.y;

		int directions = 8;
		float angle = (360 / directions); // * Mathf.Deg2Rad;
		_offsetsToCheck = new Vector3[directions+1];
		_offsetsToCheck[0] = Vector3.zero;
		for (int d=0; d<directions; d++) {
			_offsetsToCheck[d+1] = Quaternion.Euler(0, d*angle, 0) * Vector3.forward * radius;
		}
		//_radius = b.
	}

	public void Update() {
		wasOnGround = isOnGroundNow;
		isOnGroundNow = IsGrounded();
		if(isOnGroundNow) wasOnGround = true;
	}

	private bool IsGrounded() {
		//if(rb.velocity.y>0) return false;

		int i=-1;
		foreach (Vector3 offset in _offsetsToCheck) {
			i++;
			var touchesGround = Physics.Raycast(transform.position + offset, Vector3.down, _distToGround + 0.01f); //rb.velocity.y
			if (touchesGround) {
				//trace("Touches: " + offset.ToString());
				return true;
			}
		}

		//trace("!!!!");

		return false;
	}
	
}
