using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Tango;
using KDTree;

public class TouchController : MonoBehaviour {
	public Text debug;
	public GameObject line;
	public Surface surfaceTemplate;
	public TangoPointCloud tangoPointCloud;
	private Vector3 firstCorner;
	private Vector3 oppositeCorner;
	private bool hasStartPoint = false;
	private bool _dragEdgeDetectionMode = true;
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


	/* USEFUL FOR DEBUGGING */
//	void Start() {
//		var center = new Vector3(1, 1, 1);
//		var plane = new Plane (Quaternion.Euler(30, 60, 70) * -Vector3.forward, center);
//		NewSurface surface = Instantiate (surfaceTemplate) as NewSurface;
//		surface.Create (plane, center + new Vector3(1, 1, 1), center + new Vector3(-1, -1, -1), center);
//	}

	void Update () {
		if (Input.touchCount > 0)
		{	
			Touch touch = Input.GetTouch (0);
			int closestPointIndex = tangoPointCloud.FindClosestPoint (Camera.main, touch.position, 500);
			Vector3 closestPoint = tangoPointCloud.m_points [closestPointIndex];
			if (closestPointIndex != -1) {
				if (!hasStartPoint) {
					StartLine (closestPoint);
					firstCorner = closestPoint;
					hasStartPoint = true;
				}
				ExtendLine (closestPoint);
				oppositeCorner = closestPoint;
			}
			if (touch.phase == TouchPhase.Ended) {
				line.SetActive(false);
				if (hasStartPoint) {
					hasStartPoint = false;
					HandleTouch (touch.position);
				}
			}
		}
		/* USEFUL FOR DEBUGGING */
//		if (Input.GetMouseButtonDown(0)) {
//		}
	}

	private void StartLine(Vector3 start)
	{
		line.transform.position = start;
		LineRenderer lr = line.GetComponent<LineRenderer>();
		lr.SetPosition(0, start);
		lr.SetPosition(1, start);
		line.SetActive(true);
	}

	private void ExtendLine(Vector3 end) {
		LineRenderer lr = line.GetComponent<LineRenderer>();
		lr.SetPosition(1, end);
	}

	private void HandleTouch(Vector2 position) {
		var diagonal = firstCorner - oppositeCorner;
		if (diagonal.magnitude < 0.1f) { //Surface not big enough; could also be a UI tap
			if (!TrySelectSurface (position)) {
				debug.text = "No surface selected.";
			}
		} else {
			CreateSurface ();
		}
	}

	private bool CreateSurface() {
		Vector3 planeCenter;
		Plane plane;
		float lerpAmount = 0.5f;
		Vector3 center = Vector3.Lerp (firstCorner, oppositeCorner, lerpAmount);
		if (!tangoPointCloud.FindPlane (Camera.main, Camera.main.WorldToScreenPoint(center), out planeCenter, out plane)) {
			debug.text = "No surface found. Please try again.";
			return false;
		}
		Surface surface = Instantiate (surfaceTemplate) as Surface;
		if (_dragEdgeDetectionMode) {
			surface.Create (plane, firstCorner, oppositeCorner, planeCenter);
		} else {
			var surfaceVertices = FindSurfaceVertices (plane, planeCenter);
			if (surfaceVertices.Count == 0) {
				debug.text = "No vertices found on the surface.";
				return false;
			}
			surface.Create (plane, surfaceVertices, planeCenter);
		}
		return true;
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

	private bool TrySelectSurface(Vector2 touch) {
		//Check if you hit a UI element (http://answers.unity3d.com/questions/821590/unity-46-how-to-raycast-against-ugui-objects-from.html)
		var pointer = new PointerEventData(EventSystem.current);
		pointer.position = touch;
		var results = new List<RaycastResult> ();
		EventSystem.current.RaycastAll(pointer, results);
		if (results.Count == 0) {
			//Check if you hit a surface
			RaycastHit hit;
			var ray = Camera.main.ScreenPointToRay (touch);
			var layerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");
			if (Physics.Raycast (ray.origin, ray.direction, out hit, layerMask)) {
				Surface selected = hit.collider.gameObject.GetComponent<Surface> ();
				if (selected != null) {
					selected.SelectSurface ();
					return true;
				}
			}
		}
		return false;
	}

	public void ChangeEdgeDetectionMode(int mode) {
		_dragEdgeDetectionMode = (mode == 0);
	}
}
