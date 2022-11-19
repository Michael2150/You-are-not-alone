using UnityEngine;

public class DebugOnly : MonoBehaviour
{
    //Awake is called before Start
    void Awake()
    {
        //If the game is not in debug mode
        if (!Debug.isDebugBuild)
        {
            //Destroy this object
            Destroy(gameObject);
        }
    }
}
