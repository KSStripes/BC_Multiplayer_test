using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class NetworkPlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float rotationSpeed = 15f;

    [SerializeField] private Transform towerTransform;

    [SerializeField] private LayerMask groundLayer;

    [Space]

    //[SerializeField] private GameObject PlayerHUD;
    [SerializeField] private Slider healthBar;

    public string PlayerName;
    public int maxHealth = 100;
    private readonly NetworkVariable<int> health = new NetworkVariable<int>();

    private Camera cam;

    private Vector2 moveInput;
    private Quaternion targetRotation;

    private void Start()
    {
        cam = Camera.main;   
        groundLayer = LayerMask.GetMask("Default");
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            health.Value = maxHealth;
        }

        if (!IsOwner)
        {
            //PlayerHUD.SetActive(false);
            healthBar.maxValue = maxHealth;
            healthBar.value = health.Value;
        }

        GetName();
        health.OnValueChanged += OnHealthChanged;
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        health.OnValueChanged -= OnHealthChanged;
        base.OnNetworkDespawn();
    }

    void Update()
    {
        if (IsOwner)
        {
            AimAtMouse();

            // Input
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            SubmitInputServerRpc(input);


            if (Quaternion.Angle(towerTransform.rotation, targetRotation) > 0.3f)
                UpdateRotationServerRpc(targetRotation);

            towerTransform.rotation = targetRotation;
        }
        else
        { 
            towerTransform.rotation = Quaternion.Slerp(towerTransform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {        // Move Tank
            Vector3 movement = transform.forward * moveInput.y * moveSpeed * Time.fixedDeltaTime;
            transform.position += movement;

            // Turn Tank

            Vector3 rot = new Vector3(0, moveInput.x, 0) * rotationSpeed * Time.fixedDeltaTime;
            transform.Rotate(rot);

        }
    }

    private void AimAtMouse()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
        {
            Vector3 target = hit.point;
            target.y = towerTransform.position.y;

            Vector3 direction = (target - towerTransform.position).normalized;

            if (direction.sqrMagnitude > 0.001f)
            {
                targetRotation = Quaternion.LookRotation(direction);
            }
        }
    }
  
    [ServerRpc]
    private void SubmitInputServerRpc(Vector2 input)
    {
        moveInput = input;
    }

    [ServerRpc]
    private void UpdateRotationServerRpc(Quaternion rotation)
    {
        UpdateRotationClientRpc(rotation);
    }

    [ClientRpc]
    private void UpdateRotationClientRpc(Quaternion rotation)
    {
        targetRotation = rotation;
    }

    public void OnHealthChanged(int oldValue, int newValue)
    {
        healthBar.value = newValue;
    }

    public void TakeDamage(int amount)
    {
        if (!IsServer) return;

        health.Value -= amount;
        Debug.Log(health);
        if (health.Value <= 0)
        {
            //RespawnManager.instance.RespawnPlayer(NetworkObject);
            health.Value = maxHealth;
        }
    }

    private async void GetName()
    {
        PlayerName = await AuthenticationService.Instance.GetPlayerNameAsync();
    }
}
