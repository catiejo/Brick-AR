using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelController : MonoBehaviour {
	public bool slideLeft;
	[Range(0.01f, 1.0f)] public float speed;
	private bool _isOpen = false;
	private float _width;
	private RectTransform rt;

	void Start() {
		//FIXME: this won't get updated if open/closed position changes
		rt = (RectTransform)this.transform;
		_width = rt.rect.width;
		ScreenLog.Write ("Width is: " + _width);

	}

	void Update () {
		var slideAmount = _width / 7.0f; //TODO: incorporate speed
		if (slideLeft) {
			ScreenLog.Write ("slideLeft is true");
//			if (_isOpen && this.transform.position.x < _width) {
//				SlidePanel (slideAmount);
//			} else if (!_isOpen && this.transform.position.x > 0) {
//				SlidePanel (-slideAmount);
//			}
		} else {
			ScreenLog.Write (rt.position.x.ToString ()); //WHY WHY WHY IS THIS NOT THE VALUE IN THE INSPECTOR
			if (_isOpen && this.transform.position.x > 0.5f) { //0.5f for error
				SlidePanel (-slideAmount);
			} else if (!_isOpen && this.transform.position.x < _width) {
				SlidePanel (slideAmount);
			}
		}
	}

	public void TogglePanel(bool toggle) {
		_isOpen = toggle;
	}

	/// <summary>
	/// Slides the specified panel right or left (if negative).
	/// </summary>
	/// <param name="amount">Amount in pixels.</param>
	private void SlidePanel(float amount) {
		this.transform.position += new Vector3(amount, 0, 0);
	}
}
