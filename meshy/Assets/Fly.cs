using UnityEngine;
using System.Collections;

public class Fly : MonoBehaviour {

	private float flySpeed = 0.1f;

	// Use this for initialization
	void Start () {
	
	}



	// Update is called once per frame
	void Update () {
	
		if (Input.GetMouseButtonUp(0)){
			Screen.lockCursor = true;
			Screen.showCursor = false;
		}

		if (Input.GetKey(KeyCode.Escape)){
			Screen.showCursor = true;
		}


		if (Input.GetAxis("Vertical") != 0)
		{
			transform.Translate(Vector3.forward * flySpeed * Input.GetAxis("Vertical"));
		}

		if (Input.GetAxis("Horizontal") != 0)
		{
			transform.Translate(Vector3.right * flySpeed * Input.GetAxis("Horizontal"));
		}

		if (Input.GetKey(KeyCode.E))	
		{
			transform.Translate(Vector3.up * flySpeed);	
		}
		else if (Input.GetKey(KeyCode.Q))	
		{
			transform.Translate(Vector3.down * flySpeed);	
		}

		transform.Rotate (new Vector3 (0, Input.GetAxis("Mouse X"), 0), Space.World);
		transform.Rotate (new Vector3(-Input.GetAxis("Mouse Y"), 0, 0), Space.Self);
	}
}
