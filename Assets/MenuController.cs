using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuController : MonoBehaviour {
	public Material[] brickMaterials;

	private int _currentMaterial = 0;
	private bool _isAlreadyOpen = false;

	public Material GetCurrentMaterial () {
		return brickMaterials [_currentMaterial];
	}

	public void MenuClicked() {
		if (_isAlreadyOpen) {
			CollapseMenu ();
		} else {
			ExpandMenu ();
		}
	}
		
	public void SelectOption(Button option) {
		option.GetComponent<RectTransform> ().SetSiblingIndex (brickMaterials.Length - 1);
		// NOTE: needs to match order of items in brickMaterials
		switch (option.name) {
			case "Beige":
				_currentMaterial = 0;
				break;
			case "Yellow":
				_currentMaterial = 1;
				break;
			case "Green":
				_currentMaterial = 2;
				break;
			case "Purple":
				_currentMaterial = 3;
				break;
		}
		CollapseMenu();
	}

	private void CollapseMenu() {
		if (_isAlreadyOpen) {
			gameObject.GetComponent<Animation> ().Play ("collapse-picker");
		}
		_isAlreadyOpen = false;
	}

	private void ExpandMenu() {
		if (!_isAlreadyOpen) {
			gameObject.GetComponent<Animation> ().Play ("expand-picker");
		}
		_isAlreadyOpen = true;
	}

}

