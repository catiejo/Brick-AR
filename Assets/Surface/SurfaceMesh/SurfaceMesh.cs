using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class SurfaceMesh : ScriptableObject {
	public Mesh mesh;

	protected Surface _associatedSurface;
	protected Vector3 _center;
	protected Plane _plane;
	protected int[] _triangles;
	protected Vector2[] _uv;
	protected Vector3[] _vertices;

	/// <summary>
	/// Create the appropriate SurfaceMesh initialized with init params.
	/// </summary>
	/// <param name="detectionMode">Edge detection mode. Either "TAP" or "DRAG".</param>
	/// <param name="init">
	/// If DRAG, params must be ordered: (Surface) surface, (Vector3) firstCorner, (Vector3) oppositeCorner. 
	/// If TAP, order is: (Surface) surface, (List<Vector3>) worldVertices.
	/// </param>
	public static SurfaceMesh Create (string detectionMode, params object[] init) {
		//credit: http://answers.unity3d.com/answers/600984/view.html
		SurfaceMesh surfaceMesh;
		if (detectionMode == "DRAG") {
			surfaceMesh = ScriptableObject.CreateInstance<DragSurfaceMesh> ();
		} else {
			surfaceMesh = ScriptableObject.CreateInstance<TapSurfaceMesh> ();
		}
		var success = surfaceMesh.Initialize (init);
		return success ? surfaceMesh : null;
	}

	/// <summary>
	/// Creates the from the vertices, uv, and triangles generated in child classes.
	/// </summary>
	/// <returns>The mesh.</returns>
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

	/// <summary>
	/// Determines if the mesh is empty by whether or not it has vertices.
	/// </summary>
	/// <returns><c>true</c> if this instance has vertices; otherwise, <c>false</c>.</returns>
	public bool IsEmpty() {
		return _vertices.Length == 0;
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
			uv.Add (vertex * 3.0f); //Add method knows to discard z coordinate
		} 
		return uv.ToArray();
	}

	protected abstract bool Initialize (params object[] init);
}
