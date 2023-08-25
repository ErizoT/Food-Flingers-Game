using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrabHitbox : MonoBehaviour
{
    public List<GameObject> foodList;
    
    private GameObject foodHeld;
    private bool IsHolding => foodHeld != null;

    /*private bool isHolding;
    private bool IsHolding
    {
        get
        {
            return isHolding;
        }
        set
        {
            isHolding = value;
            SomeOtherFunction();
        }
    }*/

    [SerializeField] Material grabMaterial;
    [SerializeField] Material neutralMaterial;

    // Parameters for the projectile force
    // Soon will be changed when tool for projectile is done
    //[SerializeField] private float forceAmount = 10f;

    private void Start()
    {
        foodList = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Food" && !IsHolding)
        {
            foodList.Add(other.gameObject);
            other.GetComponent<MeshRenderer>().material = grabMaterial;
            //Debug.Log(other + "was added to the list");
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Food")
        {
            foodList.Remove(other.gameObject);
            other.GetComponent<MeshRenderer>().material = neutralMaterial;
            //Debug.Log(other + "was removed from the list");
        }
    }
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (IsHolding)
            {
                LetGo();
            }
            else
            {
                Grab();
            }
        }

        //IsHolding = true;
    }

    private void Grab()
    {
            if (foodList.Count > 0)
            {
                //isHolding = true;
                //Debug.Log("Gonna grab the" + foodList[0]);
                foodHeld = foodList[0];
                if (foodHeld != null && foodHeld.GetComponent<ProjectileBehaviour>().isThrown == false)
                {
                    Transform foodTransform = foodHeld.transform;

                    foodTransform.SetParent(transform);
                    foodTransform.position = transform.position;
                    foodTransform.rotation = transform.rotation;
                    foodHeld.GetComponent<Rigidbody>().isKinematic = true;
                    foodHeld.GetComponent<SphereCollider>().enabled = false;
                    foodList.Remove(foodHeld);
                }
            }
    }

    private void LetGo()
    {
            //Debug.Log("Gonna throw the" + foodHeld);
            //isHolding = false;
            foodList.Clear();
            if (foodHeld != null)
            {
                foodHeld.transform.SetParent(null);
                foodHeld.GetComponent<SphereCollider>().enabled = true;
                foodHeld.GetComponent<Rigidbody>().isKinematic = false;
                foodHeld.GetComponent<Rigidbody>().useGravity = false;

                foodHeld.GetComponent<ProjectileBehaviour>().isThrown = true;
                //foodHeld.GetComponent<Rigidbody>().AddForce(transform.forward * forceAmount, ForceMode.Impulse);
                foodHeld = null;
            }
        }

}
