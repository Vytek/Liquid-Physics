using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Liquid {
	public class Mixture : ScriptableObject {

		// Public
		public Dictionary<Base, int> components;

		// For LiquidManagerEditor
		public int dropDownSelection = 0;
		public List<Base> bases;
		public List<int> parts;
	}
}
