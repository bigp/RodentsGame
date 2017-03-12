using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ModifierArea : CoreScript {

	public GameEnums enumEnter;
	public GameEnums enumExit;
	public GameEnums enumStay;

	private void OnTriggerStay(Collider other) {
		//trace(other);
	}

	private void OnTriggerEnter(Collider other) {
		//trace(other);
	}

	private void OnTriggerExit(Collider other) {

	}

	// Update is called once per frame
	void Update () {
		
	}
}
