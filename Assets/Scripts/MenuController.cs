using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    enum Mode {
        FADE_IN,
        FADE_OUT,
        WAIT,
        END
    }
    private Mode mode;

    private delegate void Step();
    private List<Step> steps;
    public int curStep;

    [SerializeField] private Image borderImage;
    [SerializeField] private Sprite border1;
    [SerializeField] private Sprite border2;
    [SerializeField] private float borderSwapTime = 0.5f;
    private float borderTimer;

    [SerializeField] private Image display;
    [SerializeField] private Sprite presents1;
    [SerializeField] private Sprite presents2;
    [SerializeField] private Sprite logo1;
    [SerializeField] private Sprite logo2;

    private Sprite curImg;
    private Sprite curAlt;

    private void Awake() {
        Screen.SetResolution(1280, 720, false);
    }

    void Start() {
        steps = new List<Step>();

        WaitIdle(1.0f);
        steps.Add(delegate () { FadeInImage(presents1, presents2, 1.0f); });
        steps.Add(delegate () { WaitIdle(2.0f); });
        steps.Add(delegate () { FadeInImage(logo1, logo2, 0.1f); });
        steps.Add(delegate () { WaitIdle(5.0f); });
        steps.Add(delegate () { FadeOutImage(1.0f); });
        steps.Add(delegate () { WaitIdle(1.0f); });
        steps.Add(delegate () { SceneManager.LoadScene(1); });
    }

    private float fadeTimer;
    private float fadeTime;
    void Update() {
        alternateBorder();
        if (mode != Mode.END) {
            stepTimer();
            executeMode();
        }
    }

    private int curBorder = 0;
    private void alternateBorder() {
        borderTimer -= Time.deltaTime;
        if(borderTimer < 0) {
            borderTimer += borderSwapTime;
            if (curBorder % 2 ==  0) {
                borderImage.sprite = border1;
                display.sprite = curImg;
            } else {
                borderImage.sprite = border2;
                display.sprite = curAlt;
            }
            curBorder++;
        }
    }

    private void stepTimer() {

        if (fadeTimer > 0) {
            fadeTimer -= Time.deltaTime;
        } else {
            if (curStep >= steps.Count) {
                mode = Mode.END;
            } else {
                steps[curStep].Invoke();
            }
            curStep++;
        }
    }

    private void executeMode() {
        Debug.Log(mode);
        switch (mode) {
            case Mode.FADE_IN:
                float key = Mathf.Clamp(fadeTimer / fadeTime, 0, 1);
                display.color = new Color(1, 1, 1, key);
                break;
            case Mode.FADE_OUT:
                key = 1 - Mathf.Clamp(fadeTimer / fadeTime, 0, 1);
                display.color = new Color(1, 1, 1, key);
                break;
        }
    }

    private void WaitIdle(float duration) {
        mode = Mode.WAIT;
        fadeTimer = duration;
    }

    private void FadeOutImage(float fadeDuration) {
        mode = Mode.FADE_IN;
        display.color = new Color(1, 1, 1, 0);
        fadeTimer = fadeDuration;
        fadeTime = fadeDuration;
    }

    private void FadeInImage(Sprite img, Sprite alt, float fadeDuration) {
        curImg = img;
        curAlt = alt;
        mode = Mode.FADE_OUT;
        display.color = new Color(1, 1, 1, 1);
        fadeTimer = fadeDuration;
        fadeTime = fadeDuration;
        display.sprite = img;
    }

}
