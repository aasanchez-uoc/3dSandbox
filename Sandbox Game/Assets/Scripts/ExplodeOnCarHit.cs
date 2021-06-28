using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnCarHit : MonoBehaviour
{
    public GameObject explodePrefab;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Car")
        {
            gameObject.SetActive(false);
            Instantiate(explodePrefab, transform.position, Quaternion.LookRotation(collision.transform.position - transform.position));
        }
    }
}