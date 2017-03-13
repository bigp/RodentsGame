using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;

public static class CoroutineEx {
	
	private static IEnumerable<System.Object> Wait(float delay) {
		yield return new WaitForSeconds(delay);
	}

	private static IEnumerable Wait(Action action, float delay) {
		yield return new WaitForSeconds(delay);
		action();
	}

	public static Coroutine StartCoroutine(this MonoBehaviour instance, IEnumerable coroutine) {
		return instance.StartCoroutine(coroutine.GetEnumerator());
	}

	public static Coroutine StartCoroutine(this MonoBehaviour instance, IEnumerable<System.Object> coroutine, float delay) {
		return instance.StartCoroutine(Wait(delay).Concat(coroutine));
	}

	public static Coroutine StartCoroutine(this MonoBehaviour instance, Action action, float delay) {
		return instance.StartCoroutine(Wait(action, delay));
	}

	//Pierre: Added coroutine methods that doesn't depend on providing a MonoBehaviour (Defaults to GameManager):
	private static MonoBehaviour monoInstance { get { return GameObject.FindObjectOfType<MonoBehaviour>(); } }

	public static Coroutine StartCoroutine(this IEnumerable coroutine) {
		return monoInstance.StartCoroutine(coroutine.GetEnumerator());
	}

	public static Coroutine StartCoroutine(this IEnumerable<System.Object> coroutine, float delay) {
		return monoInstance.StartCoroutine(Wait(delay).Concat(coroutine));
	}

	public static Coroutine StartCoroutine(this Action action, float delay) {
		return monoInstance.StartCoroutine(Wait(action, delay));
	}

	public static Coroutine WaitForFrames(int frameCount=1, Action onFramesComplete=null) {
		return monoInstance.StartCoroutine(Frames(frameCount, onFramesComplete));
	}

	private static IEnumerator Frames(int frameCount, Action onFramesComplete) {
		if (frameCount <= 0) frameCount = 1;

		while (frameCount > 0) {
			frameCount--;
			yield return null;
		}

		if(onFramesComplete!=null) onFramesComplete();
	}
}
