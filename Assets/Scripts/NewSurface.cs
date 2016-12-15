using UnityEngine;
using UnityEngine.UI;
using MIConvexHull;
using System.Collections;
using System.Collections.Generic; //Lists
using Tango;

public class NewSurface : MonoBehaviour {
	public static NewSurface selectedSurface;
	public Color glowColor = Color.white;
	private Vector3 _center;
	private Plane _plane;
	private float _glowAmount;
	private int[] _triangles;
	private Vector2[] _uv;
	private Vector3[] _vertices;

	void Start() {
		SelectSurface ();
	}

	public void Create(Plane plane, Vector3 firstCorner, Vector3 oppositeCorner, Vector3 center) {
		var corners = FindCorners (firstCorner, oppositeCorner);
		CoreCreate (plane, corners, center);
	}

	public void Create(Plane plane, List<Vector3> vertices, Vector3 center) {
		var corners = FindCorners(vertices, center);
		CoreCreate (plane, corners, center);
	}

	public void SetMaterial(Material material) {
		GetComponent<MeshRenderer> ().material = material;
	}

	public void Undo() {
		DestroyImmediate (gameObject);
	}

	private void CoreCreate(Plane plane, Vector3[] corners, Vector3 center) {
		//Member variables
		_center = center;
		_plane = plane;
		//Plane coordinate system
		var xaxis = Quaternion.LookRotation(-_plane.normal) * Vector3.right; //Horizontal vector transformed to plane's rotation
		var yaxis = Vector3.Cross(xaxis, _plane.normal);
		//Position + Rotation
		transform.position = _center;
		transform.rotation = Quaternion.LookRotation (-_plane.normal, yaxis);
		//Set up mesh
		_vertices = corners;
		_triangles = FindTriangles ();
		_uv = FindUV ();
		CreateMesh ();

	}

	private void CreateMesh() {
		Mesh mesh = new Mesh();
		mesh.Clear();
		mesh.MarkDynamic();
		mesh.vertices = _vertices;
		mesh.uv = _uv;
		mesh.triangles = _triangles;
		GetComponent<MeshFilter>().mesh = mesh; //should this also be sharedMesh?
		GetComponent<MeshCollider>().sharedMesh = mesh;
	}

	private Vector3[] FindCorners(Vector3 firstCorner, Vector3 oppositeCorner) {
		//Put corners in local space
		var corner1 = transform.InverseTransformPoint(firstCorner);
		corner1.z = 0;
		var corner2 = transform.InverseTransformPoint(oppositeCorner);
		corner2.z = 0;
		//Find min/max coordinate values
		var min = new Vector2(Mathf.Min(corner1.x, corner2.x), Mathf.Min(corner1.y, corner2.y));
		var max = new Vector2(Mathf.Max(corner1.x, corner2.x), Mathf.Max(corner1.y, corner2.y));

		var corners = new Vector3[4];

		corners[0] = new Vector3(min.x, min.y, 0); //bottom left
		corners[1] = new Vector3(max.x, min.y, 0); //bottom right
		corners[2] = new Vector3(min.x, max.y, 0); //top left
		corners[3] = new Vector3(max.x, max.y, 0); //top right

		return corners;
	}

	private Vector3[] FindCorners(List<Vector3> vertices, Vector3 center) {
		//FIXME dear lord this whole function
		var bottomLeft = center; //lowest y coordinate
		var bottomRight = center; //highest x coordinate
		var topLeft = center; //lowest x coordinate
		var topRight = center; //highest y coordinate

		var corners = new Vector3[4];

		foreach (Vector3 v in vertices) {
			var local = transform.InverseTransformPoint (v);
			local.z = 0;
			//Check if better bottomLeft
			if (local.y <= bottomLeft.y && local.x < bottomRight.x) {
				Debug.LogWarning("bottomLeft is now " + local.ToString());
				bottomLeft = local;
			} 
			//Check if better bottomRight
			if (local.x >= bottomRight.x && local.y < topRight.y) {
				Debug.LogWarning("bottomRight is now " + local.ToString());
				bottomRight = local;
			}
			//Check if better topLeft
			if (local.x <= topLeft.x && local.y > bottomLeft.y) {
				Debug.LogWarning("topLeft is now " + local.ToString());
				topLeft = local;
			}
			//Check if better topRight
			if (local.y >= topRight.y && local.x > topLeft.x) {
				Debug.LogWarning("topRight is now " + local.ToString());
				topRight = local;
			}
		}

		corners[0] = bottomLeft;
		corners[1] = bottomRight;
		corners[2] = topLeft;
		corners[3] = topRight;

		return corners;
	}

	private int[] FindTriangles() {
		var triangles = new int[6];

		//Lower left triangle.
		triangles[0] = 0;
		triangles[1] = 2;
		triangles[2] = 1;
		//Upper right triangle.   
		triangles[3] = 2;
		triangles[4] = 3;
		triangles[5] = 1;

		return triangles;
	}

	private Vector2[] FindUV() {
		var uv = new Vector2[4];

		uv[0] = _vertices[0] * 3.0f;
		uv[1] = _vertices[1] * 3.0f;
		uv[2] = _vertices[2] * 3.0f;
		uv[3] = _vertices[3] * 3.0f;

		return uv;
	}
		
	public void SelectSurface() {
		if (selectedSurface && selectedSurface != this) {
			selectedSurface.DeselectSurface ();
		}
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
		selectedSurface = this;
		// Increase intensity (fade in)
		while (_glowAmount < 0.25)
		{
			material.SetColor("_EmissionColor", glowColor * _glowAmount);
			_glowAmount += 0.01f;
			yield return new WaitForSeconds(0.01f);
		}
	}
}
