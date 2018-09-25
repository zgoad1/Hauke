using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {
    public Color color = Color.red;
    public Mesh mesh;
    public float radius = 8;

    private void Reset() {
        mesh = Resources.Load<Mesh>("spawnpoint");
    }

    private void OnDrawGizmos() {
        Gizmos.color = color;
        Gizmos.DrawMesh(mesh, transform.position);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
