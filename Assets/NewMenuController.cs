using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewMenuController : MonoBehaviour {
	public Material[] brickMaterials;
	private int _currentMaterial = -1;

	public Material GetCurrentMaterial () {
		return brickMaterials [_currentMaterial];
	}

	public bool HasColor() {
		return _currentMaterial != -1;
	}
		
	public void SelectOption(Button option) {
		//NOTE: indices need to match order of items in brickMaterials (e.g. beige material is located at index 0)
		switch (option.name) {
			case "Beige":
				_currentMaterial = 0;
				break;
			case "Green":
				_currentMaterial = 1;
				break;
			case "Purple":
				_currentMaterial = 2;
				break;
			case "Yellow":
				_currentMaterial = 3;
				break;
//			case "Delete":
//				break;
		}
		gameObject.GetComponent<Animation> ().Play ("collapse-picker");
	}

	public void Collapse() {
		Destroy (gameObject);
	}
}

