using UnityEngine;
using System.Collections;

public class EyeController : MonoBehaviour
{
    [Header("Settings")]
    public Transform playerTarget;
    public float trackingSpeed = 5f;

    // These will be found automatically
    private Transform _top_eyelid;
    private Transform _bottom_eyelid;
    private Transform _pupil;
    private Transform _eyeball;

    [Header("Blink Values")]
    private float rotationAmount = 30f; // How much the eyelids rotate when blinking
    private bool _isBlinking = false;

    [Header("Pupil Scale")]
    private float pupil_scale = 1.2f; // How much to scale the pupil when widening
    private bool _isWidened = false;

    void Awake()
    {
        // 1. AUTO-FIND logic based on child names
        _top_eyelid = transform.Find("Eye/top_eyelid");
        _bottom_eyelid = transform.Find("Eye/bottom_eyelid");
        _pupil = transform.Find("Eye/Eyeball/Pupil");
        _eyeball = transform.Find("Eye/Eyeball");

        if (_top_eyelid == null || _bottom_eyelid == null)
        {
            Debug.LogWarning($"Eyelids not found on {gameObject.name}! Check naming.");
        }
        if (_pupil == null)
        {
            Debug.LogWarning($"Pupil not found on {gameObject.name}! Check naming.");
        }
        if (_eyeball == null)
        {
            Debug.LogWarning($"Eyeball not found on {gameObject.name}! Check naming.");
        }
    }

    void Update()
    {
        FollowPlayer();
    }

    void FollowPlayer()
    {
        if (playerTarget == null) return;

        // DEBUG: log the player's position and eyeball position
        // Debug.Log($"Player Target Position: {playerTarget.position}, Eyeball Position: {_eyeball.position}");
        
        Vector3 direction = -((new Vector3(0, 0, -2f) + playerTarget.position) - _eyeball.position);
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        _eyeball.rotation = Quaternion.Slerp(_eyeball.rotation, targetRotation, trackingSpeed * Time.deltaTime);
    }

    void SetEyelidRotate(float topAngle, float bottomAngle)
    {
        if (_top_eyelid != null)
            _top_eyelid.localRotation = Quaternion.Euler(topAngle, 0, 0);
        if (_bottom_eyelid != null)
            _bottom_eyelid.localRotation = Quaternion.Euler(180 - bottomAngle, 0, 0);
    }

    public IEnumerator BlinkRoutine()
    {
        if (_isBlinking) yield break; // Prevent multiple blinks at the same time
        if (_isWidened) yield break; // Don't blink if we're currently widened (looks weird)
        _isBlinking = true;

        // "blink" the eyes by rotating the eyelids (over 0.1 seconds), waiting, then rotating back

        float elapsed = 0f;
        while (elapsed < 0.1f)
        {
            float t = elapsed / 0.1f;
            SetEyelidRotate(Mathf.Lerp(rotationAmount, 0, t), Mathf.Lerp(rotationAmount, 0, t));
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Hold the blink for a short moment
        yield return new WaitForSeconds(0.1f);

        // Rotate back to open
        elapsed = 0f;
        while (elapsed < 0.1f)
        {
            float t = elapsed / 0.1f;
            SetEyelidRotate(Mathf.Lerp(0, rotationAmount, t), Mathf.Lerp(0, rotationAmount, t));
            elapsed += Time.deltaTime;
            yield return null;
        }


        _isBlinking = false;
    }

    public IEnumerator WidenEyes()
    {
        if (_pupil == null) yield break; // Can't widen if we don't have a pupil reference
        if (_isWidened) yield break; // Prevent multiple widenings at the same time
        _isWidened = true;

        Vector3 originalScale = _pupil.localScale;
        _pupil.localScale = originalScale * pupil_scale;

        // Hold the widened eyes for a short moment
        yield return new WaitForSeconds(1.0f);

        _pupil.localScale = originalScale;
        _isWidened = false;
    }
}
