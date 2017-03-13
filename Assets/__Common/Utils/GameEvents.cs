using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Callback = System.Action<GameEvents.Params>;
using CallbackList = System.Collections.Generic.List<System.Action<GameEvents.Params>>;

public class GameEvents : Singleton<GameEvents> {
	[Serializable]
	class Listeners : CustomDictionary<GameEnums, CallbackList> { }

	[Serializable]
	class Sandboxes : CustomDictionary<string, Listeners> { }

	public class Params {
		public string tag;
		public GameEnums type;
		public object[] args;
	}

	private static string TAG_ALL = "all";
	private static string TAG_MASTER = "master";

	Sandboxes sandboxes;
	Listeners master;
	
	// Use this for initialization
	public override void Start () {
		base.Start();
		
		sandboxes = new Sandboxes();
		sandboxes[TAG_MASTER] = master = new Listeners();
	}

	/////////////////////////////////////////////////////////////////////

	public static void AddListener(GameEnums type, Callback cb) {
		AddListener(null, type, cb);
	}

	public static void AddListener(string tag, GameEnums type, Callback cb) {
		if (tag == null) tag = TAG_MASTER;
		if (tag == TAG_ALL) throw new Exception("GameEvents 'all' is reserved for internal dispatch of ALL tags!");

		Instance.AddListener(type, cb, tag);
	}

	public void AddListener(GameEnums type, Callback cb, string tag) {
		tag = tag.ToLower();

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
		if(eventParams==null || eventParams.tag==null) {
			Debug.LogError("eventParams or tag is null, cannot dispatch event/enum!");
			return;
		}

		eventParams.tag = eventParams.tag.ToLower();

		Listeners listeners;
		if (eventParams.tag == TAG_ALL) {
			foreach (string tag in sandboxes.Keys) {
				listeners = sandboxes[tag];
				_Dispatch(eventParams, listeners);
			}
			return;
		}

		listeners = sandboxes[eventParams.tag];
		_Dispatch(eventParams, listeners);
	}

	void _Dispatch(Params eventParams, Listeners listeners) {
		if (listeners==null || !listeners.HasKey(eventParams.type)) return;

		
		CallbackList callbacks = listeners[eventParams.type];
		foreach (Callback cb in callbacks) {
			cb(eventParams);
		}
	}
}
