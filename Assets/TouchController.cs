using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using Tango;

public class TouchController : MonoBehaviour {
	public GameObject line;
	public Surface surfaceTemplate;
	public TangoPointCloud tangoPointCloud;

	private Vector3 _firstCorner;
	private bool _hasStartPoint = false;
	private Vector3 _oppositeCorner;
	private int _stationaryCount = 0;

	void Update () {
		if (Input.touchCount > 0)
		{	
			Touch touch = Input.GetTouch (0);
			int indexOfClosestPoint = tangoPointCloud.FindClosestPoint (Camera.main, touch.position, 500); // Returns -1 if not found

			if (indexOfClosestPoint != -1) {
				var closestPoint = tangoPointCloud.m_points [indexOfClosestPoint];
				var showDragLine = MainMenuController.GetEdgeDetectionMode () == "DRAG";
				if (!_hasStartPoint) {
					_hasStartPoint = true;
					_firstCorner = closestPoint;
					if (showDragLine) { 
						StartLine (closestPoint); 
					}
				}
				_oppositeCorner = closestPoint;
				if (showDragLine) { 
					ExtendLine (closestPoint); 
				}
			}
			if (touch.phase == TouchPhase.Stationary) {
				_stationaryCount++;
			}
			if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled || _stationaryCount > 100) {
				_stationaryCount = 0;
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
			ScreenLog.Write("Please try again.");
			return false;
		}
		Surface surface = Instantiate (surfaceTemplate) as Surface;
		surface.SetTransform (plane, planeCenter);
		SurfaceMesh surfaceMesh;
		var mode = MainMenuController.GetEdgeDetectionMode ();
		if (mode == "DRAG") {
			surfaceMesh = SurfaceMesh.Create(mode, surface, _firstCorner, _oppositeCorner);
		} else {
			surfaceMesh = SurfaceMesh.Create(mode, surface, FindVerticesOnPlane(plane));
		}
		if (surfaceMesh == null) {
			//Do I need to destroy the surfaceMesh object, too?
			ScreenLog.Write("Please try again.");
			surface.Undo ();
			return false;
		}
		surface.SetMeshAndSelect (surfaceMesh.mesh);
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
		ScreenLog.Write ("Found " + verticesOnPlane.Count + " vertices on plane");
		return verticesOnPlane;
	}

	/// <summary>
	/// Handles the touch by either selecting a Surface or creating a new one at the given point. 
	/// </summary>
	/// <param name="position">Touch position.</param>
	private void HandleTouch(Vector2 position) {
		if (TouchIsOnUI (position)) {
			return;
		}
		ScreenLog.Clear ();
		// Check if user tapped
		var diagonal = _firstCorner - _oppositeCorner;
		if (diagonal.magnitude < 0.1f) {
			if (TrySelectSurface (position) || MainMenuController.GetEdgeDetectionMode () == "DRAG") {
				return;
			}
		}
		CreateSurface (); // Will always create a surface if diagonal > 0.1 (regardless of mode)
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

	private bool TouchIsOnUI(Vector2 touch) {
		//Check if you hit a UI element (http://answers.unity3d.com/questions/821590/unity-46-how-to-raycast-against-ugui-objects-from.html)
		var pointer = new PointerEventData(EventSystem.current);
		pointer.position = touch;
		var results = new List<RaycastResult> ();
		EventSystem.current.RaycastAll(pointer, results); // Outputs to 'results'
		return results.Count > 0;
	}

	/// <summary>
	/// Selects the surface the user tapped on.
	/// </summary>
	/// <returns><c>true</c>, if a surface is found at the touch position, <c>false</c> otherwise.</returns>
	/// <param name="touch">Touch position.</param>
	private bool TrySelectSurface(Vector2 touch) {
		//Check if you hit a surface
		RaycastHit hit;
		var ray = Camera.main.ScreenPointToRay (touch);
		var layerMask = 1 << LayerMask.NameToLayer("Ignore Raycast"); //http://answers.unity3d.com/questions/8715/how-do-i-use-layermasks.html
		if (Physics.Raycast (ray.origin, ray.direction, out hit, layerMask)) {
			var selected = hit.collider.gameObject.GetComponent<Surface> ();
			if (selected != null) {
				SelectableBehavior.SelectSurface (selected);
				return true;
			}
		}
		return false;
	}
}
