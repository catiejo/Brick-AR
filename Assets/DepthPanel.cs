using UnityEngine;
using System.Collections;

public class DepthPanel : MonoBehaviour {
	private bool _isOpen = false;

	void Start () {
		SlideDepthPanel (-transform.localScale.x); //start with it closed
	}
	
	void Update () {
		if (_isOpen && transform.localScale.x < 1.0f) {
			SlideDepthPanel (0.2f);
		} else if (!_isOpen && transform.localScale.x > 0) {
			SlideDepthPanel (-0.2f);
		}
	}

	public void ToggleDepthPanel(bool isOpen) {
		if (isOpen) {
			ExpandDepthPanel ();
		} else {
			CollapseDepthPanel ();
		}
	}

	private void SlideDepthPanel(float width) {
		transform.localScale = new Vector3(transform.localScale.x + width, 1, 1);
	}

	private void ExpandDepthPanel() {
		_isOpen = true;
	}
	private void CollapseDepthPanel() {
		_isOpen = false;
	}
}
