using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Status")]
    [SerializeField] public bool IsMoving = false;
    [SerializeField] public float LastDamagedTime = 0f;
    [SerializeField] public float LastSprintTime = 0f;
    [SerializeField] public float Health = 1000f;
    [SerializeField] private float Stamina = 50f;
    [SerializeField] private int Score = 0;

    [Header("Other")]
    [SerializeField] private float TimeScale = 1f;
    [SerializeField] private float MovementSpeed = 10f;
    [SerializeField] public float MaxHealth = 1000f;
    [SerializeField] private float MaxShield = 50f;
    [SerializeField] public float CritChance = 0f;
    [SerializeField] private float CritModifier = 1.75f;
    [SerializeField] public float StaminaRegenSpeed = 5;
    [SerializeField] public float m_PickupRadius = 1.5f;
    [SerializeField] public float SprintModifier = 2f;
    [SerializeField] public float RegenPerKill = 0f;

    [Header("Touch Controls")]
    [SerializeField] private float doubleTapTimeThreshold = 0.3f;

    [Header("References")]
    [SerializeField] private GameObject Enemy;
    [SerializeField] private GameObject HealthBar;
    [SerializeField] private GameObject ShieldBar;
    [SerializeField] private GameObject PickupRadius;
    [SerializeField] public GameObject HitMarker;
    [SerializeField] public GameObject CoinDrop;
    [SerializeField] private Image UpgradeStatusBar;

    [Header("Weapons")]
    [SerializeField] public GameObject ThrowingKnife;
    [SerializeField] public GameObject MightyBall;

    // Touch control variables
    private float lastTapTime;
    private bool isSprinting = false;
    private Vector2 touchStartPos;
    private bool isDragging = false;
    private bool isDoubleTap = false;

    public static PlayerController Instance { get; private set; }

    void Start()
    {
        Instance = this;
    }

    private void Awake()
    {
        //DontDestroyOnLoad(this);
    }

    private void OnValidate()
    {
        Damage(0);
        SetPickupRadius(m_PickupRadius);

        if (!TimeSystem.Paused)
            Time.timeScale = TimeScale;
    }

    void Update()
    {
        Instance = this;

        Vector3 velocity = Vector3.zero;

        if ((Time.time - LastDamagedTime > 1 && Time.time - LastSprintTime > 1) && Stamina < MaxShield)
        {
            Stamina = Mathf.Clamp(Stamina + (StaminaRegenSpeed * Time.deltaTime), 0, MaxShield);
            Damage(0);
        }

        // Handle keyboard input
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            velocity.y = MovementSpeed;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            velocity.y = -MovementSpeed;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            velocity.x = -MovementSpeed;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            velocity.x = MovementSpeed;

        // Reset sprinting flag if no touches are active
        if (Input.touchCount == 0)
        {
            isSprinting = false;
            isDoubleTap = false;
        }

        // Handle touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Check for double tap (sprint)
                    if (Time.time - lastTapTime < doubleTapTimeThreshold)
                    {
                        isDoubleTap = true;
                        if (Stamina > 1)
                        {
                            isSprinting = true;
                            LastSprintTime = Time.time;
                        }
                    }
                    lastTapTime = Time.time;

                    // Start drag
                    touchStartPos = touch.position;
                    isDragging = true;
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (isDragging)
                    {
                        // Calculate movement direction based on drag
                        Vector2 dragDirection = touch.position - touchStartPos;

                        // Only move if drag distance is significant
                        if (dragDirection.magnitude > 20f)
                        {
                            // Normalize and scale the movement
                            dragDirection.Normalize();
                            velocity.x = dragDirection.x * MovementSpeed;
                            velocity.y = dragDirection.y * MovementSpeed;
                        }
                    }

                    // Continue sprinting as long as the touch is active after a double tap
                    if (isDoubleTap && Stamina > 1)
                    {
                        isSprinting = true;
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isDragging = false;
                    // Sprint will end when touchCount becomes 0
                    break;
            }
        }

        // Apply sprint if using keyboard or touch sprint is active
        if ((Input.GetKey(KeyCode.LeftShift) || isSprinting) && Stamina > 1)
        {
            LastSprintTime = Time.time;
            velocity *= SprintModifier;
            Stamina -= 10 * Time.deltaTime;
            Damage(0);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TimeSystem.TogglePause();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            var thing = Instantiate(Enemy);
            thing.transform.position = new Vector3(Random.Range(-25, 25), Random.Range(-25, 25));
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.U))
        {
            UpgradeController.Instance.PromptRandomUpgrades();
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.M))
        {
            UpgradeController.Instance.AppliedUpgrades.Clear();
            foreach (var upgrade in UpgradeController.Upgrades)
                for (int i = 0; i < upgrade.MaxApplied; i++)
                    upgrade.SystemApplyUpgrade();

            Debug.Log($"Applied {UpgradeController.Instance.AppliedUpgrades.Count} upgrades!");
        }

        var rb = GetComponent<Rigidbody2D>();
        rb.velocity = velocity;
        IsMoving = rb.velocity.sqrMagnitude > 0;
    }

    public void Damage(float damage)
    {
        if (damage > 0)
            LastDamagedTime = Time.time;

        float shieldLeft = Mathf.Max(0, Stamina - damage);
        damage = Mathf.Max(0, damage - Stamina);
        Stamina = shieldLeft;

        Health = (int)Mathf.Clamp(Health - damage, 0, MaxHealth);

        // update health bar
        float p = Health / MaxHealth;
        HealthBar.transform.localScale = new Vector3(p, HealthBar.transform.localScale.y, HealthBar.transform.localScale.z);
        HealthBar.transform.localPosition = new Vector3((p - 1) * 1 / 2, HealthBar.transform.localPosition.y, HealthBar.transform.localPosition.z);

        // update the shield bar
        float s = Stamina / MaxShield;
        ShieldBar.transform.localScale = new Vector3(s, ShieldBar.transform.localScale.y, ShieldBar.transform.localScale.z);
        ShieldBar.transform.localPosition = new Vector3((s - 1) * 1 / 2, ShieldBar.transform.localPosition.y, ShieldBar.transform.localPosition.z);

        if (Health <= 0)
            SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }

    public void Damage(int damage) { Damage((float)damage); }

    public void SetPickupRadius(float radius)
    {
        m_PickupRadius = radius;
        PickupRadius.transform.localScale = new Vector3(radius, radius, 1);
    }

    public void AddScore(int points)
    {
        int upgradeCount = UpgradeController.Instance.AppliedUpgrades.Count;
        int nextUpgradeAt = Mathf.FloorToInt(Mathf.Pow(1 + (upgradeCount * 0.5f), 2) * 200);

        if (Score >= nextUpgradeAt)
            UpgradeController.Instance.PromptRandomUpgrades();

        Score += points;

        // progress bar 
        int lastUpgradeReq = Mathf.Max(0, Mathf.FloorToInt(Mathf.Pow(1 + ((upgradeCount - 1) * 0.5f), 2) * 200));
        float p = (float)(Score - lastUpgradeReq) / (nextUpgradeAt - lastUpgradeReq);
        UpgradeStatusBar.fillAmount = p;

        Debug.Log($"Current: {Score}, Next: {nextUpgradeAt}, LastUpgrade: {lastUpgradeReq}, p: {p}");
    }

    public float TryCrit(float damage)
    {
        if (Random.value < CritChance)
            damage *= CritModifier;
        return damage;
    }
}
