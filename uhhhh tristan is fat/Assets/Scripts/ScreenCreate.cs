using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCreate : MonoBehaviour {

    private RectTransform scr;

	// Use this for initialization
	void Start () {
		// scale RawImage to fit window size
		scr = GetComponent<RectTransform>();
        float scale = Screen.width / 1280f;
        scr.localScale = new Vector3(scale, scale, 0f);
    }
}
