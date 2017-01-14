using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenLog : MonoBehaviour {
	private static Text _instance;

	void Awake() {
		_instance = this.GetComponent<Text>(); //set our static reference to our newly initialized instance
		if (!_instance) {
			Debug.LogError("!!! ScreenLog must be attached to a gameobject containing a UI Text component. !!!");
		}
	}

	public static void Write(string message) {
		_instance.text += message + "\n";
	}

	public static void Clear() {
		_instance.text = "";
	}

}
