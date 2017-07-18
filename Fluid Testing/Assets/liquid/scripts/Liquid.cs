using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiquidHandling {
	public abstract class Liquid : ScriptableObject {

		// Public
		public Color color;

		// Return either a Mixture regardless of whether it is a Base or Mixture
		public abstract Mixture ToMixture();
	}
}
