using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MTSBBI {

	// methods that should be built in

	// When I made this I didn't know that deactivating an object kind of does its children as well
	public static void SetActiveChildren(Transform transform, bool value) {
		if(transform != null) {
			//Debug.Log("Setting active: " + transform + ", " + value);
			transform.gameObject.SetActive(value);
			foreach(Transform child in transform) {
				SetActiveChildren(child, value);
			}
		}
	}

	/** An infinitely better, more versatile version of LookAt().
	 *  Use oct as an binary code for X, Y, and Z axis rotation. (zyx) (0-7)
	 *  Uses LookAt in itself which is shoddy but idk how to do it manually.
	 */
	public static void LookAtXYZ(Transform t1, Transform t2, int oct, float lerpFac) {
		Vector3 oldRot = t1.localEulerAngles;
		t1.LookAt(t2);
		Vector3 newRot = t1.localEulerAngles;
		t1.localRotation = Quaternion.Euler(oldRot);
		if((oct & 1) == 1) {
			oldRot.x = newRot.x;
		}
		if((oct & 2) == 2) {
			oldRot.y = newRot.y;
		}
		if((oct & 4) == 4) {
			oldRot.z = newRot.z;
		}
		t1.localRotation = Quaternion.Lerp(t1.localRotation, Quaternion.Euler(oldRot), lerpFac);
	}

	public static GameObject Closest(GameObject me, GameObject[] obs) {
		GameObject ans = obs[0];
		float ansDist = Vector3.Distance(me.transform.position, ans.transform.position);
		foreach(GameObject g in obs) {
			float thisDist = Vector3.Distance(me.transform.position, g.transform.position);
			if(thisDist < ansDist) {
				ans = g;
				ansDist = thisDist;
			}
		}
		return ans;
	}

	public static GameObject Closest(GameObject me, List<GameObject> obs) {
		return Closest(me, obs.ToArray());
	}

	public static bool AnyInArray(bool[] a) {
		foreach(bool b in a) if(b) return true;
		return false;
	}
}
