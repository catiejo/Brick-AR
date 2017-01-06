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

	private Vector3 _firstCorner;
	private bool _hasStartPoint = false;
	private Vector3 _oppositeCorner;

	void Start() {
		var planeCenter = new Vector3(1, 1, 1);
		var plane = new Plane (Quaternion.Euler(30, 60, 70) * -Vector3.forward, planeCenter);
		_firstCorner = planeCenter + new Vector3 (1, 1, 1);
		_oppositeCorner = planeCenter + new Vector3 (-1, -1, -1);
		Surface surface = Instantiate (surfaceTemplate) as Surface;
		SurfaceMesh surfaceMesh;
		if (MainMenuController.GetEdgeDetectionMode() == "DRAG") {
			debug.text = "Drag";
			surfaceMesh = new DragSurfaceMesh(plane, planeCenter, _firstCorner, _oppositeCorner);
		} else {
			surfaceMesh = new TapSurfaceMesh(plane, planeCenter, FindVerticesOnPlane(plane));
			if (!surfaceMesh.HasVertices ()) {
				debug.text = "No vertices found on the tapped surface. Please try again.";
			}
		}
		surface.Create (plane, planeCenter, surfaceMesh.mesh);
	}

	void Update () {
		if (Input.touchCount > 0)
		{	
			Touch touch = Input.GetTouch (0);
			int closestPointIndex = tangoPointCloud.FindClosestPoint (Camera.main, touch.position, 500);
			Vector3 closestPoint = tangoPointCloud.m_points [closestPointIndex]; // Returns -1 if not found
			if (closestPointIndex != -1) {
				if (!_hasStartPoint) {
					StartLine (closestPoint);
					_firstCorner = closestPoint;
					_hasStartPoint = true;
				}
				ExtendLine (closestPoint);
				_oppositeCorner = closestPoint;
			}
			if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
				line.SetActive(false);
				if (_hasStartPoint) {
					_hasStartPoint = false;
					HandleTouch (touch.position);
				}
			}
		}
	}

	/// <summary>
	/// Creates a Surface on a real-world plane using either a DragSurfaceMesh or TapSurfaceMesh.
	/// </summary>
	/// <returns><c>true</c>, if Surface was successfully created, <c>false</c> otherwise.</returns>
	private bool CreateSurface() {
		Vector3 planeCenter;
		Plane plane;
		if (!tangoPointCloud.FindPlane (Camera.main, Camera.main.WorldToScreenPoint(Vector3.Lerp (_firstCorner, _oppositeCorner, 0.5f)), out planeCenter, out plane)) {
			debug.text = "No surface found. Please try again.";
			return false;
		}
		Surface surface = Instantiate (surfaceTemplate) as Surface;
		SurfaceMesh surfaceMesh;
		if (MainMenuController.GetEdgeDetectionMode() == "DRAG") {
			surfaceMesh = new DragSurfaceMesh(plane, planeCenter, _firstCorner, _oppositeCorner);
		} else {
			surfaceMesh = new TapSurfaceMesh(plane, planeCenter, FindVerticesOnPlane(plane));
			if (!surfaceMesh.HasVertices ()) {
				debug.text = "No vertices found on the tapped surface. Please try again.";
				return false;
			}
		}
		surface.Create (plane, planeCenter, surfaceMesh.mesh);
		return true;
	}

	/// <summary>
	/// Extends the line renderer end point.
	/// </summary>
	/// <param name="end">New end point.</param>
	private void ExtendLine(Vector3 end) {
		LineRenderer lr = line.GetComponent<LineRenderer>();
		lr.SetPosition(1, end);
	}

	/// <summary>
	/// Narrows point cloud to points on the plane.
	/// </summary>
	/// <returns>All point cloud vertices within a given threshold of the passed plane.</returns>
	/// <param name="plane">The plane.</param>
	private List<Vector3> FindVerticesOnPlane(Plane plane) {
		var planeDistanceThreshold = 0.075f;
		var verticesOnPlane = new List<Vector3>();
		for (int i = 0; i < tangoPointCloud.m_pointsCount; i++) {
			var p = tangoPointCloud.m_points [i];
			if (Mathf.Abs (plane.GetDistanceToPoint (p)) <= planeDistanceThreshold) {
				verticesOnPlane.Add (p);
			}
		}
		return verticesOnPlane;
	}

	/// <summary>
	/// Handles the touch by either selecting a Surface or creating a new one at the given point. 
	/// </summary>
	/// <param name="position">Touch position.</param>
	private void HandleTouch(Vector2 position) {
		var diagonal = _firstCorner - _oppositeCorner;
		// Check if user tapped
		if (diagonal.magnitude < 0.1f) {
			if (TrySelectSurface (position) || MainMenuController.GetEdgeDetectionMode () == "DRAG") {
				return;
			}
		}
		CreateSurface (); // Will always create a surface if user drags (regardless of mode)
	}

	/// <summary>
	/// Sets line renderer to current touch position (in world space)
	/// </summary>
	/// <param name="start">Start point.</param>
	private void StartLine(Vector3 start)
	{
		line.transform.position = start;
		LineRenderer lr = line.GetComponent<LineRenderer>();
		lr.SetPosition(0, start);
		lr.SetPosition(1, start);
		line.SetActive(true);
	}

	/// <summary>
	/// Selects the surface the user tapped on.
	/// </summary>
	/// <returns><c>true</c>, if a surface is found at the touch position, <c>false</c> otherwise.</returns>
	/// <param name="touch">Touch position.</param>
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
				SelectableBehavior selected = hit.collider.gameObject.GetComponent<SelectableBehavior> ();
				if (selected != null) {
					selected.SelectSurface (selected.associatedSurface);
					return true;
				}
			}
		}
		return false;
	}
}
