using UnityEngine;

public class TutorialZone : MonoBehaviour
{
    public enum ZoneType
    {
        DuckGenerator,
        CashMachine
    }

    public ZoneType zoneType;
    public TutorialManager tutorialManager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        switch (zoneType)
        {
            case ZoneType.DuckGenerator:
                tutorialManager.ShowDuckPopup();
                break;

            case ZoneType.CashMachine:
                tutorialManager.ShowCashPopup();
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        switch (zoneType)
        {
            case ZoneType.DuckGenerator:
                tutorialManager.HideDuckPopup();
                break;

            case ZoneType.CashMachine:
                tutorialManager.HideCashPopup();
                break;
        }
    }
}