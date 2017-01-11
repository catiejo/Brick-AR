using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelController : MonoBehaviour {
	public bool panelAnchoredLeft;
	[Range(1, 10)] public int speed;

	private float _rightPosition;
	private float _leftPosition;
	private bool _isMovingLeft;
	private RectTransform _rt;
	private float _width;

	void Start() {
		_isMovingLeft = panelAnchoredLeft;
		_rt = (RectTransform)this.transform;
		_width = _rt.rect.width;
		_leftPosition = panelAnchoredLeft ? -_width : 0;
		_rightPosition = panelAnchoredLeft ? 0 : _width;
	}

	void Update () {
		var pos = _rt.anchoredPosition.x;
		var slideAmount = _width / (float)(11 - speed);
		if (_isMovingLeft && pos > _leftPosition) {
			SlidePanel (-slideAmount);
		} else if (!_isMovingLeft && pos < _rightPosition) {
			SlidePanel (slideAmount);
		}
	}

	/// <summary>
	/// Gets the anchored position of the Panel's rect transform.
	/// </summary>
	/// <returns>The anchored position.</returns>
	public float GetPosition() {
		return _rt.anchoredPosition.x;
	}

	/// <summary>
	/// Gets the width of the Panel.
	/// </summary>
	/// <returns>The width.</returns>
	public float GetWidth() {
		return _width;
	}

	/// <summary>
	/// Slides the specified panel right or left (if negative).
	/// </summary>
	/// <param name="amount">Amount in pixels.</param>
	private void SlidePanel(float amount) {
		this.transform.position += new Vector3(amount, 0, 0);
	}

	/// <summary>
	/// Changes the direction the panel is moving.
	/// </summary>
	public void TogglePanel() {
		_isMovingLeft = !_isMovingLeft;
	}
}
