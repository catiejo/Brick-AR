using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuController : MonoBehaviour {
	public Texture[] brickTextures;

	private int _currentTexture = 0;
	private bool _isAlreadyOpen = false;

	public Texture GetCurrentTexture () {
		return brickTextures [_currentTexture];
	}

	public void MenuClicked() {
		if (_isAlreadyOpen) {
			CollapseMenu ();
		} else {
			ExpandMenu ();
		}
	}
		
	public void SelectOption(Button option) {
		option.GetComponent<RectTransform> ().SetSiblingIndex (brickTextures.Length - 1);
		// NOTE: needs to match order of items in brickTextures
		switch (option.name) {
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

