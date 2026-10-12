using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour {
    [SerializeField] private int maxLives = 3;

    [Header("Life UI")]
    [SerializeField] private GameObject[] lifeUI;

    public int CurrentLives { get; private set; }

    private void Awake() {
        CurrentLives = maxLives;

        UpdateLifeUI();
    }

    public void TakeDamage(int damage) {
        if (CurrentLives <= 0)
            return;

        CurrentLives -= damage;

        CurrentLives = Mathf.Max(CurrentLives, 0);

        UpdateLifeUI();

        if (CurrentLives <= 0) {
            Debug.Log("You died");
            Debug.Log("Restarting from level 1 in 5 seconds");

            gameObject.SetActive(false);

            Invoke(nameof(Restart), 5f);
        }
    }

    private void UpdateLifeUI() {
        for (int i = 0; i < lifeUI.Length; i++) {
            lifeUI[i].SetActive(i < CurrentLives);
        }
    }

    private void Restart() {
        SceneManager.LoadScene(0);
    }
}