using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

    private static UIController instance;
    public static UIController GetInstance() {
        if(instance == null) {
            Debug.LogError("UiController was requested before instantiating");
        }
        return instance;
    }

    private void Awake() {
        if(instance != null) {
            Debug.LogError("Multiple UIController");
        }
        instance = this;
    }

    [SerializeField] private GameObject flameHUDPrefab;
    [SerializeField] private Transform hudContainer;

    [SerializeField] private GameObject targetIdicatorPrefab;
    [SerializeField] private Transform indicatorContainer;


    void Start() {
        
    }

    void Update() {
        
    }

    public void AddFlameHUD(FlameControler flame) {
        GameObject hud = Instantiate(flameHUDPrefab, hudContainer);
        hud.GetComponent<FlameHUD>().SetTarget(flame);
    }

    public void AddTargetIndicator(Color color, Transform target, float scale = 1) {
        GameObject ind = Instantiate(targetIdicatorPrefab, indicatorContainer);
        ind.GetComponent<FlameIndicator>().SetTarget(color, target, scale);
    }

}
