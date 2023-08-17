using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrabHitbox : MonoBehaviour
{
    public List<GameObject> foodList;
    private bool isHolding;

    private GameObject foodHeld;

    // Parameters for the projectile force
    // Soon will be changed when tool for projectile is done
    [SerializeField] private float forceAmount = 10f;

    private void Start()
    {
        foodList = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Food" && !isHolding)
        {
            foodList.Add(other.gameObject);
            Debug.Log(other + "was added to the list");
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Food")
        {
            foodList.Remove(other.gameObject);
            Debug.Log(other + "was removed from the list");
        }
    }
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isHolding)
            {
                LetGo();
            }
            else
            {
                Grab();
            }
        }
    }

    private void Grab()
    {
            if (foodList.Count > 0)
            {
                isHolding = true;
                Debug.Log("Gonna grab the" + foodList[0]);
                foodHeld = foodList[0];
                if (foodHeld != null)
                {
                    foodHeld.transform.SetParent(this.transform);
                    foodHeld.transform.position = (this.transform.position);
                    foodHeld.GetComponent<Rigidbody>().isKinematic = true;
                    foodHeld.GetComponent<SphereCollider>().enabled = false;
                }
            }
            // What this doe
            //
            //
    }

    private void LetGo()
    {
            Debug.Log("Gonna throw the" + foodHeld);

            isHolding = false;
            foodList.Clear();
            if (foodHeld != null)
            {
                foodHeld.transform.SetParent(null);
                foodHeld.GetComponent<SphereCollider>().enabled = true;
                foodHeld.GetComponent<Rigidbody>().isKinematic = false;
                foodHeld.GetComponent<Rigidbody>().AddForce(transform.forward * forceAmount, ForceMode.Impulse);
                foodHeld = null;
                foodList.Remove(foodHeld);
            }
        }

}