using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parabox.CSG;

public class LiquidVolume : MonoBehaviour {

	// Public
	public Mesh originalMesh;
	public Material material; //TODO: recieve material from liquid type
	public float yBottom, yTop, widthBottom, widthTop, cubeScale, minFullness;
	[Range(0, 1)]
	public float fullness;

	// Private
	private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;
	private MeshCollider meshCollider;

	private void Awake() {

		// Set up mesh for this object
		gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>().material = material;
		gameObject.AddComponent<MeshCollider>().convex = true;
		GetComponent<MeshCollider>().isTrigger = true;
	}

	private void FixedUpdate() {

		// Don't show if empty
		GetComponent<MeshRenderer>().enabled = fullness > minFullness;

		// Reset rotation before beginning
		transform.localRotation = Quaternion.identity;

		// Find rotated top and bottom positions

		Vector3 bottomCenter = Vector3.up;
		Vector3 topCenter = Vector3.up;

		if(	(transform.parent.transform.rotation.eulerAngles.x > 90 && transform.parent.transform.rotation.eulerAngles.x < 360 - 90)
		||	(transform.parent.transform.rotation.eulerAngles.z > 90 && transform.parent.transform.rotation.eulerAngles.z < 360 - 90)) {
			bottomCenter *= yTop;
			topCenter *= yBottom;
		}
		else {
			bottomCenter *= yBottom;
			topCenter *= yTop;
		}

		Vector3 top = topCenter;
		Vector3 bottom = bottomCenter;

		int iterations = 32;
		for(int i = 0; i < iterations + 1; i++) {
			Quaternion adjustmentRotation = Quaternion.AngleAxis(i * (360 / iterations), Vector3.up);
			Vector3 adjustedBottom = transform.parent.transform.rotation * (bottomCenter + adjustmentRotation * (Vector3.forward * widthBottom));
			Vector3 adjustedTop = transform.parent.transform.rotation * (topCenter + adjustmentRotation * (Vector3.forward * widthTop));
			if(bottom == bottomCenter || adjustedBottom.y <= bottom.y) {
				bottom = adjustedBottom;
			}
			if(top == topCenter ||  adjustedTop.y >= top.y) {
				top = adjustedTop;
			}
		}

		// Place original
		GameObject original = new GameObject();
		original.AddComponent<MeshFilter>().sharedMesh = originalMesh;
		original.transform.rotation = transform.rotation;

		// Place cube
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.GetComponent<BoxCollider>().enabled = false;
		cube.transform.localScale = Vector3.one * cubeScale;
		cube.transform.position += new Vector3(
			bottom.x + (Mathf.Max(fullness, minFullness) * (top.x - bottom.x)),
			bottom.y + (Mathf.Max(fullness, minFullness) * (top.y - bottom.y)),
			bottom.z + (Mathf.Max(fullness, minFullness) * (top.z - bottom.z))
		);
		cube.transform.position += Vector3.up * (cubeScale / 2);

		// Perform boolean operation
		Mesh m = CSG.Subtract(original, cube);
		m.name = "CSG";
		GetComponent<MeshFilter>().sharedMesh = GetComponent<MeshCollider>().sharedMesh = m;
		transform.localRotation = Quaternion.Inverse(transform.parent.transform.rotation);

		// Remove original and cube
		Destroy(original);
		Destroy(cube);
	}

	public void addLiquid(/*Liquid l*/ float amount) {
		fullness = Mathf.Max(Mathf.Min(fullness + amount, 1), 0);
	}
}
