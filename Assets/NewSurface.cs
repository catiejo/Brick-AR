using UnityEngine;
using UnityEngine.UI;
using MIConvexHull;
using System.Collections;
using System.Collections.Generic; //Lists
using Tango;

public class NewSurface : MonoBehaviour {
	private Plane _plane;
	private Vector3 _center;
	private Material _material;
	private int[] _triangles;
	private Vector2[] _uv;
	private Vector3[] _vertices;
	public NewMenuController menuTemplate;
	public Material defaultMaterial;
	private NewMenuController _surfaceOptions;
	public static NewSurface selectedSurface;

//	void Update() {
//		if (_surfaceOptions.HasColor ()) {
//			_material = _surfaceOptions.GetCurrentMaterial ();
//		}
//	}

	void Start() {
		selectedSurface = this;
	}

	public void Create(Plane plane, Vector3 topLeft, Vector3 bottomRight, Vector3 center) {
		//Member variables
		_center = center;
		_plane = plane;
		_material = defaultMaterial;
		//Plane coordinate system
		var xaxis = Quaternion.LookRotation(-_plane.normal) * Vector3.right; //Horizontal vector transformed to plane's rotation
		var yaxis = Vector3.Cross(xaxis, _plane.normal);
		//Position + Rotation
		transform.position = _center;
		transform.rotation = Quaternion.LookRotation (-_plane.normal, yaxis);
		//Set up mesh
		_vertices = FindCorners (topLeft, bottomRight);
		_triangles = FindTriangles ();
		_uv = FindUV ();
		CreateMesh ();
	}

	public void SetMaterial(Material material) {
		GetComponent<MeshRenderer> ().material = material;
	}

	private void CreateMesh() {
		Mesh mesh = new Mesh();
		mesh.Clear();
		mesh.MarkDynamic();
		mesh.vertices = _vertices;
		mesh.uv = _uv;
		mesh.triangles = _triangles;
		GetComponent<MeshFilter>().mesh = mesh;
	}

	private Vector3[] FindCorners(Vector3 topLeft, Vector3 bottomRight) {
		var topLeftCorner = transform.InverseTransformPoint(topLeft);
		topLeftCorner.z = 0;
		var bottomRightCorner = transform.InverseTransformPoint(bottomRight);
		bottomRightCorner.z = 0;

		var corners = new Vector3[4];

		corners[0] = new Vector3(topLeftCorner.x, bottomRightCorner.y, 0); //bottom left
		corners[1] = bottomRightCorner; //bottom right
		corners[2] = topLeftCorner; //top left
		corners[3] = new Vector3(bottomRightCorner.x, topLeftCorner.y, 0); //top right

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

		uv[0] = _vertices[0] * 2.0f;
		uv[1] = _vertices[1] * 2.0f;
		uv[2] = _vertices[2] * 2.0f;
		uv[3] = _vertices[3] * 2.0f;

		return uv;
	}

}
