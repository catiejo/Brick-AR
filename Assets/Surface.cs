using UnityEngine;
using UnityEngine.UI;
using MIConvexHull;
using System.Collections;
using System.Collections.Generic; //Lists
using Tango;

public class Surface : MonoBehaviour {
	private Vector3 _center;
	private Vector3[] _vertices;
	private Vector2[] _uv;
	private Plane _plane;
	private int[] _faces;
	private Vector3 _planeCenter;
	private Vector2 _dimensions;
	private Text debug;
	private Material _material;

	// Helper class for MIConvexHull
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

	public void Create(List<Vector3> vertices, Plane plane, Vector3 planeCenter, Material material) {
		// find axes
		var xaxis = Quaternion.LookRotation (_plane.normal) * Vector3.right;
		var pos = Camera.main.transform.position + Camera.main.transform.rotation * Vector3.forward * 3f;
		Debug.DrawLine (pos, pos + xaxis, Color.cyan, 15f); 
		var yaxis = Vector3.Cross(xaxis, _plane.normal);
		Debug.DrawLine (pos, pos + yaxis, Color.magenta, 15f); 
		// set position and rotation (uses yaxis which is why we have to define it here)
		transform.position = _planeCenter;
		transform.rotation = Quaternion.LookRotation (_plane.normal, yaxis);

		_plane = plane;
		_material = material;
		_planeCenter = planeCenter;
		_vertices = Convertices(vertices);
		_faces = FindFaces ();
		_uv = FindUV ();
		CreateMesh ();
//		FindDimensions ();
//		CreatePlane ();
	}

	private Vector3[] Convertices(List<Vector3> worldVertices) {
		var localVertices = new List<Vector3>();
		foreach (var v in worldVertices) {
			localVertices.Add(transform.InverseTransformPoint(v));
		}
		return localVertices.ToArray ();
	}

	private void CreateMesh() {
		Mesh mesh = new Mesh();
		mesh.Clear();
		mesh.MarkDynamic();

		mesh.vertices = _vertices;
		mesh.uv = _uv;
		mesh.triangles = _faces;
		GetComponent<MeshFilter>().mesh = mesh;
		GetComponent<MeshRenderer> ().material = _material;
	}

	private void CreatePlane() {
		Vector3 forward;
		Vector3 up = _plane.normal;
		float angle = Vector3.Angle (up, Camera.main.transform.forward);
		if (angle < 175) {
			Vector3 right = Vector3.Cross(up, Camera.main.transform.forward).normalized;
			forward = Vector3.Cross(right, up).normalized;
		} else {
			// Normal is nearly parallel to camera look direction, the cross product would have too much
			// floating point error in it.
			forward = Vector3.Cross(up, Camera.main.transform.right);
		}
//		transform.localScale = new Vector3(_dimensions.x, _dimensions.y, 1.0f);
//		transform.localScale *= 0.03f;
		transform.position = _planeCenter;
		transform.rotation = Quaternion.LookRotation(forward, up);
		GetComponent<Renderer> ().material = _material;
	}

	private void FindDimensions() {
		Vector2 _min = Vector2.zero;
		Vector2 _max = Vector2.zero;
		foreach (Vector3 worldCoord in _vertices) {
			Vector3 localCoord = worldCoord - _planeCenter;
			_min.x = (localCoord.x < _min.x) ? localCoord.x : _min.x;
			_min.y = (localCoord.y < _min.y) ? localCoord.y : _min.y;
			_max.x = (localCoord.x > _max.x) ? localCoord.x : _max.x;
			_max.y = (localCoord.y > _max.y) ? localCoord.y : _max.y;
		}
		_dimensions.x = _max.x - _min.x;
		_dimensions.y = _max.y - _min.y;
		//Find Center
		float x = _max.x - (_dimensions.x/2.0f) + _planeCenter.x;
		float y = _max.y - (_dimensions.y/2.0f) + _planeCenter.y;
		float z = _planeCenter.z;
		_center = new Vector3(x, y, z);
	}

	private int[] FindFaces() {
		var faces = new List<int>();
		var miVertices = new List<MIVertex>();

		for (int i = 0; i < _vertices.Length; ++i)
		{
			var miVertex = new MIVertex();
			var vertex = _vertices[i];
			miVertex.Index = i;
			miVertex.Position = new double[3]{vertex.x, vertex.y, vertex.z};
			miVertices.Add(miVertex);
		}

		var hull = ConvexHull.Create(miVertices);
		foreach (var face in hull.Faces)
		{
			foreach (var vertex in face.Vertices)
			{
				faces.Add(vertex.Index);
			}
		}
		return faces.ToArray();
	}

	private Vector2[] FindUV() {
		var uv = new List<Vector2>();
		foreach (var vertex in _vertices) {
			uv.Add(vertex); //code knows to discard z coord
		}
		return uv.ToArray ();
	}

}
