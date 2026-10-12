using UnityEngine;

public class Bullet : MonoBehaviour {
    public Vector2 Direction { get; private set; }
    public float Speed { get; private set; }
    public Unit Owner { get; private set; }
    public int Damage { get; private set; }

    private int ricochetsRemaining;

    public void Initialize(
        Unit owner,
        Vector2 direction,
        float speed,
        int maxRicochets = 1,
        int damage = 1) {
        Owner = owner;
        Direction = direction.normalized;
        Speed = speed;
        Damage = damage;
        ricochetsRemaining = maxRicochets;
    }

    private void Start() {
        Invoke(nameof(DestroyBullet), 40f);
    }

    private void Update() {
        transform.position +=
            (Vector3)(Direction * Speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Wall")) {
            if (ricochetsRemaining > 0) {
                Vector2 normal =
                    collision.contacts[0].normal;

                Direction = Vector2.Reflect(
                    Direction,
                    normal
                ).normalized;

                transform.rotation =
                    Quaternion.Euler(
                        0f,
                        0f,
                        Mathf.Atan2(
                            Direction.y,
                            Direction.x
                        ) * Mathf.Rad2Deg - 90f
                    );

                ricochetsRemaining--;
            } else {
                DestroyBullet();
            }
        } else if (collision.gameObject.CompareTag("Bullet")) {
            DestroyBullet();
        } else if (collision.gameObject.CompareTag("Enemy")) {
            GameManager.Instance.EnemyKilled();

            Destroy(collision.gameObject);
            DestroyBullet();
        } else if (collision.gameObject.CompareTag("Player")) {
            PlayerHealth playerHealth =
                collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
                playerHealth.TakeDamage(Damage);

            DestroyBullet();
        }
    }

    public void DestroyBullet() {
        GameManager.Instance.DestroyBullet(this);
    }
}