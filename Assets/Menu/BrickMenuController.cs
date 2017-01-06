﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BrickMenuController : MonoBehaviour {
	public Material[] brickMaterials;
	public Text debug;

	private int _currentMaterial = 0; //defaults to beige
	private Surface _trackedSurface;

	void Update() {
		// Ensures the menu is always centered over the selected surface
		if (_trackedSurface) {
			transform.position = Camera.main.WorldToScreenPoint(_trackedSurface.transform.position);
		}
		if (SelectableBehavior.selectedSurface && _trackedSurface != SelectableBehavior.selectedSurface) {
			_trackedSurface = SelectableBehavior.selectedSurface;
			ExpandMenu ();
		}
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
	public void ExpandMenu() {
		gameObject.GetComponent<Animation> ().Play ("spiral-out");
	}

	/// <summary>
	/// Gets the current material.
	/// </summary>
	/// <returns>The current material.</returns>
	public Material GetCurrentMaterial () {
		return brickMaterials [_currentMaterial];
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
		}
		CollapseMenu ();
	}
}
