using UnityEngine;
using System.Collections;

public class TurnOnDepthBuffer : MonoBehaviour {

	void Start ()
	{
		Camera.main.depthTextureMode = DepthTextureMode.Depth;
	}
}