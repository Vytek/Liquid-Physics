using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Liquid;
using System.Linq;

[CustomEditor(typeof(Manager))]
public class LiquidManagerEditor : Editor {

	Manager self;

	private void OnEnable() {
		self = (Manager)target;
	}

	public override void OnInspectorGUI() {

		// Bases
		EditorGUILayout.LabelField("Bases");

		// Editors for each existing base
		for(int i = 0; i < self.bases.Count; i++) {
			EditorGUILayout.BeginHorizontal();
			self.bases[i].name = EditorGUILayout.DelayedTextField(self.bases[i].name);
			self.bases[i].color = EditorGUILayout.ColorField(self.bases[i].color, GUILayout.Width(40));
			if(GUILayout.Button("x", GUILayout.Width(20))) {
				self.bases.RemoveAt(i);
			}
			EditorGUILayout.EndHorizontal();
		}

		// BUtton to add a new base
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("", GUILayout.ExpandWidth(true)); // empty label to align
		if(GUILayout.Button("+", GUILayout.Width(20))) {
			self.bases.Add((Base)ScriptableObject.CreateInstance("Base"));
		}
		EditorGUILayout.EndHorizontal();

		// Mixtures
		EditorGUILayout.LabelField("Mixtures");

		// Editors for each existing mixture
		for(int i = 0; i < self.mixtures.Count; i++) {
			EditorGUILayout.BeginHorizontal();
			self.mixtures[i].name = EditorGUILayout.DelayedTextField(self.mixtures[i].name);
			if(GUILayout.Button("x", GUILayout.Width(20))) {
				self.mixtures.RemoveAt(i);
			}
			EditorGUILayout.EndHorizontal();

			/* 
			 * Why does this happen every time this script is edited?
			 * For some reason, the dictionaries are null again every time we edit this script...
			 * But the names of the mixtures stay, so it's ONLY the dictonary...
			 */
			if(self.mixtures[i].components == null) {
				self.mixtures[i].components = new Dictionary<Base, int>();
				Debug.Log("No dictionary detected for this mixture. Making a new one.");
			}

			// Editors for this mixture's components
			Base[] keys = self.mixtures[i].components.Keys.ToArray();
			foreach(Base o in keys) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("", GUILayout.Width(20)); // empty label to align
				self.mixtures[i].components[o] = EditorGUILayout.IntField(self.mixtures[i].components[o], GUILayout.Width(20));
				EditorGUILayout.LabelField(o.name);
				if(GUILayout.Button("x", GUILayout.Width(20))) {
					self.mixtures[i].components.Remove(o);
				}
				EditorGUILayout.LabelField("", GUILayout.Width(20)); // empty label to align
				EditorGUILayout.EndHorizontal();
			}

			// Button to add new component
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("", GUILayout.Width(20)); // empty label to align
			Base[] options = (
				from x in self.bases
				where !self.mixtures[i].components.ContainsKey(x)
				select x
			).ToArray();
			string[] optionsNames = (
				from x in options
				select x.name
			).ToArray();
			if(options.Length > 0) {
				self.mixtures[i].dropDownSelection = EditorGUILayout.Popup(self.mixtures[i].dropDownSelection, optionsNames);
				if(GUILayout.Button("+", GUILayout.Width(20))) {
					self.mixtures[i].components.Add(options[self.mixtures[i].dropDownSelection], 0);
					self.mixtures[i].dropDownSelection = 0;
				}
			}
			else {
				EditorGUILayout.LabelField("This mixture contains every base.");
			}
			EditorGUILayout.LabelField("", GUILayout.Width(20)); // empty label to align
			EditorGUILayout.EndHorizontal();
		}

		// BUtton to add a new mixture
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("", GUILayout.ExpandWidth(true)); // empty label to align
		if(GUILayout.Button("+", GUILayout.Width(20))) {
			self.mixtures.Add((Mixture)ScriptableObject.CreateInstance("Mixture"));
		}
		EditorGUILayout.EndHorizontal();
	}
}
