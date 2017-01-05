using UnityEngine;
using System.Collections;
using Tango;

public class OcclusionController : MonoBehaviour {
	public DepthPanel depthPanel;
	public GameObject dynamicMesh;
	public TangoApplication tango;

	void Start () {
		ToggleOcclusion (false);
	}

	/// <summary>
	/// Toggles the occlusion feature (depth panel, dynamic meshing/3D reconstruction).
	/// </summary>
	/// <param name="currentState">Turn on if <c>true</c>, off if <c>false</c>.</param>
	public void ToggleOcclusion(bool currentState) {
		depthPanel.ToggleDepthPanel (currentState);
		dynamicMesh.SetActive (currentState);
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

	}
}
