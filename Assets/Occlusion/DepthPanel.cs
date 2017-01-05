using UnityEngine;
using System.Collections;

public class DepthPanel : MonoBehaviour {
	private bool _isOpen = false;

	void Start () {
//		SlideDepthPanel (0); //start with it closed
	}
	
	void Update () {
		if (_isOpen && transform.localScale.x < 1.0f) {
			SlideDepthPanel (0.2f);
		} else if (!_isOpen && transform.localScale.x > 0) {
			SlideDepthPanel (-0.2f);
		}
	}

	public void ToggleDepthPanel(bool isOpen) {
		_isOpen = isOpen;
	}

	public void SlideDepthPanel(float width) {
		transform.localScale = new Vector3(transform.localScale.x + width, 1, 1);
	}
}
