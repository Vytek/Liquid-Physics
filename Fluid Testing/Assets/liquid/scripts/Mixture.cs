using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace LiquidHandling {
	public class Mixture : Liquid {

		// Public
		public Dictionary<Base, float> components;

		// For LiquidManagerEditor
		public int dropDownSelection = 0;
		public List<Base> bases = new List<Base>();
		public List<float> parts = new List<float>();

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

		public override Mixture ToMixture() {
			return this;
		}

		public void updateColor() {

			// Total up the parts
			float partsTotal = 0;
			foreach(float o in components.Values) {
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
		// This is probably water, unlesss somebody changes that
		public static Mixture DefaultMixture() {
			Manager manager = FindObjectOfType<Manager>();
			return manager.bases[0].ToMixture();
		}

		public static Mixture Mix(Mixture a, float amountA, Mixture b, float amountB) {

			// Get total amount between a and b
			float totalAmount = amountA + amountB;
			float totalPartsA = 0, totalPartsB = 0;
			foreach(float o in a.components.Values) {
				totalPartsA += o;
			}
			foreach(float o in b.components.Values) {
				totalPartsB += o;
			}

			// Get all bases
			Base[] bases = (
				from x in a.components.Keys.Union(b.components.Keys)
				select x
			).Distinct().ToArray();

			// Mix
			Mixture mixture = CreateInstance<Mixture>();
			foreach(Base o in bases) {
				float percentOfA = a.components.ContainsKey(o) ? a.components[o] / totalPartsA : 0;
				float percentOfB = b.components.ContainsKey(o) ? b.components[o] / totalPartsB : 0;
				float absoluteA = percentOfA * amountA;
				float absoluteB = percentOfB * amountB;
				float absoluteTotal = (absoluteA + absoluteB) / totalAmount;
				mixture.components.Add(o, absoluteTotal);
			}

			// Update color
			mixture.updateColor();

			// Return the new mixture
			return mixture;
		}
	}
}
