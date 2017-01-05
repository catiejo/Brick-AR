﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic; //Lists

public abstract class SurfaceMesh : MonoBehaviour {
	public Vector3 _center;
	public Plane _plane;
	public int[] _triangles;
	public Vector2[] _uv;
	public Vector3[] _vertices;

	public abstract int[] FindTriangles ();
	public abstract Vector3[] FindVertices ();

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
//		GetComponent<MeshFilter>().mesh = mesh; //should this also be sharedMesh?
//		GetComponent<MeshCollider>().sharedMesh = mesh;
		return mesh;
	}

	public Vector2[] FindUV() {
		var uv = new List<Vector2> ();
		foreach (var vertex in _vertices) {
			uv.Add (vertex * 3.0f); //Add method knows to discard z coordinate
		} 
		return uv.ToArray();
	}

	public bool SetupLocalCoords(Plane plane, Vector3 center) {
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

}