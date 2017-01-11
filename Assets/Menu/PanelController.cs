using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelController : MonoBehaviour {
	public bool slideLeft;
	[Range(0.01f, 1.0f)] public float speed;
	private bool _isOpen = false;
	private float _width;
	private RectTransform rt;
	private float _openPosition;
	private float _closedPosition;

	void Start() {
		rt = (RectTransform)this.transform;
		_width = rt.rect.width;
		_openPosition = slideLeft ? -_width : 0.5f;
		_closedPosition = slideLeft ? 0 : _width;
	}

	void Update () {
		var pos = rt.anchoredPosition.x;
		var slideAmount = _width / 7.0f; //TODO: incorporate speed
		if (_isOpen && pos > _openPosition) { //0.5f for error
			SlidePanel (-slideAmount);
		} else if (!_isOpen && pos < _closedPosition) {
			SlidePanel (slideAmount);
		}
	}

	public void TogglePanel(bool toggle) {
		_isOpen = toggle;
	}

	public void TogglePanel() {
		_isOpen = !_isOpen;
	}

	public float GetPosition() {
		return rt.anchoredPosition.x;
	}

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
}
