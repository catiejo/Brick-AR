using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {
	public Image menuDrawer;
	private Color _color = new Color(0, 0, 0, 0);
	private bool _isOpen = false;
	private static bool _dragEdgeDetectionMode = true;

	void Update () {
		if (_isOpen && menuDrawer.transform.position.x < 0.0f) {
			SlideMenu (100f);
		} else if (!_isOpen && menuDrawer.transform.position.x > -500.0f) {
			SlideMenu (-100f);
		}
		var alpha = 1 + Mathf.Clamp(menuDrawer.transform.position.x / 500.0f, -1.0f, -0.4f);
		_color = new Color (0, 0, 0, alpha);
		GetComponent<Image> ().color = _color;
	}

	/// <summary>
	/// Toggles the boolean that opens/closes the drawer menu.
	/// </summary>
	public void ToggleMenu() {
		_isOpen = !_isOpen;
	}

	/// <summary>
	/// Changes the edge detection mode between tap and drag.
	/// </summary>
	/// <param name="mode"><c>DRAG</c> is 0, <c>TAP</c> is 1 (or any non-zero value).</param>
	public void ChangeEdgeDetectionMode(int mode) {
		_dragEdgeDetectionMode = (mode == 0);
	}

	/// <summary>
	/// Gets the edge detection mode.
	/// </summary>
	/// <returns><c>DRAG</c> or <c>TAP<c>.</returns>
	public static string GetEdgeDetectionMode() {
		return _dragEdgeDetectionMode ? "DRAG" : "TAP";
	}

	/// <summary>
	/// Slides the menu right or left (if negative).
	/// </summary>
	/// <param name="amount">Amount in pixels.</param>
	private void SlideMenu(float amount) {
		menuDrawer.transform.position += new Vector3(amount, 0, 0);
	}
}
