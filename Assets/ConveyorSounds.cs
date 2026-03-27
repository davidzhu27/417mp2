using UnityEngine;

public class ConveyorSounds : MonoBehaviour
{
    public AudioSource startAudio;
    public AudioSource loopAudio;

    private bool isRunning = false;

    public void StartConveyor()
    {
        if (isRunning) return;

        isRunning = true;

        // Play start sound
        startAudio.Play();
        Debug.Log("playing starting sound");
    }

    public void StartLoop()
    {
    }

    public void StopConveyor()
    {
    }
}