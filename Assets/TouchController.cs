using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Tango;
using KDTree;

public class TouchController : MonoBehaviour {
	public MenuController brickMenu;
	public Text debug; //For testing purposes
	public int neighborCountThreshold = 25;
	public float neighborDistanceThreshold = .25f;
	public float planeDistanceThreshold = 0.02f;
	public Surface surfaceTemplate;
	public TangoPointCloud tangoPointCloud;
	//Helper class for kdTree
	public class Point : MonoBehaviour
	{
		public double[] doublePosition;
		public Vector3 vectorPosition;
		public Point(Vector3 point) {
			doublePosition = new double[3]{point.x, point.y, point.z};
			vectorPosition = point;
		}
	}

	void Update () {
		if (Input.touchCount > 0)
		{	
			Touch touch = Input.GetTouch (0);
			if (touch.phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject (touch.fingerId)) {
				if (!CreateSurface (touch.position)) {
					debug.text = "Unable to find a surface. Please try again.";
				}
			}
		}
//		if (Input.GetMouseButtonDown (0)) {
//			if (!CreateSurface (Input.mousePosition)) {
//				depthText.text = "Unable to find a surface. Please try again.";
//			}
//		}
	}

	private bool CreateSurface(Vector2 touch) {
		Vector3 planeCenter;
		Plane plane;
		if (tangoPointCloud.FindPlane (Camera.main, touch, out planeCenter, out plane)) {
			var surfaceVertices = FindSurfaceVertices (plane, planeCenter);
//			if (surfaceVertices.Count != 0) {
//				Surface surface = Instantiate (surfaceTemplate) as Surface;
//				surface.Create (surfaceVertices, plane, planeCenter, brickMenu.GetCurrentMaterial ());
				return true;
//			}
		}
		return false;
	}

	private List<Vector3> FindSurfaceVertices(Plane plane, Vector3 planeCenter) {
		var verticesOnSurface = new List<Vector3> ();

		//Step One: Narrow point cloud to points on the plane
		var verticesOnPlane = new List<Vector3>();
		for (int i = 0; i < tangoPointCloud.m_pointsCount; i++) {
			var p = tangoPointCloud.m_points [i];
			if (Mathf.Abs (plane.GetDistanceToPoint (p)) <= planeDistanceThreshold) {
				verticesOnPlane.Add (p);
			}
		}

		//Step Two: Create a kd-tree of the points on the plane
		var kdTree = new KDTree<Point>(3);
		foreach (var vertex in verticesOnPlane) {
			var point = new Point (vertex);
			kdTree.AddPoint(point.doublePosition, point);
		}

		//Step Three: Breadth-first-style search for points on surface.
		var pointsToCheck = new UniqueQueue<Point>();
		pointsToCheck.Enqueue (new Point(planeCenter));
		while (!pointsToCheck.isEmpty()) {
			var currentPoint = pointsToCheck.Dequeue ();
			var neighbors = kdTree.NearestNeighbors(currentPoint.doublePosition, neighborCountThreshold, neighborDistanceThreshold);
			int neighborCount = 0;
			while (neighbors.MoveNext()) {
				if (!pointsToCheck.Visited(neighbors.Current)) {
					pointsToCheck.Enqueue (neighbors.Current);	
				}
				neighborCount++;
			}
			if (neighborCount >= neighborCountThreshold) {
				verticesOnSurface.Add (currentPoint.vectorPosition);
			}
		}

		debug.text = "There are " + verticesOnPlane.Count + " vertices on the plane and " + verticesOnSurface.Count + " vertices on the surface.";
		return verticesOnSurface;
	}

}
