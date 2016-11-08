using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using Tango;

public class TouchController : MonoBehaviour {
	public Text touchText;
	public BrickButtons menu;
	public Camera camera;
	public TangoPointCloud pointCloud;
	private int touchCount = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch (i);
			if (touch.phase == TouchPhase.Ended && !EventSystem.current.IsPointerOverGameObject()) {
				PlacePlane (touch.position);
			}
		}
		// For testing purposes
		if (Input.GetMouseButtonDown (0) && !EventSystem.current.IsPointerOverGameObject()) PlacePlane (Input.mousePosition);
	}

	void PlacePlane(Vector2 touchCoordinates) {
		touchText.text = "average depth is: " + pointCloud.m_overallZ; // For testing purposes
		float x = (float)(touchCoordinates.x / Screen.width);
		float y = (float)(touchCoordinates.y / Screen.height);
		float z = pointCloud.m_overallZ;
		Texture brickTexture = menu.GetCurrentTexture ();
		GameObject bricks = GameObject.CreatePrimitive (PrimitiveType.Quad);
		bricks.transform.position = camera.ViewportToWorldPoint(new Vector3(x, y, z));
		bricks.transform.rotation = Quaternion.LookRotation(camera.transform.forward);
//		bricks.transform.localScale *= 0.1f;
		bricks.GetComponent<Renderer> ().material.SetTexture("_MainTex", brickTexture);
	}
}
