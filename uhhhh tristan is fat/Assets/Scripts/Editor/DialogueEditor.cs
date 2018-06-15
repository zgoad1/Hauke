/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(NPC))]
//[CanEditMultipleObjects]
public class DialogueEditor : Editor {

	SerializedProperty faces;
	SerializedProperty text;
	NPC obj;

	private void OnEnable() {
		faces = serializedObject.FindProperty("faces");
		text = serializedObject.FindProperty("text");
		obj = (NPC)target;
	}

	public override void OnInspectorGUI() {
		serializedObject.Update();

		// dialogue array length (# of unique conversations)
		obj.dialogue = new DialogueArray[EditorGUILayout.IntField(obj.dialogue.Length, GUILayout.Width(32f))];
		for(int j = 0; j < obj.dialogue.Length; j++) {
			// text array (# of pages of this conversation)
			//Debug.Log("what: " + obj.dialogue[0]);//int pages = EditorGUILayout.IntField("Pages", obj.dialogue[j].text.Length, GUILayout.Width(32f));
			//obj.dialogue[j].text = new string[pages];//EditorGUILayout.IntField(obj.dialogue[j].text.Length, GUILayout.Width(32f))];
			//obj.dialogue[j].faces = new Sprite[pages];//EditorGUILayout.IntField(obj.dialogue[j].faces.Length, GUILayout.Width(32f))];
			for(int i = 0; i < obj.dialogue[j].text.Length; i++) {
				obj.dialogue[j].text[i] = EditorGUILayout.TextArea(obj.dialogue[j].text[i], GUILayout.Height(40f));
				//obj.dialogue[j].faces[i] = EditorGUILayout.ObjectField(obj.dialogue[j].faces[i]);
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
*/