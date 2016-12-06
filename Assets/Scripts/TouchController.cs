﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Tango;
using KDTree;

public class TouchController : MonoBehaviour {
	public MenuController brickMenu;
	public Surface surfaceTemplate;
	public TangoPointCloud tangoPointCloud;
	private int neighborCountThreshold = 4;
	private float neighborDistanceThreshold = 0.000175f;
	private float planeDistanceThreshold = 0.075f;
	//Helper class for kdTree
	public class Point : MonoBehaviour
	{
		public double[] doublePosition;
		public Vector3 vectorPosition;
		public bool visited = false;
		public Point(Vector3 point) {
			doublePosition = new double[3]{point.x, point.y, point.z};
			vectorPosition = point;
		}
	}

	/*DEBUGGING*/
	public Text debug;
	public Text nCount;
	public Text nDistance;
	public Text pDistance;
	private Surface surface;
	private Plane surfacePlane;
	private Vector3 surfacePlaneCenter;
	public void updateNeighborDistanceThreshold(float newValue) {
		neighborDistanceThreshold = newValue;
		Invoke("updateSurface", 0.5f);
	}
	public void updatePlaneDistanceThreshold(float newValue) {
		planeDistanceThreshold = newValue;
		Invoke("updateSurface", 0.5f);
	}
	public void updateNeighborCountThreshold(float newValue) {
		neighborCountThreshold = (int) newValue;
		Invoke("updateSurface", 0.5f);
	}
	private void updateSurface() {
		surface.Recreate(FindSurfaceVertices (surfacePlane, surfacePlaneCenter));
		updateParamText ();
	}
	private void updateParamText() {
		nCount.text = neighborCountThreshold.ToString();
		nDistance.text = neighborDistanceThreshold.ToString ();
		pDistance.text = planeDistanceThreshold.ToString();
	}

	void Awake() {
		updateParamText ();
	}


	void Update () {
		if (Input.touchCount > 0)
		{	
			Touch touch = Input.GetTouch (0);
			if (touch.phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject (touch.fingerId)) {
				CreateSurface (touch.position);
			}
		}
//		if (Input.GetMouseButtonDown (0)) {
//			CreateSurface (Input.mousePosition);
//		}
	}

	private bool CreateSurface(Vector2 touch) {
		Vector3 planeCenter;
		Plane plane;
		if (tangoPointCloud.FindPlane (Camera.main, touch, out planeCenter, out plane)) {
			var surfaceVertices = FindSurfaceVertices (plane, planeCenter);
			if (surfaceVertices.Count != 0) {
				surfacePlane = plane; //For debugging
				surfacePlaneCenter = planeCenter; //For debugging
				surface = Instantiate (surfaceTemplate) as Surface;
				surface.Create (surfaceVertices, plane, planeCenter, brickMenu.GetCurrentMaterial ());
				return true;
			}
			debug.text = "No vertices found on the surface.";
			return false;
		}
		debug.text = "No surface found.";
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
		var pointsToCheck = new Queue<Point>();
		pointsToCheck.Enqueue (new Point(planeCenter));
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
				verticesOnSurface.Add (currentPoint.vectorPosition);
			}
		}

		debug.text = "There are " + verticesOnPlane.Count + " vertices on the plane and " + verticesOnSurface.Count + " vertices on the surface.";
		return verticesOnSurface;
	}

}