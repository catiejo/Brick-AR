using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BrickButtons : MonoBehaviour {
	public Texture[] bricks;
	private int _currentTexture = 0;
	public bool isAlreadyOpen = false;

	public void PickerClicked() {
		if (isAlreadyOpen) {
			CollapsePicker ();
		} else {
			ExpandPicker ();
		}
	}

	public void CollapsePicker() {
		if (isAlreadyOpen) {
			gameObject.GetComponent<Animation> ().Play ("collapse-picker");
		}
		isAlreadyOpen = false;
	}

	public void ExpandPicker() {
		if (!isAlreadyOpen) {
			gameObject.GetComponent<Animation> ().Play ("expand-picker");
		}
		isAlreadyOpen = true;
	}
		
	public void Select(Button b) {
		b.GetComponent<RectTransform> ().SetSiblingIndex (bricks.Length - 1);
		switch (b.name) {
			case "Beige":
				_currentTexture = 0;
				break;
			case "Yellow":
				_currentTexture = 1;
				break;
			case "Green":
				_currentTexture = 2;
				break;
			case "Purple":
				_currentTexture = 3;
				break;
		}
		CollapsePicker();
	}

	public Texture GetCurrentTexture () {
		return bricks [_currentTexture];
	}

}

