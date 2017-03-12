using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Callback = System.Action<GameEvents.Params>;
using CallbackList = System.Collections.Generic.List<System.Action<GameEvents.Params>>;

public class GameEvents : Singleton<GameEvents> {
	class Listeners : CustomDictionary<GameEnums, CallbackList> { }
	class Sandboxes : CustomDictionary<string, Listeners> { }

	public class Params {
		public string tag;
		public GameEnums type;
		public object[] args;
	}

	private static string TAG_ALL = "ALL";
	private static string TAG_MASTER = "MASTER";

	Sandboxes sandboxes = new Sandboxes();
	Listeners master = new Listeners();

	// Use this for initialization
	void Start () {
		sandboxes[TAG_MASTER] = master;
	}

	/////////////////////////////////////////////////////////////////////

	public static void AddListener(string tag, GameEnums type, Callback cb) {
		if (tag == null) tag = TAG_MASTER;
		if (tag == TAG_ALL) throw new Exception("GameEvents 'all' is reserved for internal dispatch of ALL tags!");

		Instance.AddListener(type, cb, tag);
	}

	public void AddListener(GameEnums type, Callback cb, string tag) {
		Listeners listeners = sandboxes[tag];
		if(listeners==null) {
			listeners = sandboxes[tag] = new Listeners();
		}

		CallbackList callbacks = listeners[type];
		if(callbacks==null) {
			callbacks = listeners[type] = new CallbackList();
		}

		callbacks.Add(cb);
	}

	//public void RemoveListener

	public static void Dispatch(GameEnums type, string tag = null, params object[] args) {
		Params eventParams = new Params();

		if (tag == null) tag = TAG_ALL;

		eventParams.type = type;
		eventParams.args = args;
		eventParams.tag = tag;

		Instance.Dispatch(eventParams);
	}

	/////////////////////////////////////////////////////////////////////
	
	public void Dispatch(Params eventParams) {
		if (eventParams.tag == TAG_ALL) {
			foreach (Listeners listeners in sandboxes.Values) {
				_Dispatch(eventParams, listeners);
			}
			return;
		}

		_Dispatch(eventParams, sandboxes[eventParams.tag]);
	}

	void _Dispatch(Params eventParams, Listeners listeners) {
		if (listeners==null || !listeners.HasKey(eventParams.type)) return;

		CallbackList callbacks = listeners[eventParams.type];
		foreach (Callback cb in callbacks) {
			cb(eventParams);
		}
	}
}
