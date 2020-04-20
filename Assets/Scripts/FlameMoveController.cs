using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FlameMoveController : MonoBehaviour {

    private static GameObject FlamePrefab;
    private static GameObject FireballPrefab;
    private static void LoadPrefabs() {
        if (FlamePrefab == null) {
            FlamePrefab = Resources.Load<GameObject>("Fire");
            if (FlamePrefab == null) Debug.Log("Could not load");
        }
        if (FireballPrefab == null) {
            FireballPrefab = Resources.Load<GameObject>("Fireball");
        }
    }

    [SerializeField] private bool isStatic = false;
    [SerializeField] private float fireballCost = 4;

    [SerializeField] private float followDistance = 5;
    [SerializeField] private float followAccel = 4;

    private float mergeTimer;
    private bool noMerge = false;

    private float stunTimer;

    private SpriteRenderer dropShadow;
    private FlameControler flameControler;
    private Vector3 speed;


    [SerializeField] private float splitRecoverTime = 0.5f;
    private float splitTimer;

    private void Awake() {
        LoadPrefabs();
        flameControler = GetComponent<FlameControler>();
        dropShadow = transform.Find("DropShadow").GetComponent<SpriteRenderer>();
    }

    private void Start() {
    }

    private void Update() {
        if (!GameController.IsPaused()) {
            dropShadow.color = new Color(1, 1, 1, isSelected() ? 1.0f : 0.75f);

            if (!isStunned()) {
                updateSplit();
                moveUpdate();
                mergeUpdate();
                fireballUpdate();
            } else {
                stunTimer -= Time.deltaTime;
            }
        }
    }

    private void fireballUpdate() {
        Vector3 dist = distToMouse();
        if (Input.GetMouseButtonDown(1) && dist.magnitude < followDistance) {
            float myFlame = flameControler.GetFlameSize();
            if (myFlame <= fireballCost) {
                Destroy(gameObject);
            }
            flameControler.SetFlameSize(myFlame - fireballCost);
            GameObject fireball = Instantiate(FireballPrefab);
            fireball.transform.position = transform.position + Vector3.up * 0.5f;
            fireball.GetComponent<FireballController>().Shoot(dist.normalized);
        }
    }

    private bool isSelected() {
        Vector3 dist = distToMouse();
        return dist.magnitude < followDistance;
    }

    public void Stun(float duration) {
        stunTimer = duration;
        speed = Vector3.zero;
    }

    private bool isStunned() {
        return stunTimer > 0;
    }

    private void moveUpdate() {
        if (!isStatic) {
            Vector3 dist = distToMouse();


            if (Input.GetMouseButton(0) && dist.magnitude < followDistance) {
                speed += dist.normalized * Time.deltaTime * followAccel;
            } else {
                if (speed.magnitude > 0.25f) {
                    speed -= (speed.normalized + speed) * Time.deltaTime;
                } else {
                    speed = Vector3.zero;
                }
            }

            Vector3 newPos = transform.position + speed * Time.deltaTime;

            if (!hitsWall(newPos)) {
                transform.position = newPos;
            }
        }
    }

    private bool hitsWall(Vector2 pos) {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, 0.5f);
        foreach(Collider2D c in colliders) {
            //Debug.Log("Colliding with " + colliders.Length);
            if (c.tag == "Terrain") {
                return true;
            }
        }


        return false;
    }

    private void mergeUpdate() {
        if (noMerge) {
            if(mergeTimer > 0) {
                mergeTimer -= Time.deltaTime;
            } else {
                noMerge = false;
            }
        }
    }

    private void updateSplit() {
        if(splitTimer >= 0) {
            splitTimer -= Time.deltaTime;
        } else {
            if (speed.magnitude > 2 + Mathf.Pow(flameControler.GetFlameSize(), 1.0f / 3.0f)) {
                splitFlame();
                splitTimer = splitRecoverTime; ;
            }
        }
    }
    
    private GameObject splitFlame() {
        float myFlame = flameControler.GetFlameSize();
        if (myFlame > 2) {
            GameObject splitter = Instantiate(FlamePrefab, transform.parent);
            splitter.transform.position = transform.position;
            splitter.GetComponent<FlameMoveController>().Stun(1.0f);
            splitter.GetComponent<FlameControler>().SetFlameSize(myFlame * 0.5f);
            flameControler.SetFlameSize(myFlame * 0.75f);
            return splitter;
        } else {
            Destroy(gameObject);
            return null;
        }
        //speed = speed * 0.5f;
    }

    private Vector2 distToMouse() {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if(collision.tag == "Fire") {
            FlameMoveController other = collision.GetComponent<FlameMoveController>();
            if (canMerge() && other.canMerge() &&
                    (isStatic || flameControler.GetFlameSize() >= other.GetComponent<FlameControler>().GetFlameSize()) &&
                    !other.isStatic) {
                other.avoidMerge();
                flameControler.SetFlameSize(flameControler.GetFlameSize() + collision.GetComponent<FlameControler>().GetFlameSize() * 0.5f);
                Destroy(collision.gameObject);
            }
        }
    }

    public bool canMerge() {
        return !noMerge && splitTimer < 0;
    }

    public void avoidMerge() {
        noMerge = true;
        mergeTimer = 0.25f;
    }

}
