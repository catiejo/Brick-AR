using UnityEngine;
using System.Collections;

public class SelectableBehavior : MonoBehaviour {
	public Color glowColor = Color.white;
	public static Surface selectedSurface;
	private float _glowAmount;

	public void SelectSurface() {
		if (selectedSurface && selectedSurface != gameObject.GetComponent<Surface> ()) {
			selectedSurface.DeselectSurface ();
		}
		selectedSurface = gameObject.GetComponent<Surface> ();
		StartCoroutine (Glow ());
	}

	public void DeselectSurface() {
		selectedSurface = null;
		Material material = gameObject.GetComponent<Renderer>().material;
		_glowAmount = 0; //to account for rounding error
		material.DisableKeyword("_EMISSION");
		material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
		material.SetColor("_EmissionColor", Color.black);
	}

	private IEnumerator Glow()
	{
		// Setup
		Material material = gameObject.GetComponent<Renderer>().material;
		material.EnableKeyword("_EMISSION");
		material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
		// Increase intensity (fade in)
		while (_glowAmount < 0.25)
		{
			material.SetColor("_EmissionColor", glowColor * _glowAmount);
			_glowAmount += 0.01f;
			yield return new WaitForSeconds(0.01f);
		}
	}
}
