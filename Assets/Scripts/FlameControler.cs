using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class FlameControler : MonoBehaviour {

    [SerializeField] AnimationCurve growthCurve;
    [SerializeField] AnimationCurve emissionCurve;
    [SerializeField] AnimationCurve alphaCurve;
    [SerializeField] AnimationCurve hoverCurve;
    [SerializeField] AnimationCurve scaleCurve;
    [SerializeField] private float newFlameSize = 10;
    [SerializeField] private float flameGrowthSpeed = 10;

    [SerializeField] public float flameBurnrate = 2;

    private Transform dropShadow;
    private ParticleSystem fireEmitter;
    private float flameSize;

    void Start() {
        fireEmitter = transform.Find("Emitter").GetComponent<ParticleSystem>();
        dropShadow = transform.Find("DropShadow");

        UIController ui = UIController.GetInstance();
        ui.AddFlameHUD(this);
        ui.AddTargetIndicator(Color.yellow, transform, 2);

        UpdateFlame();
    }

    private void Update() {
        burnUpdate();
        if (newFlameSize != flameSize) {
            float growth = Time.deltaTime * flameGrowthSpeed;
            if (flameSize + growth < newFlameSize) {
                flameSize += growth;
            } else {
                flameSize = newFlameSize;
            }
            UpdateFlame();
        }
    }

    private void UpdateFlame() {
        float key = flameSize / 400.0f;
        key = Mathf.Clamp(key, 0, 1);
        key = growthCurve.Evaluate(key);
        var emission = fireEmitter.emission; emission.rateOverTime = emissionCurve.Evaluate(key);
        var color = fireEmitter.colorOverLifetime;
        Gradient g = new Gradient();
        g.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.white, 0), new GradientColorKey(Color.white, 1) }, new GradientAlphaKey[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(alphaCurve.Evaluate(key), 0.3f) });
        color.color = g;
        fireEmitter.transform.localPosition = Vector3.up * hoverCurve.Evaluate(key);
        float scale = scaleCurve.Evaluate(key);
        fireEmitter.transform.localScale = new Vector3(scale, scale, scale);
        dropShadow.localScale = fireEmitter.transform.localScale * 0.35f;
    }

    private void burnUpdate() {
        if (!GameController.IsPaused()) {
            SetFlameSize(newFlameSize - Time.deltaTime * flameBurnrate);
        }
    }

    public void SetFlameSize(float size) {
        if(size < 1) {
            Destroy(gameObject);
        }
        float dif = size - newFlameSize;
        flameSize += 0.1f * dif;
        newFlameSize = size;
    }

    public float GetFlameSize() {
        return newFlameSize;
    }

}
