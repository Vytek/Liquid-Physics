using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidParticle : MonoBehaviour {

	// Public
	public int sides;
	public float	breakThreshold,
					radius;
	[HideInInspector]
	public LiquidParticle neighbor;
	[HideInInspector]
	public Vector3[] verts;

	// Private
	private Mesh mesh;

	private void Awake() {

		mesh = GetComponent<MeshFilter>().mesh;
		verts = new Vector3[sides * 2];
	}

	

	private void Update() {

		// Get corner verts
		for(int i = 0; i < sides; i++) {
			Vector3 vert = Vector3.forward * radius;
			vert = Quaternion.Euler(0, (360f / sides) * i, 0) * vert;
			//vert += transform.position;
			//Debug.DrawLine(transform.position, transform.position + vert, Color.black);
			//vert += new Vector3(transform.position.x, 0, transform.position.z);
			//vert += transform.position;
			verts[i] = vert;
		}

		if(neighbor && Vector3.Distance(transform.position, neighbor.transform.position) < breakThreshold) {

			// Assign verts to mesh
			for(int i = 0; i < sides; i ++) {
				verts[sides + i] = neighbor.verts[i] + neighbor.transform.position - transform.position;
				//verts[sides + i] -= transform.position;
				verts[sides + i] += Vector3.down * .1f;
			}
			mesh.vertices = verts;

			// Make tris
			int[] tris = new int[(sides * 2) * 3 ];
			for(int tri = 0; tri < tris.Length / 3; tri++) {

				Color debugColor;

				// Three points per tri
				// tris[point..point+2]
				int point = tri * 3;

				// Pass over the last tri
				// TODO: Don't pass over the last tri
				if(tri == tris.Length / 3 - 1) {
					break;
				}

				// Even tris
				if(tri % 2 == 0) {
					tris[point]		= tri / 2 + 8;
					tris[point + 1]	= tri / 2 + 1;
					tris[point + 2]	= tri / 2;
					debugColor = Color.white;
				}

				// Odd tris
				else {
					tris[point]		= (tri - 1) / 2 + 8;
					tris[point + 1]	= (tri - 1) / 2 + 9;
					tris[point + 2]	= (tri - 1) / 2 + 1;
					debugColor = Color.red;
				}

				// Debug
				//Debug.DrawLine(	verts[tris[point]],		verts[tris[point + 1]], debugColor);
				//Debug.DrawLine(	verts[tris[point]],		verts[tris[point + 2]], debugColor);
				//Debug.DrawLine(	verts[tris[point + 1]],	verts[tris[point + 2]], debugColor);
			}
			mesh.triangles = tris;
		}
	}
}
