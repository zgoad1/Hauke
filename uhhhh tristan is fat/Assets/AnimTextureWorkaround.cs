using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTextureWorkaround : MonoBehaviour {

    Material mat;
    public Vector2 offset;

	// Use this for initialization
	void Start () {
        mat = GetComponent<MeshRenderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
        mat.mainTextureOffset = offset;
	}
}
