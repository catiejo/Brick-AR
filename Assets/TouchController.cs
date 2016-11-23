using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Tango;

public class TouchController : MonoBehaviour {
	public MenuController brickMenu;
	public Text debug; //For testing purposes
	public int neighborThreshold = 10;
	public float planeDistanceThreshold = 0.02f;
	public Surface surfaceTemplate;
	public TangoPointCloud tangoPointCloud;
	//Helper class for PointOctree
	public class Point : MonoBehaviour
	{
		public Vector3 position;
		public Point(Vector3 pointPosition) {
			position = pointPosition;
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
		//Mouse tracking for testing in Unity player
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
			if (surfaceVertices.Count != 0) {
				Surface surface = Instantiate (surfaceTemplate) as Surface;
				surface.Create (surfaceVertices, plane, planeCenter, brickMenu.GetCurrentMaterial ());
				return true;
			}
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
		debug.text = "There are " + verticesOnPlane.Count + " vertices on the plane";
		//Step Two: Create an octree of the points on the plane
		var pointCloudOctree = new PointOctree<Point>(1024, planeCenter, 0.25f);
		foreach (var vertex in verticesOnPlane) {
			pointCloudOctree.Add(new Point (vertex), vertex);
		}
		//Step Three: Breadth first search for points on surface.
		Queue<Point> pointsToCheck = new Queue<Point>();
		pointsToCheck.Enqueue (new Point(planeCenter));
		while (pointsToCheck.Count > 0) {
			var currentPoint = pointsToCheck.Dequeue ();
			var neighbors = pointCloudOctree.GetNearby (new Ray(Camera.main.transform.position, currentPoint.position), 0.1f);
			debug.text = "There are " + neighbors.Length + " neighbors nearby";
			var neighborsOnPlane = 0;
			foreach (var neighbor in neighbors) {
				if (Mathf.Abs(plane.GetDistanceToPoint(neighbor.position)) <= planeDistanceThreshold) {
					pointsToCheck.Enqueue(neighbor);
					neighborsOnPlane++;
				}
				pointCloudOctree.Remove (neighbor); //Already visited
			}
			if (neighborsOnPlane > neighborThreshold) {
				verticesOnSurface.Add (currentPoint.position);
			}
//			debug.text = "there are " + pointsToCheck.Count + " points left in the queue";
		}
//		debug.text = "Found " + verticesOnSurface.Count + " vertices.";
		return verticesOnSurface;
	}

}
