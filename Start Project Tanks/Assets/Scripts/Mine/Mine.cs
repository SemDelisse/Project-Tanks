using UnityEngine;

public class Mine : MonoBehaviour {
    [SerializeField] private GameObject explosionObject;
    [SerializeField] private float destroyDelay = 1.5f;

    private bool triggered;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (triggered)
            return;

        if (collision.gameObject.CompareTag("Bullet")) {
            TriggerExplosion();
        }
    }

    private void TriggerExplosion() {
        triggered = true;

        if (explosionObject != null)
            explosionObject.SetActive(true);

        Destroy(gameObject, destroyDelay);
    }
}