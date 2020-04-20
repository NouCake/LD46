using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour {

    [SerializeField] private float speed = 5;
    [SerializeField] private Vector2 bounds = new Vector2(10, 5);

    private Vector3 speedVec;

    void Start() {

    }

    void Update() {
        if (!GameController.IsPaused()) {
            Vector3 input = Vector2.up * Input.GetAxis("Vertical") + Vector2.right * Input.GetAxis("Horizontal");

            if (input.x == 0) speedVec.x = 0;
            if (input.y == 0) speedVec.y = 0;

            //speedVec += input * Time.deltaTime;

            transform.position += input * Time.deltaTime * speed;
            if (transform.position.x > bounds.x) transform.position = new Vector3(bounds.x, transform.position.y, transform.position.z);
            if (transform.position.x < -bounds.x) transform.position = new Vector3(-bounds.x, transform.position.y, transform.position.z);
            if (transform.position.y > bounds.y) transform.position = new Vector3(transform.position.x, bounds.y, transform.position.z);
            if (transform.position.y < -bounds.y) transform.position = new Vector3(transform.position.x, -bounds.y, transform.position.z);
        }
    }
}
