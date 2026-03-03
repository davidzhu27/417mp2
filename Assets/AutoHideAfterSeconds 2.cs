using UnityEngine;

public class AutoHideAfterSeconds : MonoBehaviour
{
    [SerializeField] private float seconds = 30f;
    [SerializeField] private bool hideOnStart = true;

    private void Start()
    {
        if (!hideOnStart) return;
        Invoke(nameof(Hide), seconds);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}