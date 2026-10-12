using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour {
    [Header("References")]
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private Transform player;

    [Header("Pathfinding")]
    [SerializeField] private float pathUpdateRate = 0.15f;
    [SerializeField] private float waypointDistance = 0.3f;

    [Header("Layers")]
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Random Movement")]
    [SerializeField] private float randomMoveRadius = 10f;
    [SerializeField] private float randomWaitTime = 1.5f;

    [Header("Turret")]
    [SerializeField] private float aimTolerance = 5f;
    [SerializeField] private float visionRange = 10f;

    private Rigidbody2D rb;
    private NavMeshPath path;
    private NavMeshPath randomPath;

    private Vector3[] corners;
    private int currentCorner;

    private float pathTimer;
    private bool reversing;

    private Vector3 randomTarget;
    private bool hasRandomTarget;
    private float randomWaitTimer;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        path = new NavMeshPath();
        randomPath = new NavMeshPath();
    }

    private void Start() {
        FindPlayer();

        if (enemyData != null &&
            enemyData.movementType == EnemyMovementType.Default) {
            UpdatePath();
        }
    }

    private void Update() {
        if (player == null) {
            FindPlayer();
            return;
        }

        if (enemyData == null ||
            enemyData.movementType == EnemyMovementType.None) {
            return;
        }

        if (enemyData.movementType == EnemyMovementType.Default) {
            pathTimer -= Time.deltaTime;

            if (pathTimer <= 0f) {
                pathTimer = pathUpdateRate;
                UpdatePath();
            }
        }
    }

    private void FixedUpdate() {
        if (enemyData == null || player == null) {
            Stop();
            return;
        }

        switch (enemyData.movementType) {
            case EnemyMovementType.None:
                None();
                break;

            case EnemyMovementType.Default:
                Default();
                break;

            case EnemyMovementType.Random:
                RandomMovement();
                break;

            case EnemyMovementType.Defensive:
                Defensive();
                break;

            case EnemyMovementType.SuicideBomber:
                SuicideBomber();
                break;
        }
    }

    private void None() {
        Stop();
    }

    private void Default() {
        float distance = Vector2.Distance(
            rb.position,
            player.position
        );

        if (distance <= enemyData.movementRange) {
            Stop();
            return;
        }

        FollowPath();
    }

    private void RandomMovement() {
        if (randomWaitTimer > 0f) {
            Stop();
            randomWaitTimer -= Time.fixedDeltaTime;
            return;
        }

        if (!hasRandomTarget) {
            ChooseRandomTarget();
            return;
        }

        FollowPath();

        if (currentCorner >= corners.Length) {
            hasRandomTarget = false;
            randomWaitTimer = randomWaitTime;
            Stop();
        }
    }

    private void Defensive() {
    }

    private void SuicideBomber() {
    }

    private void UpdatePath() {
        if (player == null)
            return;

        if (!NavMesh.SamplePosition(
            transform.position,
            out NavMeshHit startHit,
            2f,
            NavMesh.AllAreas)) {
            return;
        }

        if (!NavMesh.SamplePosition(
            player.position,
            out NavMeshHit targetHit,
            2f,
            NavMesh.AllAreas)) {
            return;
        }

        if (!NavMesh.CalculatePath(
            startHit.position,
            targetHit.position,
            NavMesh.AllAreas,
            path)) {
            return;
        }

        if (path.status != NavMeshPathStatus.PathComplete)
            return;

        corners = path.corners;
        currentCorner = 1;
        reversing = false;
    }

    private void FollowPath() {
        if (corners == null || corners.Length < 2) {
            Stop();
            return;
        }

        while (
            currentCorner < corners.Length &&
            Vector2.Distance(
                rb.position,
                corners[currentCorner]
            ) <= waypointDistance) {
            currentCorner++;
        }

        if (currentCorner >= corners.Length) {
            Stop();
            return;
        }

        Vector2 direction =
            (Vector2)corners[currentCorner] - rb.position;

        if (direction.sqrMagnitude < 0.001f) {
            Stop();
            return;
        }

        direction.Normalize();

        float forwardAngle = Vector2.SignedAngle(
            transform.up,
            direction
        );

        float absoluteAngle = Mathf.Abs(forwardAngle);

        if (!reversing && absoluteAngle > enemyData.reverseAngle) {
            reversing = true;
        } else if (
              reversing &&
              absoluteAngle < enemyData.reverseAngle - 25f) {
            reversing = false;
        }

        Vector2 movementDirection =
            reversing ? -direction : direction;

        float targetAngle =
            Mathf.Atan2(
                movementDirection.y,
                movementDirection.x
            ) * Mathf.Rad2Deg - 90f;

        float angleDifference =
            Mathf.DeltaAngle(
                rb.rotation,
                targetAngle
            );

        float maxRotation =
            enemyData.rotationSpeed *
            Time.fixedDeltaTime;

        float rotation =
            Mathf.Clamp(
                angleDifference,
                -maxRotation,
                maxRotation
            );

        rb.MoveRotation(
            rb.rotation + rotation
        );

        float remainingAngle =
            Mathf.Abs(
                Mathf.DeltaAngle(
                    rb.rotation,
                    targetAngle
                )
            );

        if (remainingAngle <= enemyData.rotateOnlyAngle) {
            float movement =
                reversing ? -1f : 1f;

            rb.linearVelocity =
                (Vector2)transform.up *
                movement *
                enemyData.moveSpeed;
        } else {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public bool CanSeePlayer() {
        if (player == null)
            return false;

        Vector2 direction =
            (Vector2)player.position - rb.position;

        float distance = direction.magnitude;

        if (distance > visionRange)
            return false;

        if (distance <= 0.001f)
            return true;

        float angle =
            Vector2.Angle(
                transform.up,
                direction
            );

        if (angle > aimTolerance)
            return false;

        RaycastHit2D hit = Physics2D.Raycast(
            rb.position,
            direction.normalized,
            distance,
            obstacleLayer
        );

        return hit.collider == null;
    }

    private void Stop() {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    private void FindPlayer() {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
    }

    private void ChooseRandomTarget() {
        for (int i = 0; i < 20; i++) {
            Vector2 randomDirection =
                Random.insideUnitCircle * randomMoveRadius;

            Vector3 randomPosition =
                transform.position +
                new Vector3(
                    randomDirection.x,
                    randomDirection.y,
                    0f
                );

            if (!NavMesh.SamplePosition(
                randomPosition,
                out NavMeshHit targetHit,
                2f,
                NavMesh.AllAreas)) {
                continue;
            }

            if (!NavMesh.SamplePosition(
                transform.position,
                out NavMeshHit startHit,
                2f,
                NavMesh.AllAreas)) {
                return;
            }

            if (!NavMesh.CalculatePath(
                startHit.position,
                targetHit.position,
                NavMesh.AllAreas,
                randomPath)) {
                continue;
            }

            if (randomPath.status != NavMeshPathStatus.PathComplete)
                continue;

            randomTarget = targetHit.position;
            corners = randomPath.corners;
            currentCorner = 1;
            reversing = false;
            hasRandomTarget = true;

            return;
        }

        Stop();
    }
}