using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExtensionMethods;

public class SpriteAnimator : MonoBehaviour {

	public Sprite[] sprites;
	public float frameRate = 1;
	public int frame = 0;

	private float _frameRateCounter = 0;
	private Image _image;

	public Sprite currentSprite { get { return sprites[frame]; } }

	// Use this for initialization
	void Start () {
		_image = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
		if(sprites==null || sprites.Length==0) return;

		_frameRateCounter += frameRate * Time.deltaTime;

		while(_frameRateCounter > 1.0f) {
			frame++;
			_frameRateCounter -= 1.0f;
		}

		while(_frameRateCounter < -1.0f) {
			frame--;
			_frameRateCounter += 1.0f;
		}

		frame = frame.Mod(sprites.Length);

		_image.sprite = currentSprite;
	}
}
