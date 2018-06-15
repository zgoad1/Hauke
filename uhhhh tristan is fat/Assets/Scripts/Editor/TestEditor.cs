using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestClass))]
public class TestEditor : Editor {

	SerializedProperty text;
	SerializedProperty length;

	public void OnEnable() {
	}

	public override void OnInspectorGUI() {
		((TestClass)target).text = new string[EditorGUILayout.IntField(((TestClass)target).text.Length, GUILayout.Width(32f))];
		for(int i = 0; i < ((TestClass)target).text.Length; i++) {
			((TestClass)target).text[i] = EditorGUILayout.TextArea("", GUILayout.Height(40f));
		}

		//serializedObject.ApplyModifiedProperties();

		//base.OnInspectorGUI();
	}
}
