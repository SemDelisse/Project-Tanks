using UnityEngine;

public enum EnemyMovementType {
    None,
    Default,
    Random,
    Defensive,
    SuicideBomber
}

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Enemies/Enemy Data")]
public class EnemyData : ScriptableObject {
    [Header("General")]
    public string enemyName;

    [Header("Movement")]
    public EnemyMovementType movementType = EnemyMovementType.Default;
    public float moveSpeed = 3f;
    public float rotationSpeed = 180f;
    public float reverseAngle = 120f;
    public float rotateOnlyAngle = 15f;
    public float movementRange = 5f;

    [Header("Combat")]
    public float turretRotationSpeed = 360f;
    public float aimAngle = 5f;
    public float shootCooldown = 1f;

    [Header("Shooting")]
    public int maxBullets;
    public float bulletSpeed;
    public int totalRicochets;

    [Header("Mines")]
    public int maxMines;

    [Header("Powers")]
    public bool invisible;

    [Header("Extra")]
    [TextArea(5, 10)]
    public string notes;
}