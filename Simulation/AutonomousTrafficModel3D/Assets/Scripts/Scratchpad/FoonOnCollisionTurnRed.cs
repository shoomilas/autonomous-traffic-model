using UnityEngine;

public class FoonOnCollisionTurnRed : MonoBehaviour {
    private void OnParticleCollision(GameObject other) {
        other.GetComponent<Material>().SetColor("red", Color.red);
    }
}