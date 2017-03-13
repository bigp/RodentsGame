using UnityEngine;
using System.Collections;
using ExtensionMethods;

public class ObjectUtils {

	public static T FindByComponent<T>(Component comp) where T : Component {
		if (comp == null) return null;
		return FindByComponent<T>(comp.transform);
	}

	public static T FindByComponent<T>(GameObject go) where T : Component {
		if (go == null) return null;
		return FindByComponent<T>(go.transform);
	}

	public static T FindByComponent<T>(Transform transform) where T : Component {
		if(transform==null) return null;

		T comp = transform.GetComponent<T>();

		if(comp==null) {
			foreach(Transform child in transform) {
				comp = FindByComponent<T>(child);
				if(comp!=null) return comp;
			}
		}

		return comp;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public static GameObject FindByName(Component comp, string name) {
		if (comp == null) return null;
		return FindByName(comp.transform, name);
	}

	public static GameObject FindByName(GameObject go, string name) {
		if (go == null) return null;
		return FindByName(go.transform, name);
	}

	public static GameObject FindByName(Transform transform, string name) {
		return _FindByName(transform, name);
	}

	public static T FindByName<T>(Component comp, string name) where T : Component {
		if (comp == null) return null;
		return FindByName<T>(comp.transform, name);
	}

	public static T FindByName<T>(GameObject go, string name) where T : Component {
		if (go == null) return null;
		return FindByName<T>(go.transform, name);
	}

	public static T FindByName<T>(Transform transform, string name) where T : Component {
		GameObject go = _FindByName(transform, name);
		if (go == null) return null;

		return go.GetComponent<T>();
	}

	/**
	 * Now with more HMMMPH! This allows search by child-name AND child-index!
	 * (ex: "Monsters/0/Eyes/3");
	 */
	private static GameObject _FindByName(Transform transform, string name) {
		if (transform == null) return null;

		string[] names = name.Split('/');
		Transform currentTrans = transform;

		//Replaced with a for-loop of the names:
		for (int id=0; id<names.Length; id++) {
			string currentName = names[id].ToLower(); //<-- made this case-insensitive.
			int namedIndex = -1;
			if (currentName.IsNumeric()) int.TryParse(currentName, out namedIndex);

			if (namedIndex > -1 && namedIndex < currentTrans.childCount) {
				currentTrans = currentTrans.GetChild(namedIndex);
				continue;
			}

			currentTrans = _FindByNameFirstOccurance(currentTrans, currentName);
			
			if (currentTrans==null) return null;
		}

		if(currentTrans==null) return null;

		return currentTrans.gameObject;
	}

	private static Transform _FindByNameFirstOccurance(Transform transform, string currentName) {
		//Iterate each at this level first:
		foreach (Transform childTrans in transform) {
			if (childTrans.name.ToLower() != currentName) {
				continue;
			}

			return childTrans;
		}

		//Still nothing? Iterate one-level deeper:
		foreach (Transform childTrans in transform) {
			Transform found = _FindByNameFirstOccurance(childTrans, currentName);
			if(found!=null) return found;
		}

		return null;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	public static GameObject CloneWithOffset(GameObject source, float xOffset=0, float yOffset=0, float zOffset=0) {
		Transform trans = source.transform.parent;
		GameObject dup = (GameObject)GameObject.Instantiate(source, trans, true);
		Vector3 pos = dup.transform.localPosition;
		pos.x += xOffset;
		pos.y += yOffset;
		pos.z += zOffset;

		dup.transform.localPosition = pos;
		return dup;
	}
}
