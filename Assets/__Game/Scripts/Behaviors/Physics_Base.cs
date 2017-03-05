using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physics_Base : CoreScript {

	[HideInInspector] public CheckGround checkGround;

	public override void Start() {
		base.Start();

		checkGround = this.GetComponent<CheckGround>();
	}
}
