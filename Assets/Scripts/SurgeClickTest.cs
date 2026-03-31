using UnityEngine;

public class SurgeClickTest : MonoBehaviour
{
    public SurpriseSurge surgeManager;

    void OnMouseDown()
    {
        if (surgeManager != null)
        {
            Debug.Log("Cube clicked with mouse");
            surgeManager.ActivateSurge();
        }
    }
}