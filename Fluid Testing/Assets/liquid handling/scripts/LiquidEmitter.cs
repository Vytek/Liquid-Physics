using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidEmitter : MonoBehaviour {

	// Public
	public LiquidVolume origin;
	public float emissionRate;
	public float emissionRandomForce;
	public GameObject liquidParticlePrefab;
	public bool stopped;

	// Private
	private LiquidParticle previousParticle;

	private void Awake() {

		StartCoroutine(emit());
	}

	private IEnumerator emit() {

		if(!stopped) {
			GameObject newParticle = Instantiate(liquidParticlePrefab, transform.position, Quaternion.identity);
			newParticle.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(emissionRandomForce * Random.Range(-1f, 1f), 0, emissionRandomForce) * Random.Range(-1f, 1f));
			newParticle.GetComponent<LiquidParticle>().origin = this;
			newParticle.name = "liquid particle (" + name + ")";
			if(previousParticle) {
				newParticle.GetComponent<LiquidParticle>().neighbor = previousParticle;
			}
			previousParticle = newParticle.GetComponent<LiquidParticle>();
			if(origin) {
				origin.fullness -= .02f;
			}
		}

		yield return new WaitForSeconds(1 / emissionRate);
		StartCoroutine(emit());
	}
}
