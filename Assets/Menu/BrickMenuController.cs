using UnityEngine;
using System.Collections;

public class BrickMenuController : MonoBehaviour {
	public Material[] brickMaterials;
	public Material[] brickMaterialsOccluded;

	private int _currentMaterial;
	private Surface _trackedSurface;

	void Start () {
		_currentMaterial = 4;
	}

	void Update () {
		// Ensures the menu is always centered over the selected surface
		if (_trackedSurface) {
			transform.position = Camera.main.WorldToScreenPoint(_trackedSurface.transform.position);
		}
		if (SelectableBehavior.GetSelectedSurface() && _trackedSurface != SelectableBehavior.GetSelectedSurface()) {
			_trackedSurface = SelectableBehavior.GetSelectedSurface();
			ExpandMenu ();
		}
	}

	/// <summary>
	/// Gets the current material.
	/// </summary>
	/// <returns>The current material.</returns>
	public Material GetCurrentMaterial () {
		return GetMaterial (_currentMaterial);
	}

	public Material GetMaterialByColor(string color) {
		switch (color) {
			case "Beige":
				return GetMaterial(0);
			case "Green":
				return GetMaterial(1);
			case "Purple":
				return GetMaterial(2);
			case "Yellow":
				return GetMaterial(3);
			default:
				return GetMaterial(4);
		}
	}

	/// <summary>
	/// This function is called when one of the menu options (i.e. brick colors) has been selected by the user.
	/// </summary>
	/// <param name="option">Brick color.</param>
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
		default:
			_currentMaterial = 4; // Default material
			break;
		}
		CollapseMenu ();
	}

	/// <summary>
	/// Collapses the brick menu and deselects the selected surface.
	/// </summary>
	private void CollapseMenu() {
		if (_trackedSurface) {
			_trackedSurface.SetMaterial (GetCurrentMaterial ());
			SelectableBehavior.DeselectSurface ();
			_trackedSurface = null;
		}
		gameObject.GetComponent<Animation> ().Play ("spiral-in");
	}

	/// <summary>
	/// Expands the brick menu.
	/// </summary>
	private void ExpandMenu() {
		gameObject.GetComponent<Animation> ().Play ("spiral-out");
	}

	/// <summary>
	/// Gets the material based on occlusion setting.
	/// </summary>
	/// <returns>The material.</returns>
	/// <param name="index">Array index of material.</param>
	private Material GetMaterial(int index) {
		if (OcclusionController.IsOccluding ()) {
			return brickMaterialsOccluded [index];
		} else {
			return brickMaterials [index];
		}
	}
}

