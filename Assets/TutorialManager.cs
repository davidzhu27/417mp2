using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject duckPopup;
    public GameObject cashPopup;
    public ResourceManager ResourceManager;

    public void ShowDuckPopup()
    {
        if (ResourceManager.ducks > 0) return; // already progressed

        duckPopup.SetActive(true);
    }

    public void HideDuckPopup()
    {
        duckPopup.SetActive(false);
    }

    public void ShowCashPopup()
    {
        if (ResourceManager.duckSellingUnlocked) return; // already progressed

        cashPopup.SetActive(true);
    }

    public void HideCashPopup()
    {
        cashPopup.SetActive(false);
    }
}