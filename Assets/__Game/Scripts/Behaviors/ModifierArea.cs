using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierArea : CoreScript {

	private void OnTriggerStay(Collider other) {
		trace(other);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
