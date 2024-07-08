using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NewProjectileBehaviour))]
public class NewProjectileBehaviourEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NewProjectileBehaviour projectile = (NewProjectileBehaviour)target;

        DrawDefaultInspector();
        GUILayout.Space(15);

        EditorGUILayout.LabelField("Projectile Type & Parameters", EditorStyles.boldLabel);
        projectile.projectileType = (NewProjectileBehaviour.projectileBehaviour)EditorGUILayout.EnumPopup("Projectile Type", projectile.projectileType);

        switch (projectile.projectileType)
        {
            case NewProjectileBehaviour.projectileBehaviour.straightforward:
                GUILayout.Space(5);
                EditorGUILayout.LabelField("No parameters need to be edited", EditorStyles.miniLabel);
                break;

            case NewProjectileBehaviour.projectileBehaviour.rebound:
                GUILayout.Space(5);
                projectile.maxBounces = EditorGUILayout.IntField("Max Bounces", projectile.maxBounces);
                projectile.bounceSound = (AudioClip)EditorGUILayout.ObjectField("Bounce Sound", projectile.bounceSound, typeof(AudioClip), false);
                projectile.bouncyMaterial = (PhysicMaterial)EditorGUILayout.ObjectField("Bouncy Physics Material", projectile.bouncyMaterial, typeof(PhysicMaterial), false);
                //projectile.mushroomObj = (GameObject)EditorGUILayout.ObjectField("Mushroom Transform", projectile.mushroomObj, typeof(GameObject), false);
                break;

            case NewProjectileBehaviour.projectileBehaviour.homing:
                GUILayout.Space(5);
                projectile.homingSpeed = EditorGUILayout.FloatField("Homing Speed:", projectile.homingSpeed);
                break;

            case NewProjectileBehaviour.projectileBehaviour.splash:
                GUILayout.Space(5);
                projectile.splashRadius = EditorGUILayout.FloatField("Splash Radius", projectile.splashRadius);
                break;
        }
    }
}
