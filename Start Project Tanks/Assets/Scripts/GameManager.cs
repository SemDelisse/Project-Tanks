using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Bullet bulletPrefab;

    [Header("Enemies")]
    [SerializeField] private int totalEnemies;

    [Header("Scene")]
    [SerializeField] private float nextSceneDelay = 1f;

    private List<Bullet> bullets = new List<Bullet>();

    private int enemiesRemaining;
    private bool sceneComplete;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        enemiesRemaining = totalEnemies;
    }

    public void EnemyKilled() {
        if (sceneComplete)
            return;

        enemiesRemaining--;

        if (enemiesRemaining <= 0) {
            CompleteScene();
        }
    }

    private void CompleteScene() {
        if (sceneComplete)
            return;

        sceneComplete = true;

        Invoke(nameof(LoadNextScene), nextSceneDelay);
    }

    private void LoadNextScene() {
        int currentScene =
            SceneManager.GetActiveScene().buildIndex;

        int nextScene =
            currentScene + 1;

        if (nextScene < SceneManager.sceneCountInBuildSettings) {
            SceneManager.LoadScene(nextScene);
        } else {
            Debug.Log("Game complete!");
        }
    }

    public Bullet CreateBullet(
        Unit owner,
        Vector2 position,
        Vector2 direction,
        float speed,
        int maxRicochets = 1,
        int damage = 1) {
        if (!owner.CanShoot())
            return null;

        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Bullet bullet = Instantiate(
            bulletPrefab,
            position,
            Quaternion.Euler(0, 0, angle - 90f)
        );

        bullet.Initialize(
            owner,
            direction,
            speed,
            maxRicochets,
            damage
        );

        owner.AddBullet();
        bullets.Add(bullet);

        return bullet;
    }

    public void DestroyBullet(Bullet bullet) {
        bullets.Remove(bullet);

        bullet.Owner.RemoveBullet();

        Destroy(bullet.gameObject);
    }
}