using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public static bool paused;
    public static bool IsPaused() {
        return paused;
    }
    public static void SetPaused(bool paused) {
        GameController.paused = paused;
    }

    [SerializeField] private GameObject FlamePrefab = null;
    [SerializeField] private GameObject AlpacaPrefab = null;

    [SerializeField] private GameObject CamContainer = null;
    [SerializeField] private GameObject MainFlame = null;
    [SerializeField] private DialogControler dialog = null;
    [SerializeField] private Vector2 bounds = new Vector2(10, 5);
    [SerializeField] private float FlameSpawnTime = 5;
    [SerializeField] private Vector2 FlameSpawnStrength = new Vector2(15, 50);

    [SerializeField] private bool buildQuests = true;
    private float spawnTimer;

    private List<QuestStep> quests;
    private int curQuestIndex;

    void Start() {
        quests = new List<QuestStep>();

        if (buildQuests) {
            buildQuestLine();
            quests[0].Init.Invoke();
        }
    }

    private void buildQuestLine() {


        List<GameObject> pendingObjects = new List<GameObject>();
        FlameSpawnTime = 0;
        quests.Add(generateDialogQuest("League_normal", "Welcome Summoner!"));
        quests.Add(generateDialogQuest("Flame_normal", "What?"));
        quests.Add(generateDialogQuest("Flame_angry", "Is this Legends of League?"));
        quests.Add(generateDialogQuest("League_normal", "Actually no, but..."));
        quests.Add(generateWaitQuest(0.5f));
        quests.Add(generateDialogQuest("League_normal", "Welcome to Summoner's Rift!"));
        quests.Add(generateDialogQuest("Flame_normal", "Oh, geeze! Well, then...\nHere we go... I guess..."));
        quests.Add(generateDialogQuest("League_normal", "Okay then, let's start with the basics:"));

        quests.Add(new QuestStep(
            delegate () {
                pendingObjects.Add(spawnFlame(new Vector3(-3, 0, 0), 40));
                pendingObjects.Add(spawnFlame(new Vector3( 3, 0, 0), 40));
                pendingObjects.Add(spawnFlame(new Vector3(0, -2, 0), 40));
                startNextQuest();
            },
            delegate () { },
            delegate () { }
            ));

        quests.Add(generateWaitQuest(1.0f));
        quests.Add(generateDialogQuest("League_normal", "I spawned some minor Flames for you!\nYou should get them while they're hot!"));
        quests.Add(generateDialogQuest("League_normal", "You can lure nearby flames with your left mouse button.\nTry to get them to the big one in the middle."));
        quests.Add(generateWaitAllDedQuest(pendingObjects));
        quests.Add(generateDialogQuest("League_normal", "Good Job!"));
        quests.Add(generateDialogQuest("League_normal", "The little ones will burn out very fast, if you leaven them in the wild.\nYou should better collect them before they are gone."));
        quests.Add(generateWaitQuest(0.5f));
        quests.Add(generateDialogQuest("League_normal", "Now let me introduce you to your new little friend:"));

        quests.Add(new QuestStep(
            delegate () {
                pendingObjects.Clear();
                GameObject azatron = spawnAlpaca(Vector3.left * 5);
                AlpacaController c = azatron.GetComponent<AlpacaController>();
                c.spitDistance = 0;
                c.walkSpeed = 0;
                pendingObjects.Add(azatron);
                startNextQuest();
            },
            delegate () { },
            delegate () { }
            ));

        quests.Add(generateWaitQuest(1.0f));
        quests.Add(generateDialogQuest("Flame_cute", "Awwwwwww, he's cute.\nI will call him..."));
        quests.Add(generateWaitQuest(1.0f));
        quests.Add(generateDialogQuest("Flame_happy", "Azatron, Destroyer of Worlds!"));
        quests.Add(generateDialogQuest("League_normal", "...\nExcuse me?"));
        quests.Add(generateDialogQuest("League_normal", "Well... doesn't matter, he will die soon enough."));
        quests.Add(generateDialogQuest("League_normal", "Actualy, now is a good time.\nTry to use your right mouse button while near a flame and kill him."));
        quests.Add(generateDialogQuest("Flame_normal", "Yikes!"));
        quests.Add(generateWaitAllDedQuest(pendingObjects));
        quests.Add(generateDialogQuest("Flame_cute", "Goodbye pal :("));
        quests.Add(generateDialogQuest("League_normal", "Seems like you have a talent for killing innocent creatures.\nThis wasn't your first time, was it?"));
        quests.Add(generateDialogQuest("Flame_cute", "Well..."));
        quests.Add(generateDialogQuest("League_normal", "Anyway, let's keep going."));
        quests.Add(generateDialogQuest("League_normal", "You can move your camera a little bit around the room with WASD.Try it out!"));
        quests.Add(generateWaitQuest(6.0f, false));
        quests.Add(generateDialogQuest("League_normal", "Get back here! We are not done yet.\nThere is still so much more to learn!"));
        quests.Add(generateCameraPosQuest(Vector3.zero));
        quests.Add(generateWaitQuest(0.5f));
        quests.Add(generateDialogQuest("League_normal", "The next thing you should know is:\nDo not run while holding a scissors\n...or just don't run in general."));
        quests.Add(generateDialogQuest("League_normal", "Your flames will loose power if they're too fast."));
        quests.Add(generateCameraPosQuest(Vector3.left * 10));

        List<GameObject> killLater = new List<GameObject>();
        quests.Add(new QuestStep(
            delegate () {
                killLater.Clear();
                pendingObjects.Clear();
                GameObject parent = new GameObject();
                GameObject f = spawnFlame(Vector3.left * 15, 15, parent.transform);
                
                f.GetComponent<FlameControler>().flameBurnrate = 0;
                killLater.Add(parent);
                pendingObjects.Add(f);
                startNextQuest();
            },
            delegate () { },
            delegate () { }
            ));
        quests.Add(generateDialogQuest("League_normal", "Now try to bring this pal home, before he extinguishes!"));
        quests.Add(generateWaitAllDedQuest(pendingObjects));
        quests.Add(generateDialogQuest("League_normal", "GG\nYou did a great job here.."));
        quests.Add(new QuestStep(
            delegate () {
                foreach(GameObject o in killLater) {
                    Destroy(o);
                }
                startNextQuest();
            },
            delegate () { },
            delegate () { }
            ));
        quests.Add(generateCameraPosQuest(Vector3.zero));
        quests.Add(generateDialogQuest("League_normal", "Well... whatever."));
        quests.Add(generateWaitQuest(0.5f));
        quests.Add(generateDialogQuest("League_normal", "Congratulations\nYou have completed the Tutorial\nNOT YET!!!!!", 10));
        quests.Add(generateWaitQuest(0.5f));
        quests.Add(generateDialogQuest("League_normal", "Just kidding. you did. \nBut you still have to deal with me a little longer,.\nbecause I'm trying to KEEP OUR FRIENDSHIP ALIVE here."));
        quests.Add(generateDialogQuest("League_normal", "But I will let you have a little bit of fun now."));
        quests.Add(generateDialogQuest("League_angry", "Time for Round One"));
        quests.Add(buildWave1(pendingObjects));
        quests.Add(generateWaitQuest(1.0f));
        quests.Add(generateDialogQuest("League_angry", "KILL 'EM ALL!"));
        quests.Add(generateWaitAllDedQuest(pendingObjects));
        quests.Add(generateDialogQuest("League_normal", "And another one!"));
        quests.Add(generateDialogQuest("League_angry", "Round Two"));
        quests.Add(buildWave2(pendingObjects));
        quests.Add(generateWaitAllDedQuest(pendingObjects));
        quests.Add(generateDialogQuest("League_normal", "Good Job pal!\nLooks like you've got the hang of it!"));
        quests.Add(generateDialogQuest("League_normal", "Okay, now try to reach a power level of OVER 250"));
        quests.Add(buildWave3(pendingObjects));
        quests.Add(new QuestStep(
            delegate () {
                foreach (GameObject o in pendingObjects) {
                    Destroy(o);
                }
                startNextQuest();
            },
            delegate () { },
            delegate () { }
            ));
        quests.Add(generateDialogQuest("League_normal", "You did it! I'm so proud of you son.\nThat was stressful, wasn't it?"));
        quests.Add(generateDialogQuest("Flame_cute", "Yeah... maybe let's slow down for a bit now.."));
        quests.Add(generateDialogQuest("League_normal", "Alright, let's have a short break."));
        quests.Add(generateDialogQuest("League_normal", "Did you notice the red circles?\nThey indicate nearby enemys, so be careful when you see them."));
        quests.Add(generateWaitQuest(0.5f));
        quests.Add(generateDialogQuest("League_normal", "So.. are you refreshed now?\nCan we start again?"));
        quests.Add(generateDialogQuest("Flame_normal", "I think so.. but let's have a chill round now."));
        quests.Add(generateDialogQuest("League_normal", "Everything for you, my dear Friend!"));
        quests.Add(generateDialogQuest("League_normal", "Let's start the slow Round!"));
        quests.Add(generateDialogQuest("Morty", "Ah geeze Richard, where are we?"));
        quests.Add(generateDialogQuest("Rick", "I don't care Mortimer, I just had to get away from you Dad."));
        quests.Add(generateDialogQuest("Morty", "Hey, what is this button?"));
        quests.Add(generateWaitQuest(0.5f));
        quests.Add(generateDialogQuest("Morty", "FULL ALPACA MAYHEM", 10));
        quests.Add(generateWaitQuest(0.5f));
        quests.Add(new QuestStep(
            delegate () {
                pendingObjects.Clear();
                pendingObjects.Add(spawnAlpaca(new Vector3(-10, 5)));
                pendingObjects.Add(spawnAlpaca(new Vector3(-10, -5)));
                pendingObjects.Add(spawnAlpaca(new Vector3(10, -5)));
                pendingObjects.Add(spawnAlpaca(new Vector3(10, 5)));

                pendingObjects.Add(spawnAlpaca(new Vector3(0, 15)));
                pendingObjects.Add(spawnAlpaca(new Vector3(0,-15)));

                pendingObjects.Add(spawnAlpaca(new Vector3(-18, 10)));
                pendingObjects.Add(spawnAlpaca(new Vector3(-18,-10)));
                pendingObjects.Add(spawnAlpaca(new Vector3( 18,-10)));
                pendingObjects.Add(spawnAlpaca(new Vector3( 18, 10)));
                startNextQuest();
            },
            delegate () {
            },
            delegate () { }
            ));
        quests.Add(generateWaitQuest(1.0f));
        quests.Add(generateDialogQuest("Morty", "Mh.. seems like nothing happend."));
        quests.Add(generateDialogQuest("Rick", "Come on Mortimer, we have to go now."));
        quests.Add(generateDialogQuest("Morty", "Alright alright.."));
        quests.Add(generateWaitQuest(1.0f));
        quests.Add(generateDialogQuest("Flame_normal", "What the hell was that?"));
        quests.Add(generateDialogQuest("League_normal", "Hmm?\nSorry I wasn't paying attention."));
        quests.Add(generateDialogQuest("League_normal", "I was looking for some toilet paper on amadson.\nBy the way, we need a power level of 500 now"));
        quests.Add(generateDialogQuest("League_normal", "You should better give your best"));
        quests.Add(buildWave4(pendingObjects));
        quests.Add(new QuestStep(
            delegate () {
                foreach (GameObject o in pendingObjects) {
                    Destroy(o);
                }
                startNextQuest();
            },
            delegate () { },
            delegate () { }
            ));
        quests.Add(generateDialogQuest("League_normal", "That was awesome!!\nYou are soo good!"));
        quests.Add(generateDialogQuest("League_normal", "I feel like our friendship can go through all hardships now.\nWe are the ultimate Team now."));
        quests.Add(generateDialogQuest("Flame_normal", "You're a super weird fella..\nBut I guess we kept the fire of our friendship alive!"));
        quests.Add(generateDialogQuest("League_normal", "Yeah, It was nice knowing you\nbut it seems as our adventure is now over."));
        quests.Add(generateDialogQuest("League_normal", "We will meet again, my dear friend!"));
        quests.Add(generateWaitQuest(3.0f));
        quests.Add(new QuestStep(
            delegate () {
                SceneManager.LoadScene(2);
            },
            delegate () { },
            delegate () { }
            ));

    }
    private QuestStep buildWave1(List<GameObject> wave) {
        return new QuestStep(
            delegate () {
                wave.Clear();
                float x = 9;
                float y = 4.5f;
                spawnFlame(new Vector3(-x, y), 30);
                spawnFlame(new Vector3(x, y), 30);
                spawnFlame(new Vector3(x, -y), 30);
                spawnFlame(new Vector3(-x, -y), 30);
                wave.Add(spawnAlpaca(new Vector3(-x, 0)));
                wave.Add(spawnAlpaca(new Vector3(x, 0)));
                FlameSpawnTime = 3;
                FlameSpawnStrength = new Vector2(15, 50);
                startNextQuest();
            },
            delegate () {
            },
            delegate () { }
            );
    }
    private QuestStep buildWave2(List<GameObject> wave) {
        return new QuestStep(
            delegate () {
                wave.Clear();
                float x = 12;
                float y = 4.5f;
                wave.Add(spawnAlpaca(new Vector3(-8, -2)));
                wave.Add(spawnAlpaca(new Vector3(8, -2)));
                wave.Add(spawnAlpaca(new Vector3(-8, 2)));
                wave.Add(spawnAlpaca(new Vector3(8, 2)));
                wave.Add(spawnAlpaca(new Vector3(-4, -12)));
                //wave.Add(spawnAlpaca(new Vector3(4, -12)));
                //wave.Add(spawnAlpaca(new Vector3(-4, 12)));
                wave.Add(spawnAlpaca(new Vector3(4, 12)));



                startNextQuest();
            },
            delegate () {
            },
            delegate () { }
            );
    }
    private QuestStep buildWave3(List<GameObject> wave) {
        FlameControler main = MainFlame.GetComponent<FlameControler>();
        float timer = 0;
        float enemySpawnTime = 8;
        return new QuestStep(
            delegate () {
                wave.Clear();
                float x = 12;
                float y = 4.5f;
                wave.Add(spawnAlpaca(new Vector3(-8, 2)));
                wave.Add(spawnAlpaca(new Vector3(8, 2)));
                wave.Add(spawnAlpaca(new Vector3(5, -12)));
                wave.Add(spawnAlpaca(new Vector3(-5, 12)));

                FlameSpawnTime = 1.5f;
                FlameSpawnStrength = new Vector2(30, 75);
            },
            delegate () {
                if (main.GetFlameSize() > 250) {
                    startNextQuest();
                }
                timer += Time.deltaTime;
                if (timer > enemySpawnTime) {
                    timer -= enemySpawnTime;
                    Vector3 pos = getRandomLocationInBounds() * 0.5f;
                    if (pos.magnitude < 7) {
                        pos += pos.normalized * (7 - pos.magnitude);
                    }
                    wave.Add(spawnAlpaca(pos));
                }
            },
            delegate () { }
            );
    }
    private QuestStep buildWave4(List<GameObject> wave) {
        FlameControler main = MainFlame.GetComponent<FlameControler>();
        float timer = 0;
        float enemySpawnTime = 4;
        return new QuestStep(
            delegate () {
                wave.Clear();
                float x = 12;
                float y = 4.5f;
                wave.Add(spawnAlpaca(new Vector3(-8, 2)));
                wave.Add(spawnAlpaca(new Vector3(8, 2)));
                wave.Add(spawnAlpaca(new Vector3(5, -12)));
                wave.Add(spawnAlpaca(new Vector3(-5, 12)));

                FlameSpawnTime = 0.9f;
                FlameSpawnStrength = new Vector2(60, 100);
            },
            delegate () {
                if (main.GetFlameSize() > 500) {
                    startNextQuest();
                }
                timer += Time.deltaTime;
                if (timer > enemySpawnTime) {
                    timer -= enemySpawnTime;
                    Vector3 pos = getRandomLocationInBounds() * 0.5f;
                    if (pos.magnitude < 7) {
                        pos += pos.normalized * (7 - pos.magnitude);
                    }
                    wave.Add(spawnAlpaca(pos));
                }
            },
            delegate () { }
            );
    }

    private QuestStep generateWaitAllDedQuest(List<GameObject> hasToDie) {
        return new QuestStep(
            delegate () { },
            delegate () {
                bool allded = true;
                foreach (GameObject a in hasToDie) {
                    if (a != null) allded = false;
                }
                if (allded) {
                    startNextQuest();
                }
            },
            delegate () { }
            );
    }

    private QuestStep generateCameraPosQuest(Vector3 pos) {
        return new QuestStep(
            delegate () {
                CamContainer.transform.position = pos;
                startNextQuest();
            },
            delegate () { },
            delegate () { }
            );
    }

    private QuestStep generateWaitQuest(float duration, bool pausing = true) {
        float waitTimer = duration;
        return new QuestStep(
            delegate () {  if(pausing) paused = true; },
            delegate () {
                waitTimer -= Time.deltaTime;
                if (waitTimer < 0) {
                    startNextQuest();
                }
            },
            delegate () { if (pausing) paused = false; }
            );
    }

    private QuestStep generateDialogQuest(string profile, string script, float dialogSpeed = -1) {
        float oldSpeed = dialog.CharacterPerSecond;
        return new QuestStep(
            delegate () {
                if(dialogSpeed != -1) {
                    oldSpeed = dialog.CharacterPerSecond;
                    dialog.CharacterPerSecond = dialogSpeed;
                }
                dialog.StartDialog(profile, script);
            },
            delegate () {
                if (!dialog.IsDialogRunning()) {
                    startNextQuest();
                }
            },
            delegate () {
                if (dialogSpeed != -1) {
                    dialog.CharacterPerSecond = oldSpeed;
                }
            }
            ); ;
    }

    private QuestStep generateKillQuest() {
        List<GameObject> alpacas = new List<GameObject>();
        return new QuestStep(
            delegate () {
                alpacas.Add(spawnAlpaca(getRandomLocationInBounds()));
                
            },
            delegate () {
                bool allded = true;
                foreach(GameObject a in alpacas) {
                    if (a != null) allded = false;
                }
                if (allded) {
                    startNextQuest();
                }
            },
            delegate () {
            }
            );
    }

    private Vector3 getRandomLocationInBounds() {
        return new Vector2(Random.Range(-bounds.x, bounds.x), Random.Range(-bounds.y, bounds.y));
    }

    private GameObject spawnAlpaca(Vector3 pos) {
        GameObject alp = Instantiate(AlpacaPrefab);
        alp.transform.position = pos;
        return alp;
    }

    // Update is called once per frame
    void Update() {
        spawnUpdate();
        questUpdate();
        if (MainFlame == null) {
            GameOver();
        }
    }

    private void questUpdate() {
        if(curQuestIndex < quests.Count) {
            quests[curQuestIndex].Step.Invoke();
        }

    }

    private void startNextQuest() {
        quests[curQuestIndex].End.Invoke();
        curQuestIndex++;
        if(curQuestIndex < quests.Count) {
            quests[curQuestIndex].Init.Invoke();
        }
    }

    private void spawnUpdate() {
        if (!GameController.IsPaused()) {
            if (FlameSpawnTime > 0) {
                spawnTimer += Time.deltaTime;
                if (spawnTimer > FlameSpawnTime) {
                    spawnTimer -= FlameSpawnTime;


                    float flamePower = Random.Range(FlameSpawnStrength.x, FlameSpawnStrength.y);
                    Vector2 spawnPos = getRandomLocationInBounds();
                    spawnFlame(spawnPos, flamePower);

                }
            }
        }
    }

    bool foobar = false;
    private void GameOver() {
        if (!foobar) {
            foobar = true;
            quests.Clear();
            curQuestIndex = 0;
            quests.Add(generateCameraPosQuest(Vector3.zero));
            quests.Add(generateDialogQuest("Flame_cute", "Mr. Strong...\nI don't feel so well...\nI don't wanna die...", 15));
            quests.Add(generateDialogQuest("League_normal", "MY FRIEND IS DEAD!!\nHE'S DEAD JIM >.<"));
            quests.Add(new QuestStep(
                delegate () {
                    SceneManager.LoadScene(3);
                    Debug.Log("Loading ded");
                },
                delegate () { },
                delegate () { }
                ));
            quests.Add(generateDialogQuest("League_normal", "END ._."));
            quests[0].Init.Invoke();
        }
    }

    private GameObject spawnFlame(Vector3 spawnPos, float flamePower, Transform parent = null, float burnRate = -1) {
        GameObject flame = Instantiate(FlamePrefab, parent);
        flame.transform.position = spawnPos;
        FlameControler fl = flame.GetComponent<FlameControler>();
        fl.SetFlameSize(flamePower);
        if(burnRate >= 0) {
            fl.flameBurnrate = burnRate;
        }
        return flame;
    }

    private class QuestStep {
        public delegate void func();
        public QuestStep(func init, func step, func end) {
            this.Init = init; this.Step = step; this.End = end;;
        }

        public func Init;
        public func Step;
        public func End;

    }

}
