using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHooks : CoreScript {

	[HideInInspector] public GameObject canvasUI;
	[HideInInspector] public ParticleSystem hearts;

	// Use this for initialization
	public override void Start () {
		base.Start();
		
		canvasUI = GameObject.Find("UI/CanvasUI");
		hearts = ObjectUtils.FindByName<ParticleSystem>(canvasUI, "HeartParticles");
		trace(hearts);
		if(hearts!=null) {
			hearts.enableEmission = false;
		}
	}

	public override void AddEvents() {
		GameEvents.AddListener(GameEnums.BED_ENTER, OnBedEnter);
		GameEvents.AddListener(GameEnums.BED_EXIT, OnBedExit);
	}

	private void OnBedEnter(GameEvents.Params obj) {
		if (hearts==null) return;
		hearts.enableEmission = true;
	}

	private void OnBedExit(GameEvents.Params obj) {
		if (hearts == null) return;
		hearts.enableEmission = false;
	}
	
	// Update is called once per frame
	//void Update () {
		
	//}
}
