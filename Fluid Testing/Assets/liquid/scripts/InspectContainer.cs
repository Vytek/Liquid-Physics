using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LiquidHandling;

public class InspectContainer : MonoBehaviour {

	// Public
	public Text text;
	public float	maxPatience,
					refreshInterval;

	// Private
	private string defaultString;
	private Mixture order;
	private List<Container> containersToInspect = new List<Container>();
	private float patience;

	private void Awake() {

		// Grab the default string
		defaultString = text.text;

		// Start ordering
		StartCoroutine(changeOrder());
	}

	// Detect Containers that enter
	private void OnTriggerEnter(Collider c) {
		Container container = c.GetComponent<Container>();
		if(container && !containersToInspect.Contains(container)) {
			containersToInspect.Add(container);
			updateDisplay();
		}
	}

	// Forget about Containers that leave
	private void OnTriggerExit(Collider c) {
		Container container = c.GetComponent<Container>();
		if(container && containersToInspect.Contains(container)) {
			containersToInspect.Remove(container);
			updateDisplay();
		}
	}

	private void updateDisplay() {

		string s = "";

		// Text for no Containers
		if(containersToInspect.Count == 0) {
			s = "I want " + order.name + ".";
		}

		// Text for too many Containers
		else if(containersToInspect.Count > 1) {
			s = "One at a time, please.";
		}

		// Text for Container analysis
		else {

			// Find best match
			Container c = containersToInspect[0];
			Liquid bestMatch = Manager.BestMatch(c.volume.liquid);
			float score = Manager.Compare(c.volume.liquid, order);

			// Good job
			if(bestMatch == order) {
				s = string.Format(
					"Thanks for the {0}!\nQuality: {1:0.}%\nQuantity: {2:0.}%",
					order.name,
					score * 100,
					c.volume.fullness * 100
				);
			}

			// Bad job
			else {
				s = string.Format(
					"I wanted {0}, not {1}!\nError: {2:0.}%\nQuantity: {3:0.}%",
					order.name,
					bestMatch.name,
					(1 - score) * 100,
					c.volume.fullness * 100
				);
			}
		}

		// Update text
		s += "\nPatience: " + (int)((patience / maxPatience) * 100) + "%";
		text.text = s;
	}

	private IEnumerator changeOrder() {

		// Set order to a random mixture
		order = Manager.Mixtures[Random.Range(0, Manager.Mixtures.Count)];

		// Check repeatedly until impatient
		patience += maxPatience;
		while(patience > 0) {
			updateDisplay();
			yield return new WaitForSeconds(refreshInterval);
			patience -= refreshInterval;
		}

		// Change the order
		StartCoroutine(changeOrder());
	}
}
