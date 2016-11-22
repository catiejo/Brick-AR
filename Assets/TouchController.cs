using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Tango;

public class TouchController : MonoBehaviour {
	public Text depthText; // For testing purposes
	public MenuController menu;
	public TangoPointCloud pc;

	void Update () {
		if (Input.touchCount > 0)
		{	
			Touch touch = Input.GetTouch (0);
			if (touch.phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject (touch.fingerId)) {
				if (!CreateSurface (touch.position)) {
					depthText.text = "Unable to find a surface. Please try again.";
				}
			}
		}
	}

	private bool CreateSurface(Vector2 touch) {
		depthText.text = "boo! ";
		List<Vector3> vertices = new List<Vector3> ();
		Vector3 planeCenter;
		Plane plane;
		depthText.text += "there are " + pc.m_pointsCount + " points in point cloud";
		if (pc.FindPlane (Camera.main, touch, out planeCenter, out plane)) {
			for (int i = 0; i < pc.m_pointsCount; i++) {
				Vector3 p = pc.m_points [i];
				if (Mathf.Abs(plane.GetDistanceToPoint(p)) <= 0.02f) {
//					vertices.Add(Vector3.ProjectOnPlane(p, plane.normal));
					vertices.Add(p);
				}
			}
			depthText.text = "Found " + vertices.Count + " vertices.";
			Surface surf = Instantiate(Resources.Load("Surface")) as Surface;
			surf.Create(vertices.ToArray(), plane, planeCenter, menu.GetCurrentMaterial());
			return true;
		}
		return false;
	}

}
