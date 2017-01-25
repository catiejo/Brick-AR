using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SliderBehavior : MonoBehaviour {
	public Color enabledText;
	public Color disabledText;
	public Color enabledFill;
	public Color disabledFill;
	public Image fill;

	private Slider _slider;

	void Start() {
		_slider = GetComponent<Slider> ();
	}

	public void Toggle(bool enabled) {
		fill.color = enabled ? enabledFill : disabledFill;
		_slider.interactable = enabled;
		var text = _slider.GetComponentsInChildren<Text> ();
		foreach (var t in text) {
			t.color = enabled ? enabledText : disabledText;
		}
	}
}
