using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepOrientation : MonoBehaviour {

	void Update () {
		transform.localPosition = Vector3.zero;
		transform.rotation = Quaternion.identity;
		transform.rotation = Camera.main.transform.rotation;
	}
}
