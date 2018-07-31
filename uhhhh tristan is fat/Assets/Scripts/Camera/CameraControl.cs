using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public Transform lookAt;
	public Transform adjTransform;      // adjusted position
	public Transform camTransform;      // camera position (lerps towards adjusted position)
	private Transform zoomTransform;	// position to move to upon interactions
	public float rad = 0.5f;            // distance from solid to stop at
	[SerializeField] private float height = 4f;
	[HideInInspector] public float lerpFac;
	public float iLerpFac = 0.2f;
	public float camSensitivity = 1f;
	public float idistance = 14;
	[HideInInspector] public float distance;
	[HideInInspector] public bool firstPerson = false;

	private Vector3 dir;
	private Quaternion rot;
	private float currentX = 0;
	private float currentY = 0;
	private float sensitivityX = 4;
	private float sensitivityY = 2;
	private Vector3 lookOffset;
	private int inverted = 1;
	private int raymask;
	private bool zoomIn = false;
	private float iZoomLerpFac = 0.05f;
	private float zoomLerpFac = 1;
	public bool readInput = true;

	// Use this for initialization
	void Start() {
		Reset();

		dir = new Vector3(0, 0, -distance);
		lookOffset = new Vector3(0f, height, 0f);

		lerpFac = iLerpFac;
		sensitivityX *= camSensitivity;
		sensitivityY *= camSensitivity;
		// layers to raycast upon
		raymask = 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("IgnoreCollision") | 1 << LayerMask.NameToLayer("Ground");
	}

	// Update is called once per frame
	void Update() {
		if(!zoomIn && readInput) {
			currentX += sensitivityX * Input.GetAxis("Mouse X");
			currentY = Mathf.Clamp(currentY - sensitivityY * inverted * Input.GetAxis("Mouse Y"), -60f, 75f);
		}

		// invert y axis
		if(Input.GetButtonDown("Invert")) {
			inverted *= -1;
		}
	}

	void LateUpdate() {
		if(zoomIn) {
			camTransform.position = Vector3.Lerp(camTransform.position, zoomTransform.position, 0.05f);
			camTransform.rotation = Quaternion.Lerp(camTransform.rotation, zoomTransform.rotation, 0.05f);
		} else {
			// lerp the zoomLerpFac back up to 1 so our camera rotation doesn't lag behind
			zoomLerpFac = Mathf.Lerp(zoomLerpFac, 1, 0.01f);
			if(!firstPerson) {
				// keep the camera from going through solid colliders
				RaycastHit hit;
				Vector3 rayDir = Vector3.zero;
				rayDir = transform.position - (lookAt.position + lookOffset);
				if(Physics.Raycast(lookAt.position + lookOffset, rayDir, out hit, idistance, raymask)) {
					Vector3 newPos = hit.point + hit.normal * rad;
					distance = Mathf.Min(idistance, Vector3.Distance(lookAt.position + lookOffset, newPos));
					//Debug.Log("Camera raycasted upon a " + hit.transform.gameObject);
					SetCam(distance);
					adjTransform.position = newPos;
				} else {
					distance = idistance;
					SetCam(distance);
				}
			} else {
				SetCam(idistance);
			}
			camTransform.position = Vector3.Lerp(camTransform.position, adjTransform.position, lerpFac * zoomLerpFac);
			if(!firstPerson) {
				if(1 - zoomLerpFac > 0.05f) {   // only bother with smooth rotation if the zoomLerpFac is still big
					Quaternion iRot = camTransform.rotation;
					camTransform.LookAt(lookAt.position + lookOffset);
					Quaternion newRot = camTransform.rotation;
					camTransform.rotation = Quaternion.Slerp(iRot, newRot, zoomLerpFac);
				} else {
					camTransform.LookAt(lookAt.position + lookOffset);
				}
			}
		}
	}

	private void Reset() {
		lookAt = FindObjectOfType<Controllable>().transform;
		adjTransform = GameObject.Find("CamPos Adjusted").transform;
		camTransform = FindObjectOfType<MainCamera>().transform;
	}

	void SetCam(float distance) {
		dir.z = -distance;
		rot = Quaternion.Euler(currentY, currentX, 0);

		// move adjusted camera position
		adjTransform.position = (lookAt.position + lookOffset) + rot * dir;

		// move desired camera position
		Vector3 dDir = new Vector3(0, 0, -idistance);
		transform.position = (lookAt.position + lookOffset) + rot * dDir;

		// first-person camera rotation
		if(firstPerson) camTransform.rotation = Quaternion.Lerp(camTransform.rotation, rot, 0.4f);

	}

	// meant to be called for several frames in a row as "idistance" is lerped towards 0
	public void SetFirstPerson(bool value) {
		if(value) {
			firstPerson = true;
			lerpFac = Mathf.Lerp(lerpFac, 1, 0.05f);
		} else {
			firstPerson = false;
			lerpFac = iLerpFac;
		}
	}

	public void ZoomIn(Interactable i) {
		zoomIn = true;
		// find midpoint between player and interaction
		// (interaction = 90, player = 270) try moving out at -45 (via raycast). If a wall is hit, save the distance we were able to move out and try left. Else use this position
		// try moving out at 225. If a wall is hit, compare the distance with the other try and use whichever position had the bigger distance. Else, move left a bit (so the player and interaction
		// are on the right side of the screen
		// set zoomRotation to face the midpoint of the interaction

		// or if you don't wanna die you could just set each interaction's zoom transform manually and use that
		zoomTransform = i.zoomTransform;
		zoomLerpFac = iZoomLerpFac;		// this is for smooth rotation back to where we were before the interaction
	}

	public void ZoomOut() {
		zoomIn = false;
	}
}
