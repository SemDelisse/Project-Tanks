using UnityEngine;

public class Unit : MonoBehaviour {
    public int MaxBullets { get; private set; }
    public int ActiveBullets { get; private set; }

    public void Initialize(int maxBullets) {
        MaxBullets = maxBullets;
        ActiveBullets = 0;
    }

    public bool CanShoot() {
        return ActiveBullets < MaxBullets;
    }

    public void AddBullet() {
        if (!CanShoot())
            return;

        ActiveBullets++;
    }

    public void RemoveBullet() {
        ActiveBullets = Mathf.Max(0, ActiveBullets - 1);
    }
}