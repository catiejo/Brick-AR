using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour {
	public PanelController menuDrawer;
	private bool _isOpen = false;
	private static bool _dragEdgeDetectionMode = true;

	void Update () {
		var alpha = 1 + Mathf.Clamp(menuDrawer.GetPosition() / menuDrawer.GetWidth(), -1.0f, -0.4f);
		// Enabling and disabling background so it doesn't affect UI raycast
		var background = GetComponent<Image> ();
		background.enabled = (alpha != 0);
		background.color = new Color (0, 0, 0, alpha);
	}

	/// <summary>
	/// Changes the edge detection mode between tap and drag.
	/// </summary>
	/// <param name="mode"><c>DRAG</c> is 0, <c>TAP</c> is 1 (or any non-zero value).</param>
	public void ChangeEdgeDetectionMode(int mode) {
		_dragEdgeDetectionMode = (mode == 0);
		ScreenLog.Write("Mode changed to " + (_dragEdgeDetectionMode ? "DRAG " : "TAP ") + "mode.");
	}

	/// <summary>
	/// Gets the edge detection mode.
	/// </summary>
	/// <returns><c>DRAG</c> or <c>TAP<c>.</returns>
	public static string GetEdgeDetectionMode() {
		return _dragEdgeDetectionMode ? "DRAG" : "TAP";
	}
}
