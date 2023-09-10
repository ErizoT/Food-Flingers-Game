using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
[CustomEditor(typeof(ScriptExercise))]
public class FoodSpawnerEditor : Editor
{
    ScriptExercise foodSpawnerProperties;

    SerializedProperty foodList;
    SerializedProperty likelihoods;

    private void OnEnable()
    {
        // Initialize the properties as SerializedProperties
        foodList = serializedObject.FindProperty("FoodList");
        likelihoods = serializedObject.FindProperty("LikelihoodList");

        foodSpawnerProperties = (ScriptExercise)target; // Referencing the target script
        foodSpawnerProperties.InitialiseLists(); // Calls the function to create said lists
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update(); // Allows the scriptable object to refresh and update when new valeus are added i think

        int foodListCount = foodSpawnerProperties.FoodList.Count; // Getting the length of the list to be able to iterate over it over time

        for (int i = 0; i < foodListCount; i++)
        {

            GUILayout.BeginHorizontal();
            GUILayout.Label("Food " + i+1);

            // This is the line of code that actually displayed the box where you can drag your GameObject into
            EditorGUILayout.ObjectField(foodList.GetArrayElementAtIndex(i), GUIContent.none);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Likelihood ");
            // Displays the property field for likelihoods
            EditorGUILayout.PropertyField(likelihoods.GetArrayElementAtIndex(i), GUIContent.none);
            GUILayout.EndHorizontal();


            if (GUILayout.Button("Remove Food"))
            {
                foodSpawnerProperties.RemoveFood(i); // Passes i into the RemoveFood function in the ScriptExercise script
            }
        }

        if (GUILayout.Button("Add Food"))
        {
            foodSpawnerProperties.AddFood(); // Calls the function inside ScriptExercise to add a food + likelihood into the list
        }

        serializedObject.ApplyModifiedProperties(); // Self explanmatory, applies all the shit
    }

}
#endif
