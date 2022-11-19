using System.Collections;
using System.Collections.Generic;
using GameGlobals;
using UnityEngine;
using UnityEngine.SceneManagement;

//Pickup script for the collectable key shards
public class KeyCardPickupScript : MonoBehaviour{    
    public AudioClip pickupSound;
    public GameObject pickupEffect;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        //Play pickup sound
        AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        GameManager.Instance.CollectionManager.AddCard();
        
        //Destroy pickup object
        Destroy(gameObject);
    }
}
