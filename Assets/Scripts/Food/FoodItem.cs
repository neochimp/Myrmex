using UnityEngine;

public class FoodItem : MonoBehaviour
{   
    public float DistanceToTarget(Transform target)
    {
        Transform currentPosition = gameObject.GetComponent<Transform>(); 

        return (target.position - currentPosition.position).sqrMagnitude; 
    }

    public Transform foodLocation()
    {   
        Debug.Log("I am " + gameObject.name + "My location is " + gameObject.transform.position);
        var mesh = GetComponentInChildren<MeshRenderer>();
        Debug.Log($"{name} | root={transform.position} | mesh={(mesh ? mesh.transform.position : Vector3.positiveInfinity)}");


        return gameObject.transform; 
    }
}
