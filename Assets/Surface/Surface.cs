﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Surface : MonoBehaviour {
	private Vector3 _center;
	private string _color;
	private Plane _plane;
	private Mesh _mesh;

	void Start() {
		_color = "Default";
	}

	/// <summary>
	/// Gets the Surface's brick color.
	/// </summary>
	public string GetBrickColor() {
		return _color;
	}

	/// <summary>
	/// Sets the brick material of the Surface.
	/// </summary>
	/// <param name="material">Brick material.</param>
	public void SetMaterial(Material material) {
		_color = material.name.Replace("Occluded", "");
		GetComponent<MeshRenderer> ().material = material;
	}

	/// <summary>
	/// Sets the mesh.
	/// </summary>
	/// <param name="mesh">Mesh created by either TapSurfaceMesh or DragSurfaceMesh.</param>
	public void SetMeshAndSelect(Mesh mesh) {
		GetComponent<MeshFilter>().mesh = mesh; //should this also be sharedMesh?
		GetComponent<MeshCollider>().sharedMesh = mesh;
		SelectableBehavior.SelectSurface (this);
	}

	/// <summary>
	/// Creates the surface coordinate system.
	/// </summary>
	/// <param name="plane">Plane found by Tango in the point cloud.</param>
	/// <param name="center">World coordinates of plane center.</param>
	public void SetTransform(Plane plane, Vector3 center) {
		//Member variables
		_center = center;
		_plane = plane;
		//Plane coordinate system
		var xaxis = Quaternion.LookRotation(-_plane.normal) * Vector3.right; //Horizontal vector transformed to plane's rotation
		var yaxis = Vector3.Cross(xaxis, _plane.normal);
		//Position + Rotation
		transform.position = _center;
		transform.rotation = Quaternion.LookRotation (-_plane.normal, yaxis);
	}

	/// <summary>
	/// Destroys this Surface.
	/// </summary>
	public void Undo() {
		DestroyImmediate (gameObject);
	}
}
