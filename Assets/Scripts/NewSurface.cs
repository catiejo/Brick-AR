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
	private bool _isConvexHullSurface;
	private int[] _triangles;
	private Vector2[] _uv;
	private Vector3[] _vertices;
	//Helper class for MIConvexHull
	public class MIVertex : IVertex
	{
		public int Index;

		public double[] Position { get; set; }

		public Vector3 ToVector3()
		{
			var position = Position;
			if (position == null || position.Length < 3)
				return Vector3.zero;
			return new Vector3((float)position[0], (float)position[1], (float)position[2]);
		}
	}

	void Start() {
		SelectSurface ();
	}

	public void Create(Plane plane, Vector3 firstCorner, Vector3 oppositeCorner, Vector3 center) {
		_isConvexHullSurface = false;
		SetUpSurface (plane, center);
		//Set up mesh
		_vertices = FindCorners (firstCorner, oppositeCorner);
		_triangles = FindTriangles ();
		_uv = FindUV ();
		CreateMesh ();
	}

	public void Create(Plane plane, List<Vector3> worldVertices, Vector3 center) {
		_isConvexHullSurface = true;
		SetUpSurface (plane, center);
		_vertices = FindLocalVertices (worldVertices);
		_triangles = FindTriangles ();
		_uv = FindUV ();
		CreateMesh ();
	}

	private void SetUpSurface(Plane plane, Vector3 center) {
		//Member variables
		_center = center;
		_plane = plane;
		//Plane coordinate system
		var xaxis = Quaternion.LookRotation(-_plane.normal) * Vector3.right; //Horizontal vector transformed to plane's rotation
		var yaxis = Vector3.Cross(xaxis, _plane.normal);
		//Position + Rotation
		transform.position = _center;
		transform.rotation = Quaternion.LookRotation (-_plane.normal, yaxis);
	}
		

	public void SetMaterial(Material material) {
		GetComponent<MeshRenderer> ().material = material;
	}

	public void Undo() {
		DestroyImmediate (gameObject);
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
		//Put vectors in local space
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

	private Vector3[] FindLocalVertices(List<Vector3> worldVertices) {
		var localVertices = new List<Vector3>();
		foreach (var worldVertex in worldVertices) {
			localVertices.Add(transform.InverseTransformPoint(worldVertex));
		}
		return localVertices.ToArray ();
	}

	private int[] FindTriangles() {
		var triangles = new List<int> ();
		if (_isConvexHullSurface) {
			var miVertices = new List<MIVertex> ();
			//Convert vertices to MIVertices
			for (int i = 0; i < _vertices.Length; ++i) {
				var miVertex = new MIVertex ();
				var vertex = _vertices [i];
				miVertex.Index = i;
				miVertex.Position = new double[3]{ vertex.x, vertex.y, vertex.z };
				miVertices.Add (miVertex);
			}
			//Generate convex hull + extract triangles
			var hull = ConvexHull.Create (miVertices);
			foreach (var face in hull.Faces) {
				foreach (var vertex in face.Vertices) {
					triangles.Add (vertex.Index);
				}
			}
		} else {
			//Lower left triangle.
			triangles.Add (0);
			triangles.Add (1);
			triangles.Add (2);
			//Upper right triangle.   
			triangles.Add (3);
			triangles.Add (4);
			triangles.Add (5);
		}
		return triangles.ToArray();
	}

	private Vector2[] FindUV() {
		var uv = new List<Vector2> ();
		foreach (var vertex in _vertices) {
			uv.Add (vertex * 3.0f); //Add method knows to discard z coordinate
		} 
		return uv.ToArray();
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
