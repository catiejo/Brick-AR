﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic; //Lists

public abstract class SurfaceMesh : MonoBehaviour {
	public Mesh mesh;

	protected Vector3 _center;
	protected Plane _plane;
	protected int[] _triangles;
	protected Vector2[] _uv;
	protected Vector3[] _vertices;

	public Mesh CreateMesh() {
		// Setup
		_vertices = FindVertices ();
		_triangles = FindTriangles ();
		_uv = FindUV ();
		// Create
		Mesh mesh = new Mesh();
		mesh.Clear();
		mesh.MarkDynamic();
		mesh.vertices = _vertices;
		mesh.uv = _uv;
		mesh.triangles = _triangles;
		return mesh;
	}

	public bool SetupLocalCoords(Plane plane, Vector3 center) { //FIXME: this is duplicated code from surface
		_plane = plane;
		_center = center;
		//Plane coordinate system
		var xaxis = Quaternion.LookRotation(-_plane.normal) * Vector3.right; //Horizontal vector transformed to plane's rotation
		var yaxis = Vector3.Cross(xaxis, _plane.normal);
		//Position + Rotation
		transform.position = _center;
		transform.rotation = Quaternion.LookRotation (-_plane.normal, yaxis);
		return true;
	}

	public bool HasVertices() {
		return _vertices.Length != 0;
	}

	protected abstract int[] FindTriangles ();
	protected abstract Vector3[] FindVertices ();

	protected Vector2[] FindUV() {
		var uv = new List<Vector2> ();
		foreach (var vertex in _vertices) {
			uv.Add (vertex * 3.0f); //Add method knows to discard z coordinate
		} 
		return uv.ToArray();
	}
}
