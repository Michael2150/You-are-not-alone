using GameGlobals;
using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerUIScript : MonoBehaviour
    {
        public Animator cardCollectionAnimation;
        public TMP_Text cardCollectionText;
        private static readonly int ShowKeys = Animator.StringToHash("ShowKeys");

        private void Start()
        {
            GameManager.Instance.CollectionManager.OnCardCollected += OnCardCollected;
            
            //Get the canvas 
            Canvas canvas = GetComponent<Canvas>();
            //Set the camera to the main camera
            canvas.worldCamera = Camera.main;
        }

        private void OnCardCollected(int cardsCollected, int cardsLeftToCollect)
        {
            //Change the text on the UI to show the number of cards collected and the number of cards left to collect
            cardCollectionText.text = cardsLeftToCollect.ToString();
            
            //Play the animation
            cardCollectionAnimation.SetTrigger(ShowKeys);
        }
        
        private void OnDestroy()
        {
            GameManager.Instance.CollectionManager.OnCardCollected -= OnCardCollected;
        }
    }
}
