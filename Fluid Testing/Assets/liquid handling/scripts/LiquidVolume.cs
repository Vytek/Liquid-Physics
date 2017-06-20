﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parabox.CSG;

public class LiquidVolume : MonoBehaviour {

	// Public
	public Mesh originalMesh;
	public Material material; //TODO: recieve material from liquid type
	public float yBottom, yTop, widthBottom, widthTop, cubeScale, minFullness, maxVolume;
	[Range(0, 1)]
	public float fullness;
	public LiquidEmitter liquidEmitterPrefab;

	// Private
	private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;
	private MeshCollider meshCollider;
	private LiquidEmitter emitter;
	private float volume;

	private void Awake() {

		// Set up mesh for this object
		gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>().material = material;
		gameObject.AddComponent<MeshCollider>().convex = true;
		GetComponent<MeshCollider>().isTrigger = true;

		// Set initial volume according to fullnesss
		volume = maxVolume * fullness;
		updateVolumeMesh();
	}

	private void updateVolumeMesh() {

		// Don't show if empty
		GetComponent<MeshRenderer>().enabled = fullness > minFullness;

		// Reset rotation before beginning
		transform.localRotation = Quaternion.identity;

		// Check whether container is inverted (rotated more than 90deg)
		bool inverted = (transform.parent.transform.rotation.eulerAngles.x > 90 && transform.parent.transform.rotation.eulerAngles.x < 360 - 90)
		|| (transform.parent.transform.rotation.eulerAngles.z > 90 && transform.parent.transform.rotation.eulerAngles.z < 360 - 90);

		// Find the center of the bottom and top faces of this cylindrical volume
		Vector3 bottomCenter = Vector3.up;
		Vector3 topCenter = Vector3.up;
		if(inverted) {
			bottomCenter *= yTop;
			topCenter *= yBottom;
		}
		else {
			bottomCenter *= yBottom;
			topCenter *= yTop;
		}

		// Find exact bottom and top based on the container's rotation
		Vector3 bottom = bottomCenter;
		Vector3 top = topCenter;
		Vector3 spillPoint = topCenter;
		int iterations = 32; //more iterations = more accurate positioning
		for(int i = 0; i < iterations + 1; i++) {
			Quaternion adjustmentRotation = Quaternion.AngleAxis(i * (360 / iterations), Vector3.up);
			Vector3 adjustedBottom = transform.parent.transform.rotation * (bottomCenter + adjustmentRotation * (Vector3.forward * widthBottom));
			Vector3 adjustedTop = transform.parent.transform.rotation * (topCenter + adjustmentRotation * (Vector3.forward * widthTop));
			if(bottom == bottomCenter || adjustedBottom.y <= bottom.y) {
				bottom = adjustedBottom;
				if(!inverted) {
					spillPoint = adjustedTop;
				}
				else {
					spillPoint = adjustedBottom;
				}
			}
			if(top == topCenter || adjustedTop.y >= top.y) {
				top = adjustedTop;
			}
		}

		// Place copy of original volume shape
		GameObject original = new GameObject();
		original.AddComponent<MeshFilter>().sharedMesh = originalMesh;
		original.transform.rotation = transform.rotation;

		// Place cube
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.GetComponent<BoxCollider>().enabled = false;
		cube.transform.position += new Vector3(
			bottom.x + (Mathf.Max(fullness, minFullness) * (top.x - bottom.x)),
			bottom.y + (Mathf.Max(fullness, minFullness) * (top.y - bottom.y)),
			bottom.z + (Mathf.Max(fullness, minFullness) * (top.z - bottom.z))
		);

		// Check for spilling
		if(cube.transform.position.y >= spillPoint.y && fullness > 0) {
			if(!emitter) {
				emitter = Instantiate(liquidEmitterPrefab, spillPoint, Quaternion.identity, transform);
				emitter.name = "spill emitter";
				emitter.origin = this;
			}
			emitter.transform.localPosition = spillPoint + transform.parent.transform.rotation * Vector3.up * 0.02f;
			emitter.stopped = false;
			//TODO: modify emission rate for variable pouring speed
		}
		else {
			if(emitter) {
				emitter.stopped = true;
			}
		}

		// Adjust cube position to align bottom with desired liquid level
		cube.transform.localScale = Vector3.one * cubeScale;
		cube.transform.position += Vector3.up * (cubeScale / 2);

		// Perform boolean operation
		Mesh m = CSG.Subtract(original, cube);
		m.name = "CSG";
		GetComponent<MeshFilter>().sharedMesh = GetComponent<MeshCollider>().sharedMesh = m;
		transform.localRotation = Quaternion.Inverse(transform.parent.transform.rotation);

		// Remove original and cube, leaving the desired shape
		Destroy(original);
		Destroy(cube);
	}

	public void addLiquid(/*Liquid l*/ float amount) {
		volume = Mathf.Max(Mathf.Min(volume + amount, maxVolume), 0);
		fullness = volume / maxVolume;
		updateVolumeMesh();
	}
}
