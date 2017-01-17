using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Tango;

public class OcclusionController : MonoBehaviour {
	public GameObject dynamicMesh;
	public Slider alphaSlider;
	public TangoApplication tango;
	public BrickMenuController brickMenu;
	public Surface surface;

	private static bool _isOccluding;

	void Start () {
		ToggleOcclusion (true);
	}

	/// <summary>
	/// Allows other classes to check if app is currently in occlusion mode.
	/// </summary>
	/// <returns><c>true</c> if is occluding; otherwise, <c>false</c>.</returns>
	public static bool IsOccluding() {
		return _isOccluding;
	}

	/// <summary>
	/// Toggles the occlusion feature (depth panel, dynamic meshing/3D reconstruction).
	/// </summary>
	/// <param name="currentState">Turn on if <c>true</c>, off if <c>false</c>.</param>
	public void ToggleOcclusion(bool currentState) {
		_isOccluding = currentState;
		tango.m_enable3DReconstruction = currentState;
		if (currentState) {
			tango.m_3drUpdateMethod = Tango3DReconstruction.UpdateMethod.PROJECTIVE;
			tango.m_3drResolutionMeters = 0.05f;
			tango.m_3drGenerateNormal = true;
			tango.m_3drSpaceClearing = true;
			tango.m_3drGenerateColor = false;
			tango.m_3drGenerateTexCoord = false;
			tango.m_3drUseAreaDescriptionPose = false;
			tango.m_3drMinNumVertices = 20;
		}
		dynamicMesh.SetActive (currentState);
		alphaSlider.interactable = currentState;
		surface.SetMaterial (brickMenu.GetMaterialByColor("Default"));
		SwitchMaterials ();
	}

	/// <summary>
	/// Switches the materials based on occlusion setting. Materials look the same but have different shaders.
	/// </summary>
	private void SwitchMaterials() {
		var gameObjects = GameObject.FindGameObjectsWithTag ("Surface");
		foreach (var go in gameObjects) {
			var surface = go.GetComponent<Surface> ();
			var material = brickMenu.GetMaterialByColor (surface.GetBrickColor ());
			surface.SetMaterial (material);
		}

	}
}
