using UnityEngine;
using System.Collections;

public class SelectableBehavior : MonoBehaviour {
	public Surface associatedSurface; // Surface residing in same game object
	public Color glowColor = Color.white;
	public static Surface selectedSurface;

	private float _glowAmount;

	void Start() {
		associatedSurface = gameObject.GetComponent<Surface>();
		SelectSurface (associatedSurface);
	}

	public void SelectSurface(Surface selected) {
		if (selectedSurface && selectedSurface != selected) {
			DeselectSurface ();
		}
		selectedSurface = selected;
		StartCoroutine (Glow ());
	}

	public static void DeselectSurface() {
		Material material = selectedSurface.GetComponent<Renderer>().material;
//		_glowAmount = 0; //to account for rounding error
		material.DisableKeyword("_EMISSION");
		material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
		material.SetColor("_EmissionColor", Color.black);
		selectedSurface = null;
	}

	private IEnumerator Glow()
	{
		// Setup
		Material material = selectedSurface.GetComponent<Renderer>().material;
		material.EnableKeyword("_EMISSION");
		material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
		// Increase intensity (fade in)
		_glowAmount = 0;
		while (_glowAmount < 0.25)
		{
			material.SetColor("_EmissionColor", glowColor * _glowAmount);
			_glowAmount += 0.01f;
			yield return new WaitForSeconds(0.01f);
		}
	}
}
