using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiquidHandling {
	[AddComponentMenu("Liquid Handling/Particle")]
	public class Particle : MonoBehaviour {

		// Public
		public Emitter origin;
		public int sides;
		public float breakThreshold,
						radius;
		public Liquid liquid;
		[HideInInspector]
		public Particle neighbor;
		[HideInInspector]
		public Vector3[] verts;

		// Private
		private Mesh mesh;

		private void Awake() {

			mesh = GetComponent<MeshFilter>().mesh;
			verts = new Vector3[sides * 2];

			// Destroy after a while
			Destroy(gameObject, 5);
		}

		private void Start() {

			// Colorize
			if(liquid) {
				GetComponent<MeshRenderer>().material.color = liquid.color;
			}
		}

		private void FixedUpdate() {

			// Reset rotation
			transform.rotation = Quaternion.identity;

			// Get corner verts
			for(int i = 0; i < sides; i++) {
				Vector3 vert = Vector3.forward * radius;
				vert = Quaternion.Euler(0, (360f / sides) * i, 0) * vert;
				verts[i] = vert;
			}

			// Connect to neighbor if there is one
			if(neighbor) {

				// Break connection if neighbor is too far
				if(Vector3.Distance(transform.position, neighbor.transform.position) > breakThreshold) {
					neighbor = null;
				}
				else {

					// Add neighbor verts
					for(int i = 0; i < sides; i++) {
						verts[sides + i] = neighbor.verts[i] + neighbor.transform.position - transform.position;
						/* 
						 * The following line is compensation for the
						 * spacing that otherwise shows between particles.
						 * 
						 * I'm pretty sure this happens because newer
						 * particles execute before their neighbors update
						 * (fall slightly down due to gravity).
						 * 
						 * It does not appear to be possible to make newer
						 * particles execute after their neighbors, and
						 * honestly it's probably not worth it, as this
						 * compensation is good enough from a distance.
						 * 
						 * Note that this is not framerate dependant
						 * because it happens in a FixedUpdate.
						 */
						verts[sides + i] += Vector3.down * 0.015f;
					}

					// Make tris
					int[] tris = new int[(sides * 2) * 3];
					for(int tri = 0; tri < tris.Length / 3; tri++) {

						// Three points per tri
						// tris[point..point+2]
						int point = tri * 3;

						// Even tris
						if(tri % 2 == 0) {
							tris[point] = tri / 2 + 8;
							tris[point + 1] = tri / 2 + 1;
							tris[point + 2] = tri / 2;
						}

						// Odd tris
						else {
							tris[point] = (tri - 1) / 2 + 8;
							tris[point + 1] = (tri - 1) / 2 + 9;
							tris[point + 2] = (tri - 1) / 2 + 1;
						}

						// Stitch last quad
						if(tri == tris.Length / 3 - 1) {
							tris[point + 1] = tris[point + 2];
							tris[point + 2] = 0;
						}
						if(tri == tris.Length / 3 - 2) {
							tris[point + 2] = tris[point + 2];
							tris[point + 1] = 0;
						}
					}

					// Apply changes
					mesh.vertices = verts;
					mesh.triangles = tris;
				}
			}

			// Draw self if no neighbor
			if(!neighbor) {
				//TODO
			}
		}

		private void OnTriggerEnter(Collider c) {

			Volume volume = c.gameObject.GetComponent<Volume>();
			if(volume && volume != origin) {
				volume.addLiquid(liquid, 1);
				gameObject.layer = LayerMask.NameToLayer("LiquidParticleUsed");
				neighbor = null;
				GetComponent<MeshRenderer>().enabled = false;
				Destroy(gameObject, 0.1f);
			}
		}
	}
}
