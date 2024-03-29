﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DragSurfaceMesh : SurfaceMesh {
	private Vector3 _firstCorner;
	private Vector3 _oppositeCorner;

	public DragSurfaceMesh(Surface associatedSurface, Vector3 firstCorner, Vector3 oppositeCorner) {
		_associatedSurface = associatedSurface;
		_firstCorner = firstCorner;
		_oppositeCorner = oppositeCorner;
		mesh = CreateMesh();
	}

	protected override int[] FindTriangles ()
	{
		var triangles = new List<int>();
		triangles.Add (0);
		triangles.Add (2);
		triangles.Add (1);
		//Upper right triangle.   
		triangles.Add (2);
		triangles.Add (3);
		triangles.Add (1);
		return triangles.ToArray ();
	}

	protected override Vector3[] FindVertices ()
	{
		//Put vectors in local space
		var corner1 = _associatedSurface.transform.InverseTransformPoint(_firstCorner);
		corner1.z = 0;
		var corner2 = _associatedSurface.transform.InverseTransformPoint(_oppositeCorner);
		corner2.z = 0;
		//Find min/max coordinate values
		var min = new Vector2(Mathf.Min(corner1.x, corner2.x), Mathf.Min(corner1.y, corner2.y));
		var max = new Vector2(Mathf.Max(corner1.x, corner2.x), Mathf.Max(corner1.y, corner2.y));

		var corners = new Vector3[4];

		corners[0] = new Vector3(min.x, min.y, 0); //bottom left
		corners[1] = new Vector3(max.x, min.y, 0); //bottom right
		corners[2] = new Vector3(min.x, max.y, 0); //top left
		corners[3] = new Vector3(max.x, max.y, 0); //top right

		return corners;
	}
}
