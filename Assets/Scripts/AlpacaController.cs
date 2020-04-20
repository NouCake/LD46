using DragonBones;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlpacaController : MonoBehaviour {

    //Requirements: [0]walk1, [1]walk2, [2]spit
    [SerializeField] private Sprite[] frames = { };
    [SerializeField] private GameObject spitPrefab;

    //Behaviour
    [SerializeField] public float spitDistance = 4;
    [SerializeField] public float walkSpeed = 2;
    [SerializeField] private float spitSpeed = 0.5f; //obsolete
    [SerializeField] private Vector3 spitOffset = new Vector3(0.85f, 0.5f, 0);

    private float spitTimer;

    private UnityArmatureComponent anim;
    private string curAnimationKey = "";

    private void Awake() {
        anim = transform.Find("Armature").GetComponent<UnityArmatureComponent>();
        anim.AddEventListener(EventObject.FRAME_EVENT, delegate (string type, EventObject eventObject) {
            if (curAnimationKey == "spit") {
                spawnSpit();
            }
        });
        anim.AddEventListener(EventObject.COMPLETE, delegate (string type, EventObject eventObject) {
            if (curAnimationKey == "dead") {
                Destroy(gameObject);
            }
        });
    }

    private void Start() {
        UIController ui = UIController.GetInstance();
        ui.AddTargetIndicator(Color.red, transform, 5);
    }

    private bool ded = false;
    public void Kill() {
        if (!ded) {
            ded = true;
        }
    }

    void Update() {
        if (!ded) {
            Vector3 dist = distToMid();

            if (dist.x < 0) {
                transform.localScale = new Vector3(-1, 1, 1);
            } else {
                transform.localScale = Vector3.one;
            }

            if (dist.magnitude > spitDistance) {
                walk(dist.normalized);
            } else {
                spit();
            }
        } else {
            PlayAnimation("dead", 1);
        }

    }

    private void PlayAnimation(string key, int playTimes = -1) {
        if(key == "spit") {
            anim.animation.timeScale = spitSpeed;
        } else {
            anim.animation.timeScale = 1;
        }
        if (curAnimationKey != key) {
            curAnimationKey = key;
            anim.animation.Play(key, playTimes);
        }
    }

    private void walk(Vector3 dir) {
        if (!GameController.IsPaused() && walkSpeed > 0) {
            PlayAnimation("walk");
            transform.position += Time.deltaTime * dir * walkSpeed;
        } else {
            PlayAnimation("idle");
        }
    }

    private void spit() {
        PlayAnimation("spit");
    }

    private void spawnSpit() {
        Vector3 dir = distToMid().normalized;
        GameObject spit = Instantiate(spitPrefab);
        spit.transform.position = transform.position + new Vector3(spitOffset.x * transform.localScale.x, spitOffset.y, spitOffset.z);
        spit.GetComponent<SpitController>().Shoot(Vector3.up * 0.5f);
    }

    private Vector3 distToMid() {
        return (Vector3.up * 0.5f)-transform.position;
    }

}
