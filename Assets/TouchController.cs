using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Tango;

public class TouchController : MonoBehaviour {
	public Text depthText; // For testing purposes
	public MenuController menu;
	public Camera camera;
	public TangoPointCloud pointCloud;

	void Update () {
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch (0);
			//FIXME: IsPointerOverGameObject doesn't work for TouchPhase.Ended 
			if (touch.phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject (touch.fingerId)) {
				PositionBricks (touch.position);
			}
		}
	}

	void PositionBricks(Vector2 touchCoordinates) {
		Vector3 planeCenter;
		Vector3 forward;
		Vector3 up;
		Plane plane;
		if (pointCloud.FindPlane (camera, touchCoordinates, out planeCenter, out plane)) {
			up = plane.normal;
			float angle = Vector3.Angle (up, camera.transform.forward);
			depthText.text = "angle with normal is " + angle + " degrees.";
			if (angle < 175) {
				Vector3 right = Vector3.Cross(up, camera.transform.forward).normalized;
				forward = Vector3.Cross(right, up).normalized;
			} else {
				// Normal is nearly parallel to camera look direction, the cross product would have too much
				// floating point error in it.
				forward = Vector3.Cross(up, camera.transform.right);
			}
			GameObject brickpic = GameObject.CreatePrimitive (PrimitiveType.Plane);
			brickpic.transform.localScale *= 0.05f;
			brickpic.transform.position = planeCenter;
			brickpic.transform.rotation = Quaternion.LookRotation(forward, up);
			brickpic.GetComponent<Renderer> ().material = menu.GetCurrentMaterial();
		} else {
			depthText.text = "No plane in sight...";
		}
//		float x = (float)(touchCoordinates.x / Screen.width);
//		float y = (float)(touchCoordinates.y / Screen.height);
//		float z = pointCloud.m_overallZ;
//		brickpic.transform.position = camera.ViewportToWorldPoint(new Vector3(x, y, z));
	}
}
