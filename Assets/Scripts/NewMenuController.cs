using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewMenuController : MonoBehaviour {
	public Text debug;
	private Surface _trackedSurface;
	public Material[] brickMaterials;
	private int _currentMaterial = 0; //defaults to beige

	public Material GetCurrentMaterial () {
		return brickMaterials [_currentMaterial];
	}

	public void Update() {
		if (_trackedSurface) {
			transform.position = Camera.main.WorldToScreenPoint(_trackedSurface.transform.position);
		}
		if (Surface.selectedSurface && _trackedSurface != Surface.selectedSurface) {
			_trackedSurface = Surface.selectedSurface;
			ExpandMenu ();
		}
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

	public void ExpandMenu() {
		gameObject.GetComponent<Animation> ().Play ("spiral-out");
	}

	private void CollapseMenu() {
		if (_trackedSurface) {
			_trackedSurface.SetMaterial (GetCurrentMaterial ());
			_trackedSurface.DeselectSurface ();
			_trackedSurface = null;
		}
		gameObject.GetComponent<Animation> ().Play ("spiral-in");
	}
}

