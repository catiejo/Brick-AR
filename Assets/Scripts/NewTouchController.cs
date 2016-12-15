using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Tango;
using KDTree;

public class NewTouchController : MonoBehaviour {
	public Text debug;
	public GameObject line;
	public NewSurface surfaceTemplate;
	public TangoPointCloud tangoPointCloud;
	private Vector3 firstCorner;
	private Vector3 oppositeCorner;
	private bool hasStartPoint = false;
	//Variables for surface finding
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
//		var vertices = new List<Vector3> ();
//		vertices.Add (center + new Vector3 (0, 1, 0));
//		vertices.Add (center + new Vector3 (1, 0, 0));
//		vertices.Add (center + new Vector3 (0, -1, 0));
//		vertices.Add (center + new Vector3 (-1, 0, 0));
//		vertices.Add (center + new Vector3 (1, 1, 0));
//		vertices.Add (center + new Vector3 (-1, 1, 0));
//		vertices.Add (center + new Vector3 (1, -1, 0));
//		vertices.Add (center + new Vector3 (-1, -1, 0));
//		NewSurface surface = Instantiate (surfaceTemplate) as NewSurface;
//		surface.Create (plane, vertices, center);
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

	private List<Vector3> FindVertices(Plane plane, Vector3 planeCenter) {
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
		return verticesOnSurface;
	}


	private bool CreateSurface() {
		Vector3 planeCenter;
		Plane plane;

		float lerpOffset = 0.25f; //Must be positive to start
		float lerpAmount = 0.5f;
		Vector3 center = Vector3.Lerp (firstCorner, oppositeCorner, lerpAmount);
		List<Vector3> vertices;
		//Search for plane along diagonal, fanning out from center
		while (true) {
			if (tangoPointCloud.FindPlane (Camera.main, Camera.main.WorldToScreenPoint (center), out planeCenter, out plane)) {
				vertices = FindVertices (plane, planeCenter);
				NewSurface surface = Instantiate (surfaceTemplate) as NewSurface;
				if (vertices.Count > 0) {
					surface.Create (plane, firstCorner, oppositeCorner, vertices, planeCenter);
				    return true;
				}
				debug.text = "No surface found. Generating backup plane.";
				surface.Create (plane, firstCorner, oppositeCorner, planeCenter);
				return false;
			}
			if (lerpAmount + Mathf.Abs (lerpOffset) >= 1.0f) { //Exit condition: don't lerp past full length of vector
				debug.text = "No plane found. Please try again.";
				return false;
			}
			lerpAmount = 0.5f + lerpOffset;
			center = Vector3.Lerp (firstCorner, oppositeCorner, lerpAmount);
			lerpOffset *= -1;
			if (lerpOffset > 0) {
				lerpOffset += 0.25f;
			}
		}
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
			if (Physics.Raycast (ray.origin, ray.direction, out hit)) {
				NewSurface selected = hit.collider.gameObject.GetComponent<NewSurface> ();
				if (selected != null) {
					selected.SelectSurface ();
					return true;
				}
			}
		}
		return false;
	}

}
