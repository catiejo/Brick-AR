using UnityEngine;
using System.Collections;
using Tango;

public class OcclusionController : MonoBehaviour {
	public DepthPanel depthPanel;
	public GameObject dynamicMesh;
	public TangoApplication tango;

	// Use this for initialization
	void Start () {
		ToggleOcclusion (false);
	}

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
