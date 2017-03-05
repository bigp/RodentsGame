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
		var otherBody = other.GetComponent<Rigidbody>();

		//opposed.Scale(body.GetPointVelocity(Vector3.zero));
		//body.isKinematic = false;
		other.transform.localPosition = other.transform.localPosition + new Vector3(0, 0.5f, 0);
		otherBody.velocity = Vector3.zero;
		otherBody.AddForce(Vector3.up * 10, ForceMode.Impulse);
		
		//body.AddRelativeForce(opposed, ForceMode.Impulse);
		StartCoroutine(__ChangePosition(other));
		//ChangePosition(other);
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
