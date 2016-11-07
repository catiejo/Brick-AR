using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BrickButtons : MonoBehaviour {
	public Button[] buttons;

	public void ExpandPicker() {
		gameObject.GetComponent<Animation> ().Play ("expand-picker");
	}
		
	public void Select(Button b) {
		b.GetComponent<RectTransform> ().SetSiblingIndex (buttons.Length - 1);
		gameObject.GetComponent<Animation> ().Play ("collapse-picker");
	}

}

