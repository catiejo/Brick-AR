﻿using UnityEngine;
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

	public void Create(Vector3[] vertices, Plane plane, Vector3 planeCenter, Material material) {
		_plane = plane;
		_material = material;
		_planeCenter = planeCenter;
		_vertices = vertices;
		_faces = FindFaces ();
		_uv = FindUV ();
		CreateMesh ();
	}

	private void CreateMesh() {
		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		mesh.vertices = _vertices;
		mesh.uv = _uv;
		mesh.triangles = _faces;
		CreatePlane ();
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
		transform.localScale = new Vector3(_dimensions.x, _dimensions.y, 1.0f);
		transform.position = _center;
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
		FindDimensions (); //sets _dimensions and _center...might not be needed if using hull
		var uv = new List<Vector2>();
		var xaxis = Quaternion.LookRotation (_plane.normal) * Vector3.right;
		var yaxis = Vector3.Cross(xaxis, _plane.normal);
		foreach (var vertex in _vertices) {
			var delta = vertex - _center;
			var xcoord = Vector3.Project (delta, xaxis);
			var ycoord = Vector3.Project (delta, yaxis);
			uv.Add(new Vector2(xcoord.x, ycoord.y));
		}
		return uv.ToArray ();
	}

}
