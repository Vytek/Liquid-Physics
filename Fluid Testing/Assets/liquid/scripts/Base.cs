using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiquidHandling {
	public class Base : Liquid {

		public override Mixture ToMixture() {
			Mixture mixture = CreateInstance<Mixture>();
			mixture.components.Add(this, 1);
			mixture.name = name;
			return mixture;
		}
	}
}
