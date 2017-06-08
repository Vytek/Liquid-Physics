using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parabox.CSG;

public class LiquidVolume : MonoBehaviour {

	// Public
	public Mesh originalMesh;
	public Material material; //TODO: recieve material from liquid type
	public float yBottom, yTop, fullness, cubeScale;

	// Private
	private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;
	private MeshCollider meshCollider;

	private void Awake() {

		// Set up mesh for this object
		gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>().material = material;
		gameObject.AddComponent<MeshCollider>().convex = true;
	}

	private void FixedUpdate() {

		transform.localRotation = Quaternion.identity;

		// Place original
		GameObject original = new GameObject();
		original.AddComponent<MeshFilter>().sharedMesh = originalMesh;
		original.transform.rotation = transform.rotation;

		// Place cube
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.localScale = Vector3.one * cubeScale;
		//TODO: FIX: The following only respects y, which is obviously wrong
		cube.transform.position = Vector3.up * (yBottom + (cubeScale / 2) + (fullness * (yTop - yBottom)));

		// Perform boolean operation
		Mesh m = CSG.Subtract(original, cube);
		m.name = "CSG";
		GetComponent<MeshFilter>().sharedMesh = GetComponent<MeshCollider>().sharedMesh = m;
		transform.localRotation = Quaternion.Inverse(transform.parent.transform.rotation);

		// Remove original and cube
		Destroy(original);
		Destroy(cube);
	}
}
