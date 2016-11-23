using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Tango;

public class TouchController : MonoBehaviour {
	public MenuController brickMenu;
	public Text debug; //For testing purposes
	public float planeDistanceThreshold = 0.05f;
	public Surface surfaceTemplate;
	public TangoPointCloud tangoPointCloud;

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
			Surface surface = Instantiate(surfaceTemplate) as Surface;
			surface.Create(FindSurfaceVertices(plane), plane, planeCenter, brickMenu.GetCurrentMaterial());
			return true;
		}
		return false;
	}

	private List<Vector3> FindSurfaceVertices(Plane plane) {
		List<Vector3> vertices = new List<Vector3> ();
		for (int i = 0; i < tangoPointCloud.m_pointsCount; i++) {
			Vector3 p = tangoPointCloud.m_points [i];
			if (Mathf.Abs(plane.GetDistanceToPoint(p)) <= planeDistanceThreshold) {
				vertices.Add(p);
			}
		}
		debug.text = "Found " + vertices.Count + " vertices.";
		return vertices;
	}

}
