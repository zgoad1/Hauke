﻿using UnityEngine;

public class CameraControl : MonoBehaviour {

    public Transform lookAt;
    private Transform desTransform;      // desired position
    public Transform adjTransform;      // adjusted position
    public Transform camTransform;      // camera position (lerps towards adjusted position)
    public float rad = 0.5f;
    public float lerpFac;
    public float iLerpFac = 0.2f;
    public float camSensitivity = 1f;
    public float idistance = 14;
    public float distance;

    private float currentX = 0;
    private float currentY = 0;
    private float sensitivityX = 4;
    private float sensitivityY = 2;
    private Vector3 lookOffset = new Vector3(0f, 1f, 0f);
    private int inverted = 1;
	private int raymask;

    // Use this for initialization
    void Start() {
        desTransform = transform;
        lerpFac = iLerpFac;
        sensitivityX *= camSensitivity;
        sensitivityY *= camSensitivity;
		raymask = 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("IgnoreCollision");
	}

    // Update is called once per frame
    void Update() {
        currentX += sensitivityX * Input.GetAxis("Mouse X");
        currentY = Mathf.Clamp(currentY - sensitivityY * inverted * Input.GetAxis("Mouse Y"), -40f, 75f);
        //Debug.Log(currentY.ToString());

        // invert y axis
        if(Input.GetButtonDown("Invert")) {
            inverted *= -1;
        }
    }

    void LateUpdate() {
        // keep the camera from going through solid colliders
        RaycastHit hit;
        Vector3 rayDir = Vector3.zero;
        try {
            rayDir = desTransform.position - lookAt.position;
        } catch(MissingReferenceException e) {
            Debug.LogWarning("Cannot find " + lookAt.ToString());
        }
        if(Physics.Raycast(lookAt.position, rayDir, out hit, idistance, raymask)) {
            if(hit.transform.gameObject.tag != "Transparent") {
                Vector3 newPos = hit.point + hit.normal * rad;
                distance = Mathf.Min(idistance, Vector3.Distance(lookAt.position, newPos));
                //Debug.Log("Hit non-transparent (" + hit.transform.gameObject.ToString() + "), distance = " + distance);
                setCam(distance);
                adjTransform.position = newPos;
            } else {
                distance = idistance;
                setCam(distance);
            }
        } else {
            distance = idistance;
            setCam(distance);
        }
    }

    // camera lerping must go here or else it jitters (because Unity)
    void FixedUpdate() {
        camTransform.position = Vector3.Lerp(camTransform.position, adjTransform.position, lerpFac);
        camTransform.LookAt(lookAt.position + lookOffset);
    }

    void setCam(float distance) {
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rot = Quaternion.Euler(currentY, currentX, 0);

        // move adjusted camera position
        adjTransform.position = lookAt.position + rot * dir;

        // move desired camera position
        Vector3 dDir = new Vector3(0, 0, -idistance);
        desTransform.position = lookAt.position + rot * dDir;
    }
}