using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Tango;

public class TouchController : MonoBehaviour {
	public Text debug;
	public GameObject line;
	public Surface surfaceTemplate;
	public TangoPointCloud tangoPointCloud;
	private Vector3 firstCorner;
	private Vector3 oppositeCorner;
	private bool hasStartPoint = false;
	private float planeDistanceThreshold = 0.075f;

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
			Vector3 closestPoint = tangoPointCloud.m_points [closestPointIndex]; // Returns -1 if not found
			if (closestPointIndex != -1) {
				if (!hasStartPoint) {
					StartLine (closestPoint);
					firstCorner = closestPoint;
					hasStartPoint = true;
				}
				ExtendLine (closestPoint);
				oppositeCorner = closestPoint;
			}
			if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
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

	private bool CreateSurface() {
		Vector3 planeCenter;
		Plane plane;
		if (!tangoPointCloud.FindPlane (Camera.main, Camera.main.WorldToScreenPoint(Vector3.Lerp (firstCorner, oppositeCorner, 0.5f)), out planeCenter, out plane)) {
			debug.text = "No surface found. Please try again.";
			return false;
		}
		Surface surface = Instantiate (surfaceTemplate) as Surface;
		SurfaceMesh surfaceMesh;
		if (MainMenuController.GetEdgeDetectionMode() == "DRAG") {
			surfaceMesh = new DragSurfaceMesh(plane, planeCenter, firstCorner, oppositeCorner);
		} else {
			surfaceMesh = new TapSurfaceMesh(plane, planeCenter, FindVerticesOnPlane(plane, planeCenter));
			if (!surfaceMesh.HasVertices ()) {
				debug.text = "No vertices found on the tapped surface. Please try again.";
				return false;
			}
		}
		surface.Create (plane, planeCenter, surfaceMesh.mesh);
		return true;
	}

	private void ExtendLine(Vector3 end) {
		LineRenderer lr = line.GetComponent<LineRenderer>();
		lr.SetPosition(1, end);
	}

	private List<Vector3> FindVerticesOnPlane(Plane plane, Vector3 planeCenter) {
		// Narrows point cloud to points on the plane
		var verticesOnPlane = new List<Vector3>();
		for (int i = 0; i < tangoPointCloud.m_pointsCount; i++) {
			var p = tangoPointCloud.m_points [i];
			if (Mathf.Abs (plane.GetDistanceToPoint (p)) <= planeDistanceThreshold) {
				verticesOnPlane.Add (p);
			}
		}
		return verticesOnPlane;
	}

	private void HandleTouch(Vector2 position) {
		var diagonal = firstCorner - oppositeCorner;
		if (diagonal.magnitude < 0.1f) { // Treat as a tap
			//found + (drag || tap) = return //don't care about mode; if we find something, don't create a surface
			//nothing found + drag = return //don't create tiny surfaces in drag mode
			//nothing found + tap = create
			if (TrySelectSurface (position) || MainMenuController.GetEdgeDetectionMode () == "DRAG") {
				return;
			}
		}
		CreateSurface (); //FIXME? will create a tap surface if user drags (regardless of mode)
	}

	private void StartLine(Vector3 start)
	{
		line.transform.position = start;
		LineRenderer lr = line.GetComponent<LineRenderer>();
		lr.SetPosition(0, start);
		lr.SetPosition(1, start);
		line.SetActive(true);
	}

	private bool TrySelectSurface(Vector2 touch) {
		//Check if you hit a UI element (http://answers.unity3d.com/questions/821590/unity-46-how-to-raycast-against-ugui-objects-from.html)
		var pointer = new PointerEventData(EventSystem.current);
		pointer.position = touch;
		var results = new List<RaycastResult> ();
		EventSystem.current.RaycastAll(pointer, results); // Outputs to 'results'
		if (results.Count == 0) {
			//Check if you hit a surface
			RaycastHit hit;
			var ray = Camera.main.ScreenPointToRay (touch);
			var layerMask = 1 << LayerMask.NameToLayer("Ignore Raycast"); //http://answers.unity3d.com/questions/8715/how-do-i-use-layermasks.html
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
}
