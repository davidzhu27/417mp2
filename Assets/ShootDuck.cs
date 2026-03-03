using UnityEngine;
using UnityEngine.InputSystem;

public class DuckButtShooter : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference action;   // assign Left Primary Button here
    public InputActionReference debugAction;

    [Header("Duck Production")]
    public int ducksPerShot = 1;
    public float shootCooldown = 0.3f;

    [Header("Duck Spawn")]
    public GameObject smallDuckPrefab;    // visual duck
    public Transform buttShootPoint;      // where ducks come out
    public float shootForce = 2.5f;

    public AudioSource quackSource;

    private float lastShootTime = -999f;

    void Start()
    {
        action.action.Enable();
        action.action.performed += OnShoot;

        debugAction.action.Enable();
        debugAction.action.performed += OnDebugShoot;
    }

    void OnDestroy()
    {
        action.action.performed -= OnShoot;
        debugAction.action.performed -= OnDebugShoot;
    }

    private void OnShoot(InputAction.CallbackContext ctx)
    {
        if (Time.time - lastShootTime < shootCooldown)
            return;

        lastShootTime = Time.time;

        // Increase resource
        ResourceManager.Instance.AddDucks(ducksPerShot);

        if (quackSource != null)
        {   
            Debug.Log("Playing quack sound");
            quackSource.Play();
        }

        // Spawn visual duck
        if (smallDuckPrefab != null && buttShootPoint != null)
        {
            GameObject duck = Instantiate(
                smallDuckPrefab,
                buttShootPoint.position,
                buttShootPoint.rotation
            );

            Rigidbody rb = duck.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(buttShootPoint.forward * shootForce, ForceMode.Impulse);
            }
        }
    }

    private void OnDebugShoot(InputAction.CallbackContext ctx)
    {
        ResourceManager.Instance.AddDucks(5);
    }
}
