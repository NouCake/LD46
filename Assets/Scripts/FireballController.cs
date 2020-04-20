using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour {

    [SerializeField] private float shootSpeed = 10;

    private Vector3 shootDirection;

    public void Shoot(Vector2 dir) {
        shootDirection = dir;
    }

    void Update() {
        transform.position += shootDirection * (shootSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        //Debug.Log("Collieded with " + collision.tag + " | " + collision.name);
        if(collision.tag == "Terrain") {
            Destroy(gameObject);
        } 
        if(collision.tag == "Enemy") {
            Destroy(gameObject);
            collision.GetComponent<AlpacaController>().Kill();
        }
        if (collision.tag == "EnemyBullet") {
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }


}
