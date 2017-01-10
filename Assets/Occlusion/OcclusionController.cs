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

//	//WHAT THE ACTUAL FUCK
//	void Start () {
//		ToggleOcclusion (false);
//	}

	/// <summary>
	/// Toggles the occlusion feature (depth panel, dynamic meshing/3D reconstruction).
	/// </summary>
	/// <param name="currentState">Turn on if <c>true</c>, off if <c>false</c>.</param>
	public void ToggleOcclusion(bool currentState) {
		ScreenLog.Write ("occlusion toggled");
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
//		SwitchMaterials ();
	}

	public static bool IsOccluding() {
		return _isOccluding;
	}

	//HACK Also in Surface
	private void SwitchMaterials() {
		var gameObjects = GameObject.FindGameObjectsWithTag ("Surface");
		var count = 0;
		foreach (var go in gameObjects) {
			var surface = go.GetComponent<Surface> ();
			brickMenu.SelectOption (surface.GetBrickColor ());
			surface.SetMaterial (brickMenu.GetCurrentMaterial());
			count++;
		}

	}
}
