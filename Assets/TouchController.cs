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
	private float _threshold = 0;

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
			depthText.text = pointCloud.m_pointsCount.ToString();
			int count = 0;
			Vector3 topLeft = planeCenter;
			Vector3 bottomRight = planeCenter;
			Vector3 x = planeCenter;
			Vector3 y = planeCenter;
			Vector3 z = planeCenter;
			for (int i = 0; i < pointCloud.m_pointsCount; i++) {
				Vector3 p = pointCloud.m_points [i];
				if (Mathf.Abs(plane.GetDistanceToPoint(p)) <= _threshold) {
					count++;
//					if (p.x >= x.x) {
//						x = p;
//					}
//					if (p.y >= y.y) {
//						y = p;
//					}
//					if (p.z >= z.z) {
//						z = p;
//					}
//					if (isBetterTopLeft (p, topLeft)) {
//						topLeft = p;
//					}
//					if (isBetterBottomRight (p, bottomRight)) {
//						bottomRight = p;
//					}
				}
				depthText.text = count + "/" + pointCloud.m_pointsCount + " points on surface";
			}
//			GameObject topLeftPt = GameObject.CreatePrimitive (PrimitiveType.Sphere);
//			topLeftPt.transform.localScale *= 0.05f;
//			topLeftPt.transform.position = topLeft;
//			GameObject bottomRightPt = GameObject.CreatePrimitive (PrimitiveType.Sphere);
//			bottomRightPt.transform.localScale *= 0.05f;
//			bottomRightPt.transform.position = bottomRight;
//			GameObject xPt = GameObject.CreatePrimitive (PrimitiveType.Sphere);
//			xPt.transform.localScale *= 0.05f;
//			xPt.transform.position = x;
//			xPt.GetComponent<Renderer> ().material.color = Color.red;
//			GameObject yPt = GameObject.CreatePrimitive (PrimitiveType.Sphere);
//			yPt.transform.localScale *= 0.05f;
//			yPt.transform.position = x;
//			yPt.GetComponent<Renderer> ().material.color = Color.green;
//			GameObject zPt = GameObject.CreatePrimitive (PrimitiveType.Sphere);
//			zPt.transform.localScale *= 0.05f;
//			zPt.transform.position = x;
//			zPt.GetComponent<Renderer> ().material.color = Color.blue;
			up = plane.normal;
			float angle = Vector3.Angle (up, camera.transform.forward);
//			depthText.text = "angle with normal is " + angle + " degrees.";
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

	private bool isBetterTopLeft(Vector3 checkPoint, Vector3 refPoint) {
//		if (checkPoint.x <= refPoint.x && checkPoint.y <= refPoint.y && checkPoint.z <= refPoint.z) {
		if (checkPoint.x <= refPoint.x && checkPoint.y <= refPoint.y) {
			return true;
		}
		return false;
	}

	private bool isBetterBottomRight(Vector3 checkPoint, Vector3 refPoint) {
//		if (checkPoint.x >= refPoint.x && checkPoint.y >= refPoint.y && checkPoint.z >= refPoint.z) {
		if (checkPoint.x >= refPoint.x && checkPoint.y >= refPoint.y) {
			return true;
		}
		return false;
	}

	public void incThresh() {
		_threshold += 0.01f;
		depthText.text = "Threshold is now " + _threshold;
	}

	public void decThresh() {
		if (_threshold > 0) {
			_threshold -= 0.01f;
		}
		depthText.text = "Threshold is now " + _threshold;
	}

	public void FindSurface() {
		//Make an empty list of Vector3
		//Iterate through the point cloud
			//if a point lies on the plane (or is within a certain margin of error), add it to the list
		//Compute the convex hull of the list
	}
}
