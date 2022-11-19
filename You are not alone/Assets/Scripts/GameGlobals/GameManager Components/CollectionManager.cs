using System;
using UnityEngine;

namespace GameGlobals.GameManager_Components
{
    [Serializable]
    public class CollectionManager
    {
        public int CardsToCollect = 0;
        public int CardsCollected = 0;
        
        /*
         * This method is called when a card is collected.
         * Calls: private void OnCardCollected(int cardsCollected, int cardsLeftToCollect)
         */
        public event Action<int, int> OnCardCollected;
        
        public bool IsCollectionComplete => CardsCollected >= CardsToCollect;

        public void ResetCollection()
        {
            CardsCollected = 0;
        }
        
        public void AddCard()
        {
            CardsCollected++;
            OnCardCollected?.Invoke(CardsCollected, CardsToCollect-CardsCollected);
            
            if (IsCollectionComplete)
            {
                GameManager.Instance.OnCollectionComplete();
            }
        }

        public void GetAllKeyShardsInScene()
        {
            //TODO: Get all key shards in scene and add them to the collection
            var keyShards = GameObject.FindGameObjectsWithTag("KeyShard");
            CardsToCollect = keyShards.Length;

            //TODO: REMOVE THIS LINE
            CardsToCollect = 10;
        }
    }
}