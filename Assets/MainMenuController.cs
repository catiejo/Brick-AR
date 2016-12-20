using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {
	public Image menuDrawer;
	private Color _color = new Color(0, 0, 0, 0);
	private bool _isOpen = false;

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

	public void ToggleMenu() {
		_isOpen = !_isOpen;
	}

	private void SlideMenu(float amount) {
		menuDrawer.transform.position += new Vector3(amount, 0, 0);
	}


}
