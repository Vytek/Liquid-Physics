using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Liquid;
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

		serializedObject.Update();

		EditorGUILayout.PropertyField(liquidParticlePrefab);
		EditorGUILayout.PropertyField(emissionRate);
		EditorGUILayout.PropertyField(emissionRandomForce);

		EditorGUILayout.PropertyField(automatic);
		GUI.enabled = automatic.boolValue;


		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.LabelField("Emission");

		manager = FindObjectOfType<Manager>();
		IList[] lists = new IList[] {
			manager.bases,
			manager.mixtures
		};
		Type[] typeOptions = (
			from x in lists
			select x[0].GetType()
		).ToArray();
		string[] typeOptionsNames = (
			from x in typeOptions
			select x.Name
		).ToArray();

		int typeSelected = 0;
		if(self.emission) {
			for(int i = 0; i < typeOptions.Length; i++) {
				if(typeOptions[i].Equals(self.emission.GetType())) {
					typeSelected = i;
					break;
				}
			}
		}
		typeSelected = EditorGUILayout.Popup(typeSelected, typeOptionsNames);

		var type = typeof(List<>).MakeGenericType(typeOptions[typeSelected]);
		var context = Activator.CreateInstance(type);

		Array emissionOptions = Array.CreateInstance(typeOptions[typeSelected], lists[typeSelected].Count);


		/*
		Type t = typeOptions[typeSelected];

		Array emissionOptions = Array.CreateInstance(t, lists[typeSelected].Count);
		//emissionOptions = lists[typeSelected].Cast<t>().ToArray();

		for(int i = 0; i < emissionOptions.Length; i++) {
			emissionOptions.SetValue(lists[typeSelected][0], i);
		}

		Debug.Log("\n\n\n\n");
		foreach(object o in emissionOptions) {
			Debug.Log(o.GetType());
		}
		*/



		EditorGUILayout.EndHorizontal();

		/*
		Mixture[] options = manager.mixtures.ToArray();
		int selected;
		try {
			selected = manager.mixtures.IndexOf(self.emission);
		}
		catch {
			selected = 0;
		}
		string[] optionsNames = (
			from x in options
			select x.name
		).ToArray();
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Mixture to emit");
		selected = EditorGUILayout.Popup(selected, optionsNames);
		self.mixture = options[selected];
		EditorGUILayout.EndHorizontal();
		*/

		EditorGUILayout.PropertyField(stopped);

		GUI.enabled = true;

		serializedObject.ApplyModifiedProperties();
	}
}
