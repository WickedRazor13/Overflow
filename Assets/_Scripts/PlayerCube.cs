using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For UI Slider support

public class PlayerCube : MonoBehaviour
{
    [Header("Movement Variables")]
    [Tooltip("Player's horizontal movement speed.")]
    public float HoMoveSpeed = 5f;                  // Player's horizontal movement speed.
    [Tooltip("Player's vertical movement speed.")]
    public float VeMoveSpeed = 100f;                // Player's vertical movement speed.

    [Header("Ice Platform Variables")]
    [Tooltip("Place prefab for ice platform here.")]
    public GameObject icePlatformPrefab;            // Prefab for ice platform.
    [Tooltip("Place prefab for placement indicator here.")]
    public GameObject indicatorPrefab;              // Indicator prefab.
    [Tooltip("Distance to spawn ice platform.")]
    public float spawnDistance = 10f;               // Distance ice platform will spawn.
    [Tooltip("Audio Source for ice platform placing sound.")]
    public AudioSource iceSound;
    [Tooltip("Cooldown for placing platforms. (in seconds)")]
    public float platformCooldown = 1f;
    private float cooldownTimer;
    [Tooltip("Cooldown for refilling platform ammo. (in seconds)")]
    public float refillCooldown = 3f;
    private float refillTimer;
    private bool refill = false;
    [Tooltip("Max number of ice platforms.")]
    public const int maxPlatforms = 5;
    private int platformNumber;

    [Header("Dynamic")]
    public GameObject indicator;                    // For indicator prefab.
    public Vector3 playerPos;                       // Player position.
    public int playerHealth = 10;                   // Player health.

    [Header("Air Variables")]
    [Tooltip("Player air capacity.")]
    public float maxAir = 100f;                     // Player's air capacity.
    [Tooltip("Rate the player loses air.")]
    public float airDecreaseRate = 10f;             // Rate player's air decreases.
    [Tooltip("Rate the player can recover air.")]
    public float airIncreaseRate = 5f;              // Rate player's air increases.
    [Tooltip("Amount of air player has left.")]
    public float currentAir;                        // Air player has left.
    [Tooltip("Place slider for air here.")]
    public Slider oxygenSlider;                     // Slider for oxygen display
    private bool isUnderwater = false;
    [Tooltip("Place player's head collider here.")]
    public GameObject playerHead;

    private Rigidbody rb;                           // Player's rigid body.
    private Vector3 mvmt;                           // Vector for player movement direction. (Positive means the player is moving right, negative is left)
    private float healthLossTimer = 0f;             // Timer for player health loss.
    private Camera mainCam;                         // Camera.

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Get main camera for converting mouse position to world position
        mainCam = Camera.main;

        playerPos = transform.position;

        indicator = Instantiate(indicatorPrefab) as GameObject;
        indicator.transform.position = playerPos;
        indicator.SetActive(true);

        this.GetComponent<SphereCollider>().enabled = false;

        currentAir = maxAir;

        // Initialize oxygen slider
        if (oxygenSlider != null)
        {
            oxygenSlider.maxValue = maxAir;
            oxygenSlider.value = currentAir;
        }

        // Initialize timers
        cooldownTimer = platformCooldown;
        refillTimer = refillCooldown;

        // Initialize platform capacity
        platformNumber = maxPlatforms;
    }

    void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        mvmt.x = Input.GetAxisRaw("Horizontal");
        bool canJump = Mathf.Abs(rb.velocity.y) < 0.01f;

        // Display indicator
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -mainCam.transform.position.z;
        Vector3 mousePos3D = mainCam.ScreenToWorldPoint(mousePos2D);

        Vector3 mouseDelta = mousePos3D - playerPos;
        // Limit mouseDelta to radius of SphereCollider
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }
        Vector3 placementPos = playerPos + mouseDelta;
        indicator.transform.position = placementPos;

        if (Input.GetMouseButtonDown(0) && cooldownTimer <= 0f && !refill)
        {
            Instantiate(icePlatformPrefab, placementPos, Quaternion.identity);
            iceSound.Play();
            cooldownTimer = platformCooldown;
            platformNumber--;   // Decrement platform number
        }

        // Check for platform capacity and reload if needed
        if(Input.GetKey(KeyCode.R) || platformNumber == 0 || refill)
        {
            Debug.Log("reloading");
            refill = true;

            if (refillTimer > 0f)
            {
                refillTimer -= Time.deltaTime;
            }
            else
            {
                Debug.Log("finished reloading");
                refillTimer = refillCooldown;   // Reset refill timer
                platformNumber = maxPlatforms;  // Reset platforms
                refill = false;
            }
        }

        // Handle oxygen decrease or increase
        if (isUnderwater)
        {
            currentAir -= airDecreaseRate * Time.deltaTime; // Decrease player's air.
            currentAir = Mathf.Max(currentAir, 0);          // Clamp to 0

            if (oxygenSlider != null)
            {
                oxygenSlider.value = currentAir;            // Update slider
            }

            if (currentAir <= 0)
            {
                Debug.Log("Player is out of air!");
                healthLossTimer += Time.deltaTime;          // Update health loss timer.

                // Every 1 second decrease the player health by 2.
                // This means the player will lose all health in 5 seconds.
                if (healthLossTimer >= 1f)
                {
                    playerHealth -= 2;
                    playerHealth = Mathf.Max(playerHealth, 0);
                    healthLossTimer = 0f;
                }
            }
        }
        else
        {
            // Replenish oxygen when out of water
            currentAir += airIncreaseRate * Time.deltaTime;
            currentAir = Mathf.Min(currentAir, maxAir); // Clamp to maxAir

            if (oxygenSlider != null)
            {
                oxygenSlider.value = currentAir; // Update slider
            }

            healthLossTimer = 0f;
        }

        if (Input.GetKeyDown(KeyCode.W) && canJump)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        Vector3 newPos = new Vector3(
            rb.position.x + mvmt.x * HoMoveSpeed * Time.fixedDeltaTime,
            rb.position.y,
            rb.position.z);
        rb.MovePosition(newPos);

        // Update player position so that indicator logic follows position
        playerPos = rb.position;
        FollowCam.POI = this.gameObject;

        //Player Rotations
        if (mvmt.x < 0) // Rotate Left
        {
            transform.localEulerAngles = new Vector3(0, 75, 0);
        }
        else if (mvmt.x > 0) // Rotate Right
        {
            transform.localEulerAngles = new Vector3(0, -75, 0);
        }
        else if(mvmt.x == 0) // Face Forward
        {
            //transform.localEulerAngles = new Vector3(0, 0, 0);
        }

    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log($"Entered Trigger: {collision.name}");

        if (collision.CompareTag("Water") && playerHead.GetComponent<Collider>().bounds.Intersects(collision.bounds))
        {
            isUnderwater = true;
            Debug.Log("Player is underwater.");
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        Debug.Log($"Exited Trigger: {collision.name}");

        if (collision.CompareTag("Water") && !playerHead.GetComponent<Collider>().bounds.Intersects(collision.bounds))
        {
            isUnderwater = false;
            Debug.Log("Player exited water.");
        }
    }

    Vector3 GetSize(GameObject prefab)
    {
        Renderer renderer = prefab.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds.size;
        }
        else
        {
            return Vector3.zero;
        }
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * VeMoveSpeed, ForceMode.Impulse);
    }
}
