using UnityEngine;
using System.Collections;

public class PlatformController : RaycastController {


	public Vector3 move;
	
	public override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 velocity = move * Time.deltaTime;
		transform.Translate (velocity);
	}
}
