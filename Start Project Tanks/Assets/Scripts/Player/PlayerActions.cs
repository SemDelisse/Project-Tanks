using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour {
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;

    [Header("Shooting")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private int maxBullets;
    [SerializeField] private int maxRicochet;
    [SerializeField] private int bulletDamage = 1;

    private Rigidbody2D rb;
    private Unit unit;

    private Vector2 moveInput;
    private Vector2 rotateInput;

    private InputSystem_Actions inputActions;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        unit = GetComponent<Unit>();

        unit.Initialize(maxBullets);

        inputActions = new InputSystem_Actions();
    }

    private void OnEnable() {
        inputActions.Enable();
    }

    private void OnDisable() {
        inputActions.Disable();
    }

    private void Update() {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        rotateInput = inputActions.Player.Rotate.ReadValue<Vector2>();

        if (inputActions.Player.Shoot.WasPressedThisFrame()) {
            Shoot();
        }
    }

    private void FixedUpdate() {
        float rotation = rotateInput.x * rotateSpeed;

        rb.angularVelocity = -rotation;

        rb.linearVelocity =
            (Vector2)transform.up * moveInput.y * moveSpeed;
    }

    private void Shoot() {
        GameManager.Instance.CreateBullet(
            unit,
            firePoint.position,
            firePoint.right,
            bulletSpeed,
            maxRicochet,
            bulletDamage
        );
    }
}