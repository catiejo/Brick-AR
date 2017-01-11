using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Tango;

public class OcclusionController : MonoBehaviour {
	public Toggle depthPanelToggle;
	public GameObject dynamicMesh;
	public TangoApplication tango;
	public BrickMenuController brickMenu;
	private static bool _isOccluding;

	void Start () {
		ToggleOcclusion (true);
	}

	/// <summary>
	/// Toggles the occlusion feature (depth panel, dynamic meshing/3D reconstruction).
	/// </summary>
	/// <param name="currentState">Turn on if <c>true</c>, off if <c>false</c>.</param>
	public void ToggleOcclusion(bool currentState) {
		_isOccluding = currentState;
		depthPanelToggle.isOn = false;
		depthPanelToggle.interactable = currentState;
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
		SwitchMaterials ();
	}

	public static bool IsOccluding() {
		return _isOccluding;
	}

	//HACK Also in Surface
	private void SwitchMaterials() {
		var gameObjects = GameObject.FindGameObjectsWithTag ("Surface");
		foreach (var go in gameObjects) {
			var surface = go.GetComponent<Surface> ();
			var material = brickMenu.GetMaterialByColor (surface.GetBrickColor ());
			surface.SetMaterial (material);
		}

	}
}
