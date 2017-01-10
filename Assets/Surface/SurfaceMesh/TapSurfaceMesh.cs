using KDTree;
using MIConvexHull;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TapSurfaceMesh : SurfaceMesh {
	private int neighborCountThreshold = 4;
	private float neighborDistanceThreshold = 0.000175f;
	private List<Vector3> _worldVertices;
	//Helper class for kdTree
	public class Point
	{
		public double[] doublePosition;
		public Vector3 vectorPosition;
		public bool visited = false;
		public Point(Vector3 point) {
			doublePosition = new double[3]{point.x, point.y, point.z};
			vectorPosition = point;
		}
	}
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

	public TapSurfaceMesh(Surface surface, List<Vector3> worldVertices) {
		_worldVertices = worldVertices;
		_associatedSurface = surface;
		Mesh tapSurfaceMesh = CreateMesh ();
		mesh = tapSurfaceMesh;
	}

	protected override int[] FindTriangles ()
	{
		var triangles = new List<int> ();
		//Convert vertices to MIVertices
		var miVertices = new List<MIVertex> ();
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
		return triangles.ToArray ();
	}

	protected override Vector3[] FindVertices ()
	{
		var vertices = new List<Vector3> ();
		var localVertices = FindLocalVertices (_worldVertices);

		//Step One: Create a kd-tree of the points on the plane
		var kdTree = new KDTree<Point>(3);
		foreach (var vertex in localVertices) {
			var point = new Point (vertex);
			kdTree.AddPoint(point.doublePosition, point);
		}
		//Step Two: Breadth-first-style search for points on surface.
		var pointsToCheck = new Queue<Point>();
		pointsToCheck.Enqueue (new Point(_center));
		while (pointsToCheck.Count != 0) {
			var currentPoint = pointsToCheck.Dequeue ();
			var neighbors = kdTree.NearestNeighbors(currentPoint.doublePosition, neighborCountThreshold, neighborDistanceThreshold);
			int neighborCount = 0;
			while (neighbors.MoveNext()) {
				if (!neighbors.Current.visited) {
					pointsToCheck.Enqueue (neighbors.Current);
					neighbors.Current.visited = true;
				}
				neighborCount++;
			}
			if (neighborCount >= neighborCountThreshold) {
				vertices.Add (currentPoint.vectorPosition);
			}
		}

		return vertices.ToArray();
	}

	/// <summary>
	/// Initialize the DragSurface. Parameters must be in order: surface, worldVertices.
	/// </summary>
	protected override bool Initialize(params object[] init) {
		if (init.Length != 2) {
			Debug.LogError ("Incorrect number of arguments called. Must be in order: (Surface) surface, (List<Vector3>) worldVertices");
			return false;
		}
		_associatedSurface = (Surface) init [0];
		_worldVertices = (List<Vector3>) init [1];
		Mesh tapSurfaceMesh = CreateMesh ();
		mesh = tapSurfaceMesh;
		return true;
	}

	/// <summary>
	/// Converts world vertices to local space. Only works after <c>Initialize</c> is called.
	/// </summary>
	/// <returns>Vertices converted to local space.</returns>
	/// <param name="worldVertices">World vertices.</param>
	private List<Vector3> FindLocalVertices(List<Vector3> worldVertices) {
		var localVertices = new List<Vector3>();
		foreach (var worldVertex in worldVertices) {
			localVertices.Add(_associatedSurface.transform.InverseTransformPoint(worldVertex));
		}
		return localVertices;
	}

}
