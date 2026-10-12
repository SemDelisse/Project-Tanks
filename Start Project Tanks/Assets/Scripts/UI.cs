using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour {
    public void Restart() {
        SceneManager.LoadScene(0);
    }
}