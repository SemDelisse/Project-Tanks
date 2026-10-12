using UnityEngine;

public class MineDetection : MonoBehaviour {
    [SerializeField] private int damage = 1;

    private void OnTriggerEnter2D(Collider2D other) {
        DamageTarget(other.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other) {
        DamageTarget(other.gameObject);
    }

    private void DamageTarget(GameObject target) {
        if (target.CompareTag("Enemy")) {
            GameManager.Instance.EnemyKilled();
            Destroy(target);
        } else if (target.CompareTag("Player")) {
            PlayerHealth playerHealth =
                target.GetComponent<PlayerHealth>();

            // Will still instantly kill the player because this will activate every frame.
            // Its intendend that the player instantly dies, because the mine is stronger then a bullet.
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);
        }
        if (target.CompareTag("Bullet")) {
            Bullet bullet = target.GetComponent<Bullet>();

            if (bullet != null)
                bullet.DestroyBullet();
        }
    }
}