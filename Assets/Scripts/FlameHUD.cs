using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlameHUD : MonoBehaviour {

    private FlameControler target;
    private TextMeshProUGUI healthText;

    private void Awake() {
        healthText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
    }

    void Update() {
        if(target != null) {
            string displayText = "" + (int)target.GetFlameSize();
            healthText.text = displayText;
            transform.position = target.transform.position;
        } else {
            Destroy(gameObject);
        }
    }

    public void SetTarget(FlameControler target) {
        this.target = target;
    }
}
