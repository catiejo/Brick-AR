using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TouchController : MonoBehaviour {
	public Text touchText;
	private int touchCount = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
//		Touch myTouch = Input.GetTouch(0);

//		Touch[] myTouches = Input.touches;
//		foreach (Touch t in myTouches)
		for(int i = 0; i < Input.touchCount; i++)
		{
			if (Input.GetTouch (i).phase == TouchPhase.Began) {
				touchCount++;
				//Do something with the touches
				touchText.text = touchCount + " touches";
			}
		}	
	}
}
