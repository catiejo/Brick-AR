﻿using UnityEngine;
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
		//FIXME: IsPointerOverGameObject doesn't work for TouchPhase.Ended 
//		for(int i = 0; i < Input.touchCount; i++)
		if (Input.touchCount >= 1)
		{
			Touch touch = Input.GetTouch (0);
			bool pointOverGameObject = EventSystem.current.IsPointerOverGameObject (touch.fingerId);
			if (touch.phase == TouchPhase.Began && !pointOverGameObject) {
				depthText.text = " " + touch.fingerId.ToString() + "=" + pointOverGameObject.ToString() + " " + depthText.text; // For testing purposes
				PositionBricks (touch.position);
			}
		}
		// For testing purposes
//		if (Input.GetMouseButtonDown (0) && !EventSystem.current.IsPointerOverGameObject()) PositionBricks (Input.mousePosition);
	}

	void PositionBricks(Vector2 touchCoordinates) {
//		depthText.text = "average depth is: " + pointCloud.m_overallZ; // For testing purposes
		float x = (float)(touchCoordinates.x / Screen.width);
		float y = (float)(touchCoordinates.y / Screen.height);
		float z = pointCloud.m_overallZ;
		GameObject brickpic = GameObject.CreatePrimitive (PrimitiveType.Quad);
		brickpic.transform.position = camera.ViewportToWorldPoint(new Vector3(x, y, z));
		brickpic.transform.rotation = Quaternion.LookRotation(camera.transform.forward);
		brickpic.GetComponent<Renderer> ().material = menu.GetCurrentMaterial();
	}

	private bool IsTouchingUI() {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}