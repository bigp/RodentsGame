using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

public class ApplyGravityOnTouch : Physics_Base {
	// Use this for initialization
	public override void Start () {
		base.Start();
	}

	private void OnCollisionEnter(Collision collision) {
		if(collision.gameObject.tag.ToLower()=="player") {
			this.rb.useGravity = true;
			this.rb.velocity = collision.rigidbody.velocity + new Vector3().Randomize(collision.rigidbody.velocity.magnitude*3);
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
