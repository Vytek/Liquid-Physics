﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace LiquidHandling {
	[AddComponentMenu("Liquid Handling/Manager")]
	public class Manager : MonoBehaviour {

		// Static
		public static List<Base> Bases = new List<Base>();
		public static List<Mixture> Mixtures = new List<Mixture>();

		// Public
		public List<Base> bases = new List<Base>();
		public List<Mixture> mixtures = new List<Mixture>();

		void Awake() {

			// Assign static lists from one manager's public variables
			Bases = bases;
			Mixtures = mixtures;
		}

		// Return which mixture most resembles a liquid
		public static Liquid BestMatch(Liquid a) {

			// Combine Mixtures and Bases into one list
			List<Liquid> candidates = new List<Liquid>();
			candidates.AddRange(Mixtures.ToArray());
			candidates.AddRange(Bases.ToArray());

			// Select which liquid is the closest match
			Liquid bestMatch = null;
			float bestScore = 0;
			foreach(Liquid o in candidates) {
				float score = Compare(a.ToMixture(), o.ToMixture());
				if(score > bestScore) {
					bestMatch = o;
					bestScore = score;
				}
			}

			// Friendly debug message
			/*
			Debug.Log(string.Format(
				"{0:0.}%\t'{1}' best match is '{2}'\n",
				bestScore * 100,
				a.name,
				bestMatch ? bestMatch.name : "null"
			));
			Debug.Log("\n\n");
			*/

			// Return the best match
			return bestMatch;
		}

		// Return a float representing how similar two liquids are
		public static float Compare(Mixture a, Mixture b) {

			// Get all bases
			
			Base[] bases = (
				from x in a.components.Keys.Union(b.components.Keys)
				select x
			).Distinct().ToArray();

			// Get total parts in a and b
			float totalPartsA = 0, totalPartsB = 0;
			foreach(float o in a.components.Values) {
				totalPartsA += o;
			}
			foreach(float o in b.components.Values) {
				totalPartsB += o;
			}

			// Calculate score
			/*
			string debugString = "";
			*/
			float score = 1;
			foreach(Base o in bases) {
				float percentOfA = a.components.ContainsKey(o) ? a.components[o] / totalPartsA : 0;
				float percentOfB = b.components.ContainsKey(o) ? b.components[o] / totalPartsB : 0;
				score -= Mathf.Abs(percentOfA - percentOfB) / 2;
				/*
				debugString += string.Format(
					"\n{0}: {1:0.}% - {2:0.}% ---> -{3:0.}",
					o.name,
					percentOfA * 100,
					percentOfB * 100,
					Mathf.Abs(percentOfA - percentOfB) / 2
				);
				*/
			}

			// Friendly debug message
			/*
			if(score > -100.0f) {
				Debug.Log(string.Format(
					"{0:0.}%\t'{1}' compared to '{2}'\n{3}\n\n\n\n\n\n",
					score * 100,
					a.name,
					b.name,
					debugString
				));
			}
			*/

			// Return score
			return score;
		}
	}
}
