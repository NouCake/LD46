using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DialogControler : MonoBehaviour {

    private const string ProfilePath = "Profiles/";

    [SerializeField] private Image ImageCursor;
    [SerializeField] private Image ImageProfile;
    [SerializeField] private TextMeshProUGUI TextDialog;
    [SerializeField] private GameObject DialogContainer;
    [Space(15)]
    [SerializeField] public float CharacterPerSecond = 5;
    private float spc;

    private bool dialogRunning = false;
    private bool waitForInput = false;

    private string curDialogScript;
    private int curDialogPos;

    public float curCharTimer;

    private void Awake() {
        if (ImageProfile == null) Debug.LogError("UIImage Objekt for Profile is not set");
        if (TextDialog == null) Debug.LogError("TMPro Objekt for Dialog is not set");

        spc = 1.0f / CharacterPerSecond;
        DialogContainer.SetActive(false);
        ImageCursor.gameObject.SetActive(false);
    }

    private void Start() {
    }

    private void Update() {
        if (dialogRunning) {

            if (waitForInput) {
                if (Input.GetKeyDown(KeyCode.Return)) {
                    waitForInput = false;
                    ImageCursor.gameObject.SetActive(false);
                }
            } else {
                stepCharTimer();
            }
        }
    }

    private void showNextChar() {
        curDialogPos++;

        if (curDialogPos > curDialogScript.Length) {
            //End Dialog
            dialogRunning = false;
            DialogContainer.SetActive(false);
            ImageCursor.gameObject.SetActive(false);
            GameController.SetPaused(false);
        } else {
            if (curDialogPos == curDialogScript.Length || curDialogScript[curDialogPos] == '\n') {
                waitForInput = true;
                ImageCursor.gameObject.SetActive(true);
                curCharTimer = spc;
            }
            if (curDialogPos <= curDialogScript.Length) {
                TextDialog.text = curDialogScript.Substring(0, curDialogPos);
            }
        }


    }

    private void stepCharTimer() {
        while(curCharTimer < 0) {
            curCharTimer += spc;
            showNextChar();
        }
        curCharTimer -= Time.deltaTime;
    }

    public void StartDialog(string profile, string dialogScript) {
        if (!dialogRunning) {
            TextDialog.text = "";
            spc = 1.0f / CharacterPerSecond;
            setProfile(profile);
            curDialogScript = dialogScript;
            dialogRunning = true;
            curDialogPos = 0;
            DialogContainer.SetActive(true);
            //GameController.SetPaused(true);
        }
    }

    private void setProfile(string path) {
        Sprite spr = Resources.Load<Sprite>(ProfilePath + path);
        ImageProfile.sprite = spr;
    }

    public bool IsDialogRunning() {
        return dialogRunning;
    }

}
