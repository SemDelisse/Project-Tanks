using UnityEngine;

public class EnemyShooting : MonoBehaviour {
    [Header("References")]
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private Transform player;
    [SerializeField] private Transform turret;
    [SerializeField] private Transform firePoint;

    [Header("Layers")]
    [SerializeField] private LayerMask obstacleLayer;

    private Unit unit;
    private float shootTimer;
    [SerializeField] private float startupDelay = 1f;

    private void Awake() {
        unit = GetComponent<Unit>();
        unit.Initialize(enemyData.maxBullets);
    }

    private void Start() {
        FindPlayer();
        shootTimer = startupDelay;
    }

    private void Update() {
        if (player == null) {
            FindPlayer();
            return;
        }

        RotateTurretTowardsPlayer();

        shootTimer -= Time.deltaTime;


        Vector2 direction =
            (player.position - firePoint.position).normalized;

        if (!IsAimingAtPlayer(direction))
            return;

        float distance = Vector2.Distance(
            firePoint.position,
            player.position
        );

        if (!CanSeePlayer(direction, distance))
            return;

        if (shootTimer <= 0f) {
            Shoot(direction);
            shootTimer = enemyData.shootCooldown;
        }
    }

    private void RotateTurretTowardsPlayer() {
        Vector2 direction =
            player.position - turret.position;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        float targetAngle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        float newAngle =
            Mathf.MoveTowardsAngle(
                turret.eulerAngles.z,
                targetAngle,
                enemyData.turretRotationSpeed * Time.deltaTime
            );

        turret.rotation =
            Quaternion.Euler(0f, 0f, newAngle);
    }

    private bool IsAimingAtPlayer(Vector2 direction) {
        float targetAngle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        float angleDifference =
            Mathf.Abs(
                Mathf.DeltaAngle(
                    turret.eulerAngles.z,
                    targetAngle
                )
            );

        return angleDifference <= enemyData.aimAngle;
    }

    private bool CanSeePlayer(Vector2 direction, float distance) {
        RaycastHit2D hit = Physics2D.Raycast(
            firePoint.position,
            direction,
            distance,
            obstacleLayer
        );

        if (hit.collider != null) {
            return false;
        }

        return true;
    }

    private void Shoot(Vector2 direction) {
        GameManager.Instance.CreateBullet(
            unit,
            firePoint.position,
            direction,
            enemyData.bulletSpeed,
            enemyData.totalRicochets
        );
    }

    private void FindPlayer() {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
    }
}