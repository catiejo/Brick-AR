using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour {
	public Text tapModeText;
	public Text alphaAmountText;
	public Image menuDrawer;
	[Range(1, 10)] public int speed;

	private static bool _dragEdgeDetectionMode;
	private bool _drawerIsOpen;
	private RectTransform _rt;
	private float _width;

	void Start () {
		_dragEdgeDetectionMode = true;
		tapModeText.enabled = !_dragEdgeDetectionMode;
		_drawerIsOpen = false;
		_rt = (RectTransform)menuDrawer.transform;
		_width = _rt.rect.width;
	}

	void Update () {
		var pos = _rt.anchoredPosition.x;
		var slideAmount = _width / (float)(11 - speed);

		if (_drawerIsOpen && pos < -0.5) { //
			menuDrawer.transform.position += new Vector3(slideAmount, 0, 0);
		} else if (!_drawerIsOpen && pos > -_width) {
			menuDrawer.transform.position += new Vector3(-slideAmount, 0, 0);
		}
		var alpha = 1 + Mathf.Clamp(pos / _width, -1.0f, -0.4f);
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
		tapModeText.enabled = !_dragEdgeDetectionMode;
		ScreenLog.Write("Mode changed to " + (_dragEdgeDetectionMode ? "DRAG " : "TAP ") + "mode.");
	}

	/// <summary>
	/// Gets the edge detection mode.
	/// </summary>
	/// <returns><c>DRAG</c> or <c>TAP<c>.</returns>
	public static string GetEdgeDetectionMode() {
		return _dragEdgeDetectionMode ? "DRAG" : "TAP";
	}

	/// <summary>
	/// Sets the transparency of the dynamic mesh.
	/// </summary>
	/// <param name="alpha">Alpha (0.0 to 1.0)</param>
	public void SetMeshTransparency(float alpha) {
		alphaAmountText.text = alpha.ToString("0.00"); //http://stackoverflow.com/a/6356381/5143682
		DynamicMeshController.SetShaderAlpha (alpha);
	}

	/// <summary>
	/// Changes the direction the panel is moving.
	/// </summary>
	public void ToggleDrawerl() {
		_drawerIsOpen = !_drawerIsOpen;
	}

}
