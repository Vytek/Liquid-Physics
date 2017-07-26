using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiquidHandling {
	[AddComponentMenu("Liquid/Container")]
	public class Container : MonoBehaviour {

		// Public
		[HideInInspector]
		public Volume volume;

		private void Awake() {

			// Find the volume
			volume = GetComponentInChildren<Volume>();
		}
	}
}
