using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Food", menuName = "Projectiles/Food")]
public class ScriptExercise : MonoBehaviour
{
    [SerializeField] public List<GameObject> FoodList;

    [SerializeField] public List<float> LikelihoodList;

    public void InitialiseLists()
    {
        if (FoodList == null)
        {
            FoodList = new List<GameObject>();
        }

        if (LikelihoodList == null)
        {
            LikelihoodList = new List<float>();
        }
    }

    public void AddFood()
    {
        FoodList.Add(null);
        LikelihoodList.Add(0);
    }
    public void RemoveFood(int index)
    {
        FoodList.RemoveAt(index);
        LikelihoodList.RemoveAt(index);
    }
}
