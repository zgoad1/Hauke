using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTSBBI {

	// methods that should be built in

	public static void SetActiveChildren(Transform transform, bool value) {
		if(transform != null) {
			//Debug.Log("Setting active: " + transform + ", " + value);
			transform.gameObject.SetActive(value);
			foreach(Transform child in transform) {
				SetActiveChildren(child, value);
			}
		}
	}
}
