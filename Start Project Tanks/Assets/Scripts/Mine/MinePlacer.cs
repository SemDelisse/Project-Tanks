using UnityEngine;
using UnityEngine.InputSystem;

public class MinePlacer : MonoBehaviour {
    [SerializeField] private GameObject minePrefab;
    [SerializeField] private float cooldown = 2f;

    private float cooldownTimer;

    private void Update() {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (Keyboard.current.spaceKey.wasPressedThisFrame &&
            cooldownTimer <= 0f) {
            PlaceMine();
        }
    }

    private void PlaceMine() {
        Instantiate(
            minePrefab,
            transform.position,
            Quaternion.identity
        );

        cooldownTimer = cooldown;
    }
}