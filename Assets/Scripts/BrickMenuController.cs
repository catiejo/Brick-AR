using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BrickMenuController : MonoBehaviour {
	public Material[] brickMaterials;
	public Text debug;

	private int _currentMaterial = 0; //defaults to beige
	private Surface _trackedSurface;

	void Update() {
		if (_trackedSurface) {
			transform.position = Camera.main.WorldToScreenPoint(_trackedSurface.transform.position);
		}
		if (SelectableBehavior.selectedSurface && _trackedSurface != SelectableBehavior.selectedSurface) {
			_trackedSurface = SelectableBehavior.selectedSurface;
			ExpandMenu ();
		}
	}

	private void CollapseMenu() {
		if (_trackedSurface) {
			_trackedSurface.SetMaterial (GetCurrentMaterial ());
			SelectableBehavior.DeselectSurface ();
			_trackedSurface = null;
		}
		gameObject.GetComponent<Animation> ().Play ("spiral-in");
	}

	public void ExpandMenu() {
		gameObject.GetComponent<Animation> ().Play ("spiral-out");
	}

	public Material GetCurrentMaterial () {
		return brickMaterials [_currentMaterial];
	}
		
	public void SelectOption(string option) {
		//NOTE: indices need to match order of items in brickMaterials (e.g. beige material is located at index 0)
		switch (option) {
			case "Delete":
				_trackedSurface.Undo();
				break;
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
		}
		CollapseMenu ();
	}
}

