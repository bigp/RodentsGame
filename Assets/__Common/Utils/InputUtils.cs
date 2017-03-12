using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputUtils : MonoBehaviour {

	public static bool IsShift() {
		return HoldAny(KeyCode.LeftShift, KeyCode.LeftShift);
	}

	public static bool IsAlt() {
		return HoldAny(KeyCode.AltGr, KeyCode.LeftAlt, KeyCode.RightAlt);
	}

	public static bool IsCTRL() {
		return HoldAny(KeyCode.LeftControl, KeyCode.RightControl);
	}

	//KeyCode.LeftApple, KeyCode.RightApple

	public static bool HoldOrPressed(KeyCode key) {
		return Input.GetKey(key) || Input.GetKeyDown(key);
	}

	public static bool HoldAny(string keyname, params KeyCode[] keys) { return Input.GetKey(keyname) || HoldAny(keys); }
	public static bool HoldAny(params KeyCode[] keys) {
		foreach (KeyCode key in keys) {
			if (Input.GetKey(key)) return true;
		}

		return false;
	}

	public static bool HoldAll(string keyname, params KeyCode[] keys) { return Input.GetKey(keyname) && HoldAll(keys); }
	public static bool HoldAll(params KeyCode[] keys) {
		foreach (KeyCode key in keys) {
			if (!Input.GetKey(key)) return false;
		}

		return true;
	}

	public static bool PressedAny(string keyname, params KeyCode[] keys) { return Input.GetKeyDown(keyname) || PressedAny(keys); }
	public static bool PressedAny(params KeyCode[] keys) {
		foreach (KeyCode key in keys) {
			if (Input.GetKeyDown(key)) return true;
		}

		return false;
	}

	public static bool PressedAll(string keyname, params KeyCode[] keys) { return Input.GetKeyDown(keyname) && PressedAll(keys); }
	public static bool PressedAll(params KeyCode[] keys) {
		foreach (KeyCode key in keys) {
			if (!Input.GetKeyDown(key)) return false;
		}

		return true;
	}

	public static bool ReleasedAny(string keyname, params KeyCode[] keys) { return Input.GetKeyUp(keyname) || ReleasedAny(keys); }
	public static bool ReleasedAny(params KeyCode[] keys) {
		foreach (KeyCode key in keys) {
			if (Input.GetKeyUp(key)) return true;
		}

		return false;
	}

	public static bool ReleasedAll(string keyname, params KeyCode[] keys) { return Input.GetKeyUp(keyname) && ReleasedAll(keys); }
	public static bool ReleasedAll(params KeyCode[] keys) {
		foreach (KeyCode key in keys) {
			if (!Input.GetKeyUp(key)) return false;
		}

		return true;
	}


}
