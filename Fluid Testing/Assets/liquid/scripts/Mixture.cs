using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
		// This is probably water in most cases
		public static Mixture DefaultMixture() {
			Manager manager = FindObjectOfType<Manager>();
			return Mixture.MixtureFromBase(manager.bases[0]);
		}

		// Returns a mixture made entirely of one Base
		// Useful for mixing a Base into a Mixture
		public static Mixture MixtureFromBase(Base o) {
			Mixture mixture = CreateInstance<Mixture>();
			//mixture.bases.Add(o);
			//mixture.parts.Add(1);
			mixture.components.Add(o, 1);
			return mixture;
		}

		public static Mixture Mix(Mixture a, float amountA, Mixture b, float amountB) {

			float totalAmount = amountA + amountB;

			float totalPartsA = 0, totalPartsB = 0;
			foreach(float o in a.components.Values) {
				totalPartsA += o;
			}
			foreach(float o in b.components.Values) {
				totalPartsB += o;
			}

			Base[] bases = (
				from x in a.components.Keys.Union(b.components.Keys)
				select x
			).Distinct().ToArray();

			Mixture mixture = CreateInstance<Mixture>();
			foreach(Base o in bases) {
				/*
				float parts = 0;
				parts += a.components.ContainsKey(o) ? (a.components[o] / totalPartsA) * totalAmount: 0;
				parts += b.components.ContainsKey(o) ? (b.components[o] / totalPartsB) * totalAmount: 0;
				mixture.components.Add(o, parts);
				*/
				float percentOfA = a.components.ContainsKey(o) ? a.components[o] / totalPartsA : 0;
				float percentOfB = b.components.ContainsKey(o) ? b.components[o] / totalPartsB : 0;
				float absoluteA = percentOfA * amountA;
				float absoluteB = percentOfB * amountB;
				float absoluteTotal = (absoluteA + absoluteB) / totalAmount;
				mixture.components.Add(o, absoluteTotal);
			}

			mixture.updateColor();

			return mixture;
		}
	}
}
