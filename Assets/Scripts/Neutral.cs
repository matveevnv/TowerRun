using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neutral : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hit " + other.gameObject.name);

        if (other.gameObject.name == "Player")
        {
            Debug.Log("neutral hit " );
            Destroy(this.gameObject);
        }
    }
}
