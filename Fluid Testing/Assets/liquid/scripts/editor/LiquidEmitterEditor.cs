using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LiquidHandling;
using System.Linq;
using System;

[CustomEditor(typeof(Emitter))]
public class LiquidEmitterEditor : Editor {

	Emitter self;
	Manager manager;

	SerializedProperty	origin,
						emissionRate,
						emissionRandomForce,
						liquidParticlePrefab,
						stopped,
						automatic,
						type;

	private void OnEnable() {

		self = (Emitter)target;

		origin = serializedObject.FindProperty("origin");
		emissionRate = serializedObject.FindProperty("emissionRate");
		emissionRandomForce = serializedObject.FindProperty("emissionRandomForce");
		liquidParticlePrefab = serializedObject.FindProperty("liquidParticlePrefab");
		stopped = serializedObject.FindProperty("stopped");
		automatic = serializedObject.FindProperty("automatic");
	}

	public override void OnInspectorGUI() {

		// Update inspector
		serializedObject.Update();

		// General property fields
		EditorGUILayout.PropertyField(liquidParticlePrefab);
		EditorGUILayout.PropertyField(emissionRate);
		EditorGUILayout.PropertyField(emissionRandomForce);

		// Automatic emitter properties
		EditorGUILayout.PropertyField(automatic);
		GUI.enabled = automatic.boolValue;
		emissionDropdowns();
		EditorGUILayout.PropertyField(stopped);
		GUI.enabled = true;

		// Apply inspector changes
		serializedObject.ApplyModifiedProperties();
	}

	private void emissionDropdowns() {

		// Formatting
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Emission", GUILayout.MinWidth(0));

		// Get lists (bases and mixtures) from the manager
		manager = FindObjectOfType<Manager>();
		IList[] lists = new IList[] {
			manager.bases,
			manager.mixtures
		};

		// Get options for the type dropdown
		Type[] typeOptions = (
			from x in lists
			select x[0].GetType()
		).ToArray();
		string[] typeOptionsNames = (
			from x in typeOptions
			select x.Name
		).ToArray();

		// Get which type is already selected
		int typeSelected = 0;
		if(self.emission) {
			for(int i = 0; i < typeOptions.Length; i++) {
				if(typeOptions[i].Equals(self.emission.GetType())) {
					typeSelected = i;
					break;
				}
			}
		}

		// Type dropdown
		typeSelected = EditorGUILayout.Popup(typeSelected, typeOptionsNames);
		Type type = typeOptions[typeSelected];

		// Get options for emission dropdown
		Liquid[] emissionOptions = lists[typeSelected].Cast<Liquid>().ToArray();
		string[] emissionOptionsNames = (
			from x in emissionOptions
			select x.name
		).ToArray();

		// Get which emission is already selected
		int emissionSelected = 0;
		if(self.emission) {
			for(int i = 0; i < emissionOptions.Length; i++) {
				if(emissionOptions[i].Equals(self.emission)) {
					emissionSelected = i;
					break;
				}
			}
		}

		// Emission dropdown
		emissionSelected = EditorGUILayout.Popup(emissionSelected, emissionOptionsNames);
		self.emission = emissionOptions[emissionSelected];

		// Formatting
		EditorGUILayout.EndHorizontal();
	}
}
