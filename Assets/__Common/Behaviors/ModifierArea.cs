using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ModifierArea : CoreScript {

	public GameEnums enumEnter;
	public GameEnums enumExit;
	public GameEnums enumStay;

	private void OnTriggerEnter(Collider other) {
		GameEvents.Dispatch(enumEnter);
	}

	private void OnTriggerExit(Collider other) {
		GameEvents.Dispatch(enumExit);
	}

	private void OnTriggerStay(Collider other) {
		GameEvents.Dispatch(enumStay);
	}
}
