using UnityEngine;
using UnityEngine.UI;
using MIConvexHull;
using System.Collections;
using System.Collections.Generic; //Lists
using Tango;

public class Surface : MonoBehaviour {
	public static Surface selectedSurface;
	public Color glowColor = Color.white;
	private Vector3 _center;
	private Plane _plane;
	private float _glowAmount;
	private Mesh _mesh;

	void Start() {
		SelectSurface ();
	}

	public void Create(Plane plane, Vector3 center, Mesh mesh) {
		//Member variables
		_center = center;
		_plane = plane;
		//Plane coordinate system
		var xaxis = Quaternion.LookRotation(-_plane.normal) * Vector3.right; //Horizontal vector transformed to plane's rotation
		var yaxis = Vector3.Cross(xaxis, _plane.normal);
		//Position + Rotation
		transform.position = _center;
		transform.rotation = Quaternion.LookRotation (-_plane.normal, yaxis);
		//Mesh
		GetComponent<MeshFilter>().mesh = mesh; //should this also be sharedMesh?
		GetComponent<MeshCollider>().sharedMesh = mesh;
	}		

	public void SetMaterial(Material material) {
		GetComponent<MeshRenderer> ().material = material;
	}

	public void Undo() {
		DestroyImmediate (gameObject);
	}
		
	public void SelectSurface() {
		if (selectedSurface && selectedSurface != this) {
			selectedSurface.DeselectSurface ();
		}
		selectedSurface = this;
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
