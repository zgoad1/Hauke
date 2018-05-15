using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rectangle {

	/* Assumes horizontal rectangles, i.e. parallel to ground.
	 */

	private float width;
	private float length;
	private float x;	// center x
	private float z;    // center y
	private Vector2 pt;	// for use in PointInBounds(Vector3)

	public Rectangle(float x, float y, float l, float w) {
		length = l;
		width = w;
		this.x = x;
		this.z = y;
	}

	public Rectangle(Vector3 p, float l, float w) {
		length = l;
		width = w;
		x = p.x;
		z = p.z;
	}

	public void SetPos(Vector3 p) {
		x = p.x;
		z = p.z;
	}

	public bool PointInBounds(Vector2 point) {
		return point.x > x - length / 2 && point.x < x + length / 2 &&
			   point.y > z - width / 2 && point.y < z + width / 2;
	}

	public bool PointInBounds(Vector3 point) {
		pt.x = point.x;
		pt.y = point.z;
		return PointInBounds(pt);
	}
}
