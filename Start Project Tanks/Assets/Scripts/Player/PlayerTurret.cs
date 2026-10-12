using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTurret : MonoBehaviour {
    void Update() {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0f;

        Vector2 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; 
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }
}
