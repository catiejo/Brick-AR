using UnityEngine;
using System.Collections;

public class SelectableBehavior : MonoBehaviour {
	public static Color glowColor = Color.white;

	private static SelectableBehavior _instance;
	private static float _maxGlowAmount = 0.25f;
	private static Surface _selectedSurface;

	void Awake() {
		_instance = this; //set our static reference to our newly initialized instance
	}

	/// <summary>
	/// Deselects the current selectedSurface.
	/// </summary>
	public static void DeselectSurface() {
		Material material = _selectedSurface.GetComponent<Renderer>().material;
		material.DisableKeyword("_EMISSION");
		material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
		material.SetColor("_EmissionColor", Color.black);
		_selectedSurface = null;
	}

	/// <summary>
	/// Wrapper to prevent other classes from writing to the selected surface.
	/// </summary>
	/// <returns>The selected surface.</returns>
	public static Surface GetSelectedSurface() {
		return _selectedSurface;
	}

	/// <summary>
	/// Selects a new selectedSurface.
	/// </summary>
	/// <param name="selected">The surface to be selected.</param>
	public static void SelectSurface(Surface surface) {
		if (_selectedSurface && _selectedSurface != surface) {
			DeselectSurface ();
		}
		_selectedSurface = surface;
		Glow ();
	}

	/// <summary>
	/// Make the selected surface glow.
	/// </summary>
	private static void Glow() {
		//credit: flaminghairball's answer to https://forum.unity3d.com/threads/c-coroutines-in-static-functions.134546/
		_instance.StartCoroutine("GlowRoutine");
	}

	/// <summary>
	/// Makes the selectedSurface glow.
	/// </summary>
	private static IEnumerator GlowRoutine()
	{
		// Setup
		Material material = _selectedSurface.GetComponent<Renderer>().material;
		material.EnableKeyword("_EMISSION");
		material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
		// Increase intensity (fade in)
		var glowAmount = 0.0f;
		while (glowAmount < _maxGlowAmount)
		{
			material.SetColor("_EmissionColor", glowColor * glowAmount);
			glowAmount += 0.01f;
			yield return new WaitForSeconds(0.01f);
		}
	}
}
