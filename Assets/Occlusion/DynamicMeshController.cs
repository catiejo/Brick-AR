using UnityEngine;
using System.Collections;

public class DynamicMeshController : MonoBehaviour {
	private static DynamicMeshController _instance;

	void Start() {
		_instance = this; //set our static reference to our newly initialized instance
		SetShaderAlpha(0.75f);
	}

	public static void SetShaderAlpha(float alpha) {
		_instance.GetComponent<MeshRenderer> ().material.SetFloat("_Alpha", alpha);
	}
}
