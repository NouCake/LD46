using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitController : MonoBehaviour {

    [SerializeField] private float spitSpeed = 5;
    [SerializeField] private float spitDamage = 5;

    private Vector3 direction;

    public void Shoot(Vector3 target) {
        Vector3 dir = (target - transform.position).normalized;
        this.direction = dir;
        //transform.localRotation = Quaternion.LookRotation(dir); 
        transform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }

    void Update() {
        transform.position += direction * spitSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Fire") {
            Destroy(gameObject);
            FlameControler flame = collision.GetComponent<FlameControler>();
            flame.SetFlameSize(flame.GetFlameSize() - spitDamage);
        }
        if (collision.tag == "Terrain") {
            Destroy(gameObject);
        }
    }

}
