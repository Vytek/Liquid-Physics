using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiquidHandling {
	[AddComponentMenu("Liquid/Manager")]
	public class Manager : MonoBehaviour {

		// Public
		public List<Base> bases = new List<Base>();
		public List<Mixture> mixtures = new List<Mixture>();
	}
}
