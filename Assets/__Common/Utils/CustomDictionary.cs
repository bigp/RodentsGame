using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

[Serializable]
public class CustomDictionary<K, T> {
	[SerializeField] private List<K> _keys = new List<K>();
	[SerializeField] private List<T> _values = new List<T>();
	internal bool isDestroyed = false;

	public CustomDictionary() {
		if (_keys == null) _keys = new List<K>();
		if (_values == null) _values = new List<T>();
	}

	public virtual void Clear() {
		_keys.Clear();
		_values.Clear();
	}

	public virtual void Destroy() {
		if (_keys == null) return;
		Clear();
		isDestroyed = true;
		_keys = null;
		_values = null;
	}

	public K KeyOf(T pValue) {
		int index = _values.IndexOf(pValue);
		return index == -1 ? default(K) : _keys[index];
	}

	public bool RemoveKey(K pKey) {
		int index = _keys.IndexOf(pKey);
		if (index == -1) return false;
		_keys.RemoveAt(index);
		_values.RemoveAt(index);
		return true;
	}

	public bool RemoveValue(T pValue) {
		int index = _values.IndexOf(pValue);
		if (index == -1) return false;
		_values.RemoveAt(index);
		_keys.RemoveAt(index);
		return true;
	}

	public int IndexOfKey(K pKey) {
		return _keys.IndexOf(pKey);
	}

	public int IndexOfValue(T pValue) {
		return _values.IndexOf(pValue);
	}

	public virtual T Lookup(K pKey) { return this[pKey]; }
	public virtual void SetKey(K pKey, T pValue) { this[pKey] = pValue; }

	public virtual T this[K key] {
		get {
			int index = _keys.IndexOf(key);
			return index==-1 ? default(T) : _values[index];
		}
		set {
			if (value == null) {
				RemoveKey(key);
			} else {
				int index = _keys.IndexOf(key);
				if (index > -1) _values[index] = value;
				else {
					_keys.Add(key);
					_values.Add(value);
				}
			}
		}
	}

	public bool HasKey(K pKey) { return _keys.IndexOf(pKey) > -1; }
	public bool HasValue(T pValue) { return _values.IndexOf(pValue) > -1; }

	public int Count { get { return _keys.Count; } }
	public K[] Keys { get { return _keys.ToArray(); } }
	public T[] Values { get { return _values.ToArray(); } }

	public List<K> KeysList {
		get {
			List<K> copyKeys = new List<K>(_keys);
			return copyKeys;
		}
	}

	public List<T> ValuesList {
		get {
			List<T> copyValues = new List<T>(_values);
			return copyValues;
		}
	}
}