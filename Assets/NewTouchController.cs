using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Tango;
using KDTree;

public class NewTouchController : MonoBehaviour {
	public Text debug;
//	public MenuController brickMenu;
//	public Surface surfaceTemplate;
	public TangoPointCloud tangoPointCloud;
//	private int neighborCountThreshold = 4;
//	private float neighborDistanceThreshold = 0.000175f;
//	private float planeDistanceThreshold = 0.075f;
	private Corner topLeft;
	private Corner bottomRight;
	private bool isDragging;
	private GameObject line;

//	//Helper class for kdTree
//	public class Point : MonoBehaviour
//	{
//		public double[] doublePosition;
//		public Vector3 vectorPosition;
//		public bool visited = false;
//		public Point(Vector3 point) {
//			doublePosition = new double[3]{point.x, point.y, point.z};
//			vectorPosition = point;
//		}
//	}

	public class Corner : MonoBehaviour
	{
		public Vector2 screenPosition;
		public Vector3 worldPosition;
		public Corner (Vector2 sPos, Vector3 wPos) {
			screenPosition = sPos;
			worldPosition = wPos;
		}
	}

	void Update () {
		if (Input.touchCount > 0)
		{	
			Touch touch = Input.GetTouch (0);
//			Vector3 closestPoint = tangoPointCloud.m_points[tangoPointCloud.FindClosestPoint(Camera.main, touch.position, 100)];
			if (touch.phase == TouchPhase.Began) {
				debug.text = "Touch started!";
//				StartLine (point);
				topLeft = SetCorner (touch.position);
			} else if (touch.phase == TouchPhase.Moved) {
//				ExtendLine (point);
			} else if (touch.phase == TouchPhase.Ended) {
				bottomRight = SetCorner (touch.position);
//				Destroy (line);
				CreateSurface ();
			}
		}
	}

	private void StartLine(Vector3 start)
	{
		line = new GameObject();
		line.transform.position = start;
		line.AddComponent<LineRenderer>();
		LineRenderer lr = line.GetComponent<LineRenderer>();
		lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		lr.SetColors(Color.cyan, Color.cyan);
		lr.SetWidth(0.1f, 0.1f);
		lr.SetPosition(0, start);
		lr.SetPosition(1, start);
	}

	private void ExtendLine(Vector3 end) {
		LineRenderer lr = line.GetComponent<LineRenderer>();
		lr.SetPosition(1, end);
	}

	private Color GetCurrentColor() {
		return Color.cyan;
	}

	private Corner SetCorner(Vector2 touch) {
		Vector3 closestPoint = tangoPointCloud.m_points[tangoPointCloud.FindClosestPoint(Camera.main, touch.position, 100)];
		return new Corner(touch, closestPoint);
	}

	private bool CreateSurface() {
		Vector3 midpoint = Vector3.Lerp (topLeft.worldPosition, bottomRight.worldPosition, 0.5f);
		Vector3 midPlaneCenter;
		Plane midPlane;
		if (tangoPointCloud.FindPlane (Camera.main, midpoint, out midPlaneCenter, out midPlane)) {
			debug.text = "Found a midpoint!";
			return true;
		}
		debug.text = "Please try again.";
		return false;
	}

//	private List<Vector3> FindSurfaceVertices(Plane plane, Vector3 planeCenter) {
//		var verticesOnSurface = new List<Vector3> ();
//
//		//Step One: Narrow point cloud to points on the plane
//		var verticesOnPlane = new List<Vector3>();
//		for (int i = 0; i < tangoPointCloud.m_pointsCount; i++) {
//			var p = tangoPointCloud.m_points [i];
//			if (Mathf.Abs (plane.GetDistanceToPoint (p)) <= planeDistanceThreshold) {
//				verticesOnPlane.Add (p);
//			}
//		}
//
//		//Step Two: Create a kd-tree of the points on the plane
//		var kdTree = new KDTree<Point>(3);
//		foreach (var vertex in verticesOnPlane) {
//			var point = new Point (vertex);
//			kdTree.AddPoint(point.doublePosition, point);
//		}
//
//		//Step Three: Breadth-first-style search for points on surface.
//		var pointsToCheck = new Queue<Point>();
//		pointsToCheck.Enqueue (new Point(planeCenter));
//		while (pointsToCheck.Count != 0) {
//			var currentPoint = pointsToCheck.Dequeue ();
//			var neighbors = kdTree.NearestNeighbors(currentPoint.doublePosition, neighborCountThreshold, neighborDistanceThreshold);
//			int neighborCount = 0;
//			while (neighbors.MoveNext()) {
//				if (!neighbors.Current.visited) {
//					pointsToCheck.Enqueue (neighbors.Current);
//					neighbors.Current.visited = true;
//				}
//				neighborCount++;
//			}
//			if (neighborCount >= neighborCountThreshold) {
//				verticesOnSurface.Add (currentPoint.vectorPosition);
//			}
//		}
//
//		debug.text = "There are " + verticesOnPlane.Count + " vertices on the plane and " + verticesOnSurface.Count + " vertices on the surface.";
//		return verticesOnSurface;
//	}

}
