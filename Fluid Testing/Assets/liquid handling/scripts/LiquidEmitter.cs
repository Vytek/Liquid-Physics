using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidEmitter : MonoBehaviour {

	// Public
	public float emissionRate;
	public float emissionRandomForce;
	public GameObject liquidParticlePrefab;

	// Private
	private LiquidParticle previousParticle;

	private void Start() {

		StartCoroutine(emit());
	}

	private IEnumerator emit() {

		GameObject newParticle = Instantiate(liquidParticlePrefab, transform.position, Quaternion.identity);
		newParticle.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(emissionRandomForce * Random.Range(-1f, 1f), 0, emissionRandomForce) * Random.Range(-1f, 1f));
		if(previousParticle) {
			newParticle.GetComponent<LiquidParticle>().neighbor = previousParticle;
		}
		previousParticle = newParticle.GetComponent<LiquidParticle>();

		yield return new WaitForSeconds(1 / emissionRate);
		StartCoroutine(emit());
	}
}
