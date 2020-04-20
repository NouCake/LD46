using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlameIndicator : MonoBehaviour {

    private Transform target;
    private Image imgIndicator;

    private void Awake() {
        imgIndicator = transform.Find("Indicator").GetComponent<Image>();
    }

    void Update() {
        if (target != null) {
            if (isTargetOnScreen()) {
                imgIndicator.gameObject.SetActive(false);
            } else {
                imgIndicator.gameObject.SetActive(true);
                updatePosition();
            }
        } else {
            Destroy(gameObject);
        }
    }

    private void updatePosition() {
        Bounds bo = new Bounds(Vector3.zero, new Vector3(Screen.width, Screen.height, 1));
        Vector3 rayDir = Camera.main.WorldToScreenPoint(target.position) - new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);

        float d;
        bo.IntersectRay(new Ray(Vector3.zero, new Vector3(rayDir.x, rayDir.y, 0)), out d);

        transform.localPosition = rayDir.normalized * -d;
    }

    private bool isTargetOnScreen() {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);
        if(screenPos.x > 0 && screenPos.x < Screen.width &&
            screenPos.y > 0 && screenPos.y < Screen.height) {
            return true;
        }
        return false;
    }

    public void SetTarget(Color color, Transform target, float scale) {
        imgIndicator.color = color;
        this.target = target;
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
