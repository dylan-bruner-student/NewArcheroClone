using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Status")]
    [SerializeField] public bool IsMoving = false;
    [SerializeField] private float Health = 1000f;
    [SerializeField] private int Score = 0;
    [SerializeField] private int UpgradesUnlocked = 0;

    [Header("Other")]
    [SerializeField] private float MovementSpeed = 10f;
    [SerializeField] private float MaxHealth = 1000f;
    [SerializeField] private float m_PickupRadius = 5f;

    [Header("Refrences")]
    [SerializeField] private GameObject Enemy;
    [SerializeField] private GameObject HealthBar;
    [SerializeField] private GameObject PickupRadius;
    [SerializeField] private Image UpgradeStatusBar;

    [Header("Weapons")]
    [SerializeField] private GameObject ThrowingKnife;
    [SerializeField] private GameObject MightyBall;

    
    public static PlayerController Instance { get; private set; }


    void Start() {
        Instance = this;

        gameObject.AddComponent<ThrowingKnifeManager>().ThrowingKnifePrefab = ThrowingKnife;
        gameObject.AddComponent<MightyBallsManager>().BallRefrence = MightyBall;
    }

    private void OnValidate()
    {
        Damage(0);
        SetPickupRadius(m_PickupRadius);
    }


    void Update() {
        Vector3 velocity = Vector3.zero;


        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            velocity.y = MovementSpeed;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            velocity.y = -MovementSpeed;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            velocity.x = -MovementSpeed;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            velocity.x = MovementSpeed;


        if (Input.GetKeyDown(KeyCode.P))
        {
            TimeSystem.TogglePause();
        }


        if (Input.GetKey(KeyCode.Space)) {
            var thing = Instantiate(Enemy);
            thing.transform.position = new Vector3(Random.Range(-25, 25), Random.Range(-25, 25));
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.U))
        {
            UpgradeController.Instance.PromptRandomUpgrades();
        }


        var rb = GetComponent<Rigidbody2D>();
        rb.velocity = velocity;
        IsMoving = rb.velocity.sqrMagnitude > 0;
    }

    public void Damage(int damage)
    {
        Health = Mathf.Clamp(Health - damage, 0, MaxHealth);
        
        float p = Health / MaxHealth;
        HealthBar.transform.localScale = new Vector3(p, HealthBar.transform.localScale.y, HealthBar.transform.localScale.z);
        HealthBar.transform.localPosition = new Vector3((p - 1) * 1 / 2, HealthBar.transform.localPosition.y, HealthBar.transform.localPosition.z);

    }

    public void SetPickupRadius(float radius) 
    {
        PickupRadius.transform.localScale = new Vector3(radius, radius, 1);
    }

    public void AddScore(int points)
    {
        int upgradeCount = UpgradeController.Instance.AppliedUpgrades.Count;
        int nextUpgradeAt = Mathf.FloorToInt(Mathf.Pow(1 + (upgradeCount * 0.5f), 2) * 200);

        if (Score > nextUpgradeAt)
            UpgradeController.Instance.PromptRandomUpgrades();

        Score += points;
        
        // progress bar 
        int lastUpgradeReq = Mathf.Max(0, Mathf.FloorToInt(Mathf.Pow(1 + ((upgradeCount - 1) * 0.5f), 2) * 200));
        float p = (float) (Score - lastUpgradeReq) / (nextUpgradeAt - lastUpgradeReq);
        UpgradeStatusBar.fillAmount = p;

        // Debug.Log($"Current: {Score}, Next: {nextUpgradeAt}, LastUpgrade: {lastUpgradeReq}, p: {p}");
    }
}
