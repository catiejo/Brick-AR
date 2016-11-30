using UnityEngine;
using UnityEngine.UI;
using MIConvexHull;
using System.Collections;
using System.Collections.Generic; //Lists
using Tango;

public class Surface : MonoBehaviour {
	private Text debug;
	private Plane _plane;
	private Vector3 _planeCenter;
	private Material _material;
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

//	void Start() {
//		debug = GameObject.FindWithTag ("Debug").GetComponent<Text>();
//		debug.text = "surface created";
//	}

	public void Create(List<Vector3> worldVertices, Plane plane, Vector3 planeCenter, Material material) {
		//Member variables
		_plane = plane;
		_planeCenter = planeCenter;
		_material = material;
		//Plane coordinate system
		var xaxis = Quaternion.LookRotation(_plane.normal) * Vector3.right; //Horizontal vector transformed to plane's rotation
		var yaxis = Vector3.Cross(xaxis, _plane.normal);
		//Position plane
		transform.position = _planeCenter;
		transform.rotation = Quaternion.LookRotation (_plane.normal, yaxis);
		//Set up mesh
		_vertices = FindLocalVertices(worldVertices);
		_triangles = FindTriangles ();
		_uv = FindUV ();
		CreateMesh ();
	}

	public void Recreate(List<Vector3> worldVertices) {
		_vertices = FindLocalVertices(worldVertices);
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
		GetComponent<MeshFilter>().mesh = mesh;
		GetComponent<MeshRenderer> ().material = _material;
	}

	private Vector3[] FindLocalVertices(List<Vector3> worldVertices) {
		var localVertices = new List<Vector3>();
		foreach (var worldVertex in worldVertices) {
			localVertices.Add(transform.InverseTransformPoint(worldVertex));
		}
		return localVertices.ToArray ();
	}

	private int[] FindTriangles() {
		var triangles = new List<int>();
		var miVertices = new List<MIVertex>();
		//Convert vertices to MIVertices
		for (int i = 0; i < _vertices.Length; ++i)
		{
			var miVertex = new MIVertex();
			var vertex = _vertices[i];
			miVertex.Index = i;
			miVertex.Position = new double[3]{vertex.x, vertex.y, vertex.z};
			miVertices.Add(miVertex);
		}
		//Generate convex hull + extract faces
		var hull = ConvexHull.Create(miVertices);
		foreach (var face in hull.Faces)
		{
			foreach (var vertex in face.Vertices)
			{
				triangles.Add(vertex.Index);
			}
		}
		return triangles.ToArray();
	}

	private Vector2[] FindUV() {
		var uv = new List<Vector2>();
		foreach (var vertex in _vertices) {
			uv.Add(vertex); //Add method knows to discard z coordinate
		}
		return uv.ToArray ();
	}

}
