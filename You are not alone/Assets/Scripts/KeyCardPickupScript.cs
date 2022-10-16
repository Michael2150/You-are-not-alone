using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Pickup script for the collectable key shards
public class KeyCardPickupScript : MonoBehaviour{    
    public AudioClip pickupSound;
    public GameObject pickupEffect;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        //Play pickup sound
        AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        //Play pickup effect
        //Instantiate(pickupEffect, transform.position, transform.rotation);
        //Destroy the key shard
        Destroy(gameObject);
    }
}
