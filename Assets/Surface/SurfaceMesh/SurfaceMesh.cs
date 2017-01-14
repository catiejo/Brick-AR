using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class SurfaceMesh {
	public Mesh mesh;

	protected Surface _associatedSurface;
	protected Vector3 _center;
	protected Plane _plane;
	protected int[] _triangles;
	protected Vector2[] _uv;
	protected Vector3[] _vertices;

	/// <summary>
	/// Creates the from the vertices, uv, and triangles generated in child classes.
	/// </summary>
	/// <returns>The mesh.</returns>
	public Mesh CreateMesh() {
		// Setup
		_vertices = FindVertices ();
		if (_vertices.Length < 3) {
			ScreenLog.Write ("...insuficient vertices found. At least 3 required, found " + _vertices.Length);
			return null;
		}
		ScreenLog.Write ("..." + _vertices.Length + " vertices found");
		_triangles = FindTriangles ();
		ScreenLog.Write ("..." + _triangles.Length/3 + " triangles found");
		_uv = FindUV ();
		ScreenLog.Write ("..." + _uv.Length + " uv coords found");
		// Create
		Mesh mesh = new Mesh();
		mesh.Clear();
		mesh.MarkDynamic();
		mesh.vertices = _vertices;
		mesh.uv = _uv;
		mesh.triangles = _triangles;
		return mesh;
	}

	/// <summary>
	/// Finds the triangles for the mesh.
	/// </summary>
	/// <returns>Triangles of the mesh.</returns>
	protected abstract int[] FindTriangles ();

	/// <summary>
	/// Finds the vertices for the mesh.
	/// </summary>
	/// <returns>Vertices of the mesh.</returns>
	protected abstract Vector3[] FindVertices ();

	/// <summary>
	/// Finds the UV coordinates for the mesh.
	/// </summary>
	/// <returns>UV coordinates of the mesh.</returns>
	protected Vector2[] FindUV() {
		var uv = new List<Vector2> ();
		foreach (var vertex in _vertices) {
			uv.Add (vertex * 0.3f); //Add method knows to discard z coordinate
		} 
		return uv.ToArray();
	}
}
