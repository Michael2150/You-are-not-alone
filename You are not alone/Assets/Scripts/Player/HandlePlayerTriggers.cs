using System;
using System.Collections;
using System.Collections.Generic;
using GameGlobals;
using UnityEngine;

public class HandlePlayerTriggers : MonoBehaviour
{
    private PlayerHideScript playerHideScript;
    private LayerMask hideableLayer;
    
    private void Start()
    {
        hideableLayer = LayerMask.GetMask("Hideable");
        playerHideScript = GetComponent<PlayerHideScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isInLayerMask(other.gameObject.layer, hideableLayer))
            playerHideScript.AddHidingSpot(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (isInLayerMask(other.gameObject.layer, hideableLayer))
            playerHideScript.RemoveHidingSpot(other.gameObject);
    }
    
    private bool isInLayerMask(int layer, LayerMask layerMask)
    {
        return layerMask == (layerMask | (1 << layer));
    }
}
