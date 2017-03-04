using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobbing : CoreScript {

	public float idleFreq = 1;
	public float idleAmp = 1;
	public float walkFreq = 2;
	public float walkAmp = 2;
	public float runFreq = 4;
	public float runAmp = 2.5f;
	public float tiltSmoothness = 50f;
	public float tiltAmplitude = 50f;
	public float tiltMax = 20f;

	public GameObject head;
	public Camera headCamera;
	private Vector3 headPos;

	private float _progress = 0;
	
	private float _camTiltSmooth = 0;

	private CheckGround _checkGround;

	// Use this for initialization
	public override void Start () {
		base.Start();

		headPos = head.transform.localPosition;
		_checkGround = GetComponent<CheckGround>();
	}
	
	// Update is called once per frame
	void Update () {
		_progress += Time.deltaTime * idleFreq;

		float offsetY = Mathf.Sin(_progress) * idleAmp * 0.01f;

		head.transform.localPosition = headPos + new Vector3(0, offsetY, 0);
		

		Vector3 vecCamFront = transform.forward.normalized;
		Vector3 vecVelocity = rb.velocity.normalized;
		Quaternion camQuarternion = Quaternion.FromToRotation(vecCamFront, vecVelocity);
		float camTiltMax = tiltMax * 0.01f;
		float camTiltClamped = Mathf.Clamp(-camQuarternion.y, -camTiltMax, camTiltMax);
		if(_checkGround!=null && !_checkGround.isOnGround) camTiltClamped = 0;
		_camTiltSmooth += (camTiltClamped-_camTiltSmooth) * (tiltSmoothness*0.01f) * Time.deltaTime;
		
		head.transform.localRotation = Quaternion.Euler(0, 0, _camTiltSmooth * tiltAmplitude);
		//head.transform.localRotation = 
	}
}
