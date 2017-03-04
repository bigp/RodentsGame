using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBoundsReset : MonoBehaviour {

	public GameObject respawnPosition;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.R)) {
			ChangePosition(PlayerManager.Instance);
		}
	}
	
	private void OnTriggerEnter(Collider other) {
		Debug.Log(other);

		var opposed = new Vector3(1, -1000000, 1) * Time.deltaTime;

		var ctrl = other.GetComponent<CharacterController>();
		if(ctrl!=null) {
			//ctrl.velocity = opposed;
			ctrl.SimpleMove(opposed);
		} else {
			var body = other.GetComponent<Rigidbody>();
			//opposed.Scale(body.GetPointVelocity(Vector3.zero));
			//body.isKinematic = false;
			body.AddForce(opposed);
			Debug.Log(opposed);
		}

		//body.AddRelativeForce(opposed, ForceMode.Impulse);
		//StartCoroutine(__ChangePosition(other));
		ChangePosition(other);
	}

	private IEnumerator __ChangePosition(Collider other) {
		yield return new WaitForSeconds(0.5f);
		ChangePosition(other);
	}

	void ChangePosition(Component obj) {
		GameObject randomKid = Randomize.PickRandomChild(respawnPosition);
		Transform randomTrans = randomKid.transform;
		Transform objTrans = obj.transform;
		objTrans.position = randomTrans.position;

		Rigidbody rigid = obj.GetComponent<Rigidbody>();
		if(rigid!=null) {
			rigid.velocity = new Vector3(0, 1f, 0);
		}
	}
}
