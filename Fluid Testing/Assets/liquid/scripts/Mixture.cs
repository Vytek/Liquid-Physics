using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiquidHandling {
	public class Mixture : Liquid {

		// Public
		public Dictionary<Base, float> components;

		// For LiquidManagerEditor
		public int dropDownSelection = 0;
		public List<Base> bases = new List<Base>();
		public List<int> parts = new List<int>();

		private void Awake() {

			// Create dictionary from LiquidManagerEditor lists
			/*
			 * Note: This is done because editing of a dictionary in the
			 * inspector results in unwanted resetting of said dictionary.
			 */
			components = new Dictionary<Base, float>();
			for(int i = 0; i < bases.Count; i++) {
				components.Add(bases[i], parts[i]);
			}

			// Extra check to update color
			updateColor();
		}

		public void updateColor() {

			// Total up the parts
			int partsTotal = 0;
			foreach(int o in components.Values) {
				partsTotal += o;
			}

			// Add rgb values from each component
			float r = 0, g = 0, b = 0;
			foreach(KeyValuePair<Base, float> o in components) {
				Color componentColor = o.Key.color;
				float componentPercentage = o.Value / partsTotal;
				r += componentColor.r * componentPercentage;
				g += componentColor.g * componentPercentage;
				b += componentColor.b * componentPercentage;
			}
			color = new Color(r, g, b);
		}

		// Returns a mixture made only of the first Base in the liquid manager
		// This is probably water in most cases
		public static Mixture DefaultMixture() {
			Manager manager = FindObjectOfType<Manager>();
			Mixture defaultMixture = ScriptableObject.CreateInstance<Mixture>();
			defaultMixture.bases.Add(manager.bases[0]);
			defaultMixture.parts.Add(1);
			return defaultMixture;
		}
	}
}
