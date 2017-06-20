﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Liquid {
	public class Mixture : ScriptableObject {

		// Public
		public Dictionary<Base, int> components;

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
			for(int i = 0; i < bases.Count; i++) {
				components.Add(bases[i], parts[i]);
			}
		}
	}
}
