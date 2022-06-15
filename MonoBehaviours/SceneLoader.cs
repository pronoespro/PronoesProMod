using PronoesProMod.MonoBehaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace PronoesProMod
{
    public class SceneLoader:MonoBehaviour
    {

        string scene;

        public string[] entranceGOs = new string []{ "Portal", "GroundObjs", "Ground","crates_d", "Background", "Spikes_spk" };
        public string[] townGOs = new string[] { "TownGround","Decorations", "crates_d", "NoBounce_nb", "Spikes_spk","Lamps_l", "Basket" };
        public string hitParticlesName = "Bounce_hit",spikePariclesName = "Spike_hit",successParticlesName="Success";
        public ParticleSystem hitParticles,spikeParticles,successParticles;
        public SceneManager scenesss;

        public Dictionary<string, int> appleAmmountsPerScene = new Dictionary<string, int>() { { "CreatedTown", 13 } };

        public void Start()
        {
            On.GameManager.EnterHero += GameManager_EnterHero;
            On.SceneManager.Start += SceneManager_Start;
        }

        public static void DreamTransition(string toScene,string entry)
        {
            GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
            {
                SceneName = toScene,
                EntryGateName = entry,
                Visualization=GameManager.SceneLoadVisualizations.Dream,
                WaitForSceneTransitionCameraFade=false
            }) ;
        }

        private void GameManager_EnterHero(On.GameManager.orig_EnterHero orig, GameManager self, bool additiveGateSearch)
        {
            scene = self.sceneName;
            if (self.sceneName == "Tutorial_01")
            {
                CreateGateway("left", new Vector2(36.5f, 17f), new Vector2(17f, 2f), "entrance", "left", false, true, GameManager.SceneLoadVisualizations.Dream);

                if (PronoesProMod.Instance.GOBundle.ContainsKey("portal"))
                {
                    AssetBundle ab = PronoesProMod.Instance.GOBundle["portal"];
                    GameObject go = Instantiate(ab.LoadAsset<GameObject>("Portal"));
                    go.SetActive(PlayerData.instance.hasDreamNail);
                    go.transform.position = new Vector3(36.5f, 17f, 2f);
                    foreach (SpriteRenderer rend in go.GetComponentsInChildren<SpriteRenderer>())
                    {
                        rend.material = new Material(Shader.Find("Sprites/Default"));
                    }

                    ParticleSystem particles = go.transform.Find("DreamEntering").GetComponent<ParticleSystem>();

                    if (particles != null)
                    {
                        CreateDreamNailPortal(new Vector2(36.25f, 12.25f), new Vector2(4.1f, 4.5f), "entrance", "left", particles);
                    }
                    else
                    {
                        CreateDreamNailPortal(new Vector2(36.25f, 12.25f), new Vector2(4.1f, 4.5f), "entrance", "left");
                    }
                }
                else
                {
                    PronoesProMod.Instance.Log("Error loading Portal, try Portal 2 instead");
                }
            }
            else if (self.sceneName == "entrance")
            {
                LoadBlurPlane(new Vector3(30, 30, 7),new Vector3(17, 17, 17));

                CreateGateway("left", new Vector2(32, 22), new Vector2(2, 6), "Tutorial_01", "left", false, true, GameManager.SceneLoadVisualizations.Dream);
                CreateGateway("right", new Vector2(58, 15), new Vector2(1, 6), "CreatedTown", "left", true, false, GameManager.SceneLoadVisualizations.Default);

                GameObject portalGO = GameObject.Find("Portal");
                if (portalGO != null)
                {
                    PronoesProMod.Instance.Log("portal found");
                    ParticleSystem particles = portalGO.GetComponentInChildren<ParticleSystem>();
                    if (particles != null)
                    {
                        CreateDreamNailPortal(new Vector2(32, 22), new Vector2(4.1f, 4.5f), "Tutorial_01", "left", particles);
                    }
                    else
                    {
                        CreateDreamNailPortal(new Vector2(32, 22), new Vector2(4.1f, 4.5f), "Tutorial_01", "left");
                    }
                }
                else
                {
                    PronoesProMod.Instance.Log("portal not found");
                }

                LoadObjects(entranceGOs);

                if (PronoesProMod.Instance.GOBundle.ContainsKey("music"))
                {
                    AssetBundle ab = PronoesProMod.Instance.GOBundle["music"];
                    AudioClip clip = ab.LoadAsset<AudioClip>("entrance");
                    MusicChanger.PlayBackgroundMusicForScene(clip);
                }
                StartCoroutine(ShowTitle(0));
            }
            else if (self.sceneName == "CreatedTown")
            {
                LoadBlurPlane(new Vector3(420, 30, 7), new Vector3(100, 17, 17));
                CreateBench(new Vector3(78, 10.75f, 1), new Vector3(1, 1, 1), 0);

                CreateGateway("left", new Vector2(1, 16), new Vector2(4, 8), "entrance", "right", false, false, GameManager.SceneLoadVisualizations.Default);
                LoadCharacter("pronoespro", new Vector3(105,13.75f,1));
                
                LoadObjects(townGOs);

                if (PronoesProMod.Instance.GOBundle.ContainsKey("music"))
                {
                    AssetBundle ab = PronoesProMod.Instance.GOBundle["music"];
                    AudioClip clip = ab.LoadAsset<AudioClip>("town");
                    MusicChanger.PlayBackgroundMusicForScene(clip);
                }

                StartCoroutine(ShowTitle(1));
            }
            if (PronoesProMod.Instance != null && PronoesProMod.Instance.fadedIn)
            {
                PronoesProMod.Instance.CustomSceneFadeOutsis();
            }
            orig(self,additiveGateSearch);
        }

        public void LoadObjects(string[] objNames)
        {
            GameObject hitPart = GameObject.Find(hitParticlesName);
            if (hitPart != null){
                hitParticles = hitPart.GetComponentInChildren<ParticleSystem>();
            }
            hitPart = GameObject.Find(spikePariclesName);
            if (hitPart != null){
                spikeParticles = hitPart.GetComponentInChildren<ParticleSystem>();
            }
            hitPart = GameObject.Find(successParticlesName);
            if (hitPart != null)
            {
                successParticles = hitPart.GetComponentInChildren<ParticleSystem>();
            }

            for (int i = 0; i < objNames.Length; i++)
            {
                if (objNames[i] == "")
                {
                    continue;
                }
                Transform foundGO = GameObject.Find(objNames[i]).transform;
                if (foundGO != null)
                {
                    foreach (SpriteRenderer rend in foundGO.GetComponentsInChildren<SpriteRenderer>())
                    {
                        if (objNames[i].Contains("_Dif"))
                        {
                            rend.material = new Material(Shader.Find("Sprites/Diffuse"));
                        }
                        else
                        {
                            rend.material = new Material(Shader.Find("Sprites/Default"));
                        }
                    }
                    if (objNames[i].EndsWith("_hit"))
                    {
                        ParticleSystem particles = foundGO.GetComponent<ParticleSystem>();
                        if (particles != null)
                        {
                            hitParticles = particles;
                        }
                    }
                    if (objNames[i].Contains("_spk"))
                    {
                        for (int c = 0; c < foundGO.transform.childCount; c++)
                        {
                            foundGO.GetChild(c).gameObject.AddComponent<DestructibleProp>();
                            BouncingObject bounce = foundGO.GetChild(c).gameObject.AddComponent<BouncingObject>();
                            if (spikeParticles != null)
                            {
                                bounce.particles = spikeParticles;
                            }
                        }
                    }
                    if (objNames[i].Contains("_d"))
                    {
                        for (int c = 0; c < foundGO.transform.childCount; c++)
                        {
                            foundGO.GetChild(c).gameObject.AddComponent<DestructibleProp>();
                            BouncingObject bounce= foundGO.GetChild(c).gameObject.AddComponent<BouncingObject>();
                            if (hitParticles != null)
                            {
                                bounce.particles = hitParticles;
                            }
                            if (foundGO.GetChild(c).name.Contains("apple"))
                            {
                                bounce.OverrideDepth(0.05f);
                            }
                        }
                    }
                    if (objNames[i].Contains("_nb"))
                    {
                        for (int child = 0; child < foundGO.childCount; child++)
                        {
                            foundGO.GetChild(child).gameObject.AddComponent<NonBouncer>();
                        }
                    }
                    if (objNames[i].Contains("_l"))
                    {
                        for (int child = 0; child < foundGO.childCount; child++)
                        {
                            Lamp lamp= foundGO.GetChild(child).gameObject.AddComponent<Lamp>();
                            if (spikeParticles != null)
                            {
                                lamp.particles = spikeParticles;
                            }
                        }
                    }
                    if(objNames[i].Contains("Basket"))// && appleAmmountsPerScene.ContainsKey(scene))
                    {
                        AppleBasket basket= foundGO.gameObject.AddComponent<AppleBasket>();
                        basket.applesToReward = appleAmmountsPerScene[scene];
                        basket.correctParticles = successParticles;
                    }
                }
            }
        }

        public void LoadBlurPlane(Vector3 pos,Vector3 scale)
        {
            Transform t;
            if(PronoesProMod.Instance.preloadedObjs.ContainsKey("Tutorial_01") && PronoesProMod.Instance.preloadedObjs["Tutorial_01"].ContainsKey("BlurPlane (1)"))
            {
                t=GameObject.Instantiate(PronoesProMod.Instance.preloadedObjs["Tutorial_01"]["BlurPlane (1)"],pos,Quaternion.Euler(90,180,0)).transform;
                t.localScale = scale;
                t.gameObject.SetActive(true);
            }
        }

        public void CreateBench(Vector3 pos,Vector3 scale, float rotation)
        {
            Transform t;
            if(PronoesProMod.Instance.preloadedObjs.ContainsKey("Town") && PronoesProMod.Instance.preloadedObjs["Town"].ContainsKey("RestBench"))
            {
                t=GameObject.Instantiate(PronoesProMod.Instance.preloadedObjs["Town"]["RestBench"],pos,Quaternion.Euler(0,0,rotation)).transform;
                t.localScale = scale;
                t.gameObject.SetActive(true);
            }
        }

        public GameObject LoadCharacter(string type, Vector3 position)
        {

            if (PronoesProMod.Instance.NPCBundle.ContainsKey(type))
            {
                AssetBundle ab = PronoesProMod.Instance.NPCBundle[type];
                GameObject go = Instantiate(ab.LoadAsset<GameObject>("Character"));
                if (go != null)
                {
                    go.transform.SetPosition2D(position);
                }
                go.AddComponent<PronoNPC>();
                PronoCustomNPC customNPC= go.AddComponent<PronoCustomNPC>();

                customNPC.npcSuperName = "The lost apprentice";
                customNPC.npcName = "PronoesPro";
                customNPC.npcSubName= "";
                customNPC.conversation = new string[]
                {
                    "prono_welcome_0",
                    "prono_welcome_1",
                    "prono_welcome_2",
                    "prono_welcome_3",
                    "prono_welcome_4"
                };

                return go;
            }

            /*
            if (PronoesProMod.Instance.NPCBundle.ContainsKey(type))
            {
                AssetBundle ab = PronoesProMod.Instance.NPCBundle[type];
                GameObject go = Instantiate(ab.LoadAsset<GameObject>("Character"));
                if (go != null)
                {
                    go.transform.SetPosition2D(position);
                }
                go.AddComponent<PronoNPC>();

                AddInvisibleZote(go.transform.position + new Vector3(0, -2.25f),4f,1f);

                return go;
            }*/
            return null;
        }

        public void AddInvisibleZote(Vector3 position, float xScaleMult = 1, float yScaleMult = 1)
        {
            GameObject zote = GameObject.Instantiate(PronoesProMod.Instance.preloadedObjs["Town"]["_NPCs"]);
            Transform newZote = zote.transform.Find("Zote Final Scene");
            newZote = newZote.Find("Zote Final");
            newZote.parent = null;
            Destroy(zote);

            newZote.transform.position = position;

            newZote.GetComponent<AudioSource>().clip=null;
            newZote.GetComponent<tk2dSprite>().enabled = false;

            //GameObject.Destroy(newZote.GetComponent<tk2dSprite>());

            BoxCollider2D col = newZote.GetComponent<BoxCollider2D>();
            col.size = new Vector2(col.size.x * xScaleMult, col.size.y * yScaleMult);

            DialogueNPC dnpc = newZote.gameObject.AddComponent<DialogueNPC>();
            dnpc.NPC_TITLE = "Pronoespro";
            dnpc.Dialogue = new string[] { "prono_welcome_0", "prono_welcome_1", "prono_welcome_2", "prono_welcome_3", "prono_welcome_4" };
            dnpc.NPC_DREAM_KEY = "prono_dreamnail_0";
            AssetBundle ab = PronoesProMod.Instance.soundBundle["sounds"];

            dnpc.SingleClips = new System.Collections.Generic.Dictionary<string, AudioClip>()
            {
            };
            dnpc.MultiClips = new System.Collections.Generic.Dictionary<string, AudioClip[]>
            {
                {"prono_welcome_0",new AudioClip[]{ ab.LoadAsset<AudioClip>("introPiano"), ab.LoadAsset<AudioClip>("introPiano"), ab.LoadAsset<AudioClip>("introPiano"), ab.LoadAsset<AudioClip>("introPiano"), ab.LoadAsset<AudioClip>("introPiano") } }
            };
            dnpc.DialogueSelector = (() => {
                return "prono_welcome_0";
            });
            //dnpc.SetUp();
        }

        public IEnumerator ShowTitle(int titleType)
        {
            if (PronoesProMod.Instance.fadedIn)
            {
                yield return new WaitForSeconds(1f);
            }
            PronoesProMod.Instance.ShowLevelName(titleType);
        } 

        public void CreateDreamNailPortal(Vector2 pos,Vector2 size,string destination,string gate)
        {
            GameObject portal = new GameObject();
            portal.transform.SetPosition2D(pos);
            BoxCollider2D col = portal.AddComponent<BoxCollider2D>();
            col.size = size;
            col.isTrigger = true;
            col.gameObject.layer = LayerMask.NameToLayer("Interactive Object");
            DreamPortal dp = portal.AddComponent<DreamPortal>();
            dp.sceneToLoad = destination;
            dp.curScene = gate;
        }

        public void CreateDreamNailPortal(Vector2 pos, Vector2 size, string destination, string gate,ParticleSystem particles)
        {
            GameObject portal = new GameObject();
            portal.transform.SetPosition2D(pos);
            BoxCollider2D col = portal.AddComponent<BoxCollider2D>();
            col.size = size;
            col.isTrigger = true;
            col.gameObject.layer = LayerMask.NameToLayer("Interactive Object");
            DreamPortal dp = portal.AddComponent<DreamPortal>();
            dp.sceneToLoad = destination;
            dp.curScene = gate;
            dp.particles = particles;
            portal.AddComponent<NonBouncer>();
        }

        public void CreateGateway(string gateName,Vector2 pos,Vector2 size,string toScene,string entryGate, bool left, bool onlyOut,GameManager.SceneLoadVisualizations vis)
        {
            GameObject gate = new GameObject(gateName);

            gate.transform.SetPosition2D(pos);
            TransitionPoint tp = gate.AddComponent<TransitionPoint>();

            if (!onlyOut)
            {
                BoxCollider2D bc = gate.AddComponent<BoxCollider2D>();
                bc.size = size;
                bc.isTrigger = true;
                tp.targetScene = toScene;
                tp.entryPoint = entryGate;
            }

            tp.alwaysEnterLeft = left;
            tp.alwaysEnterRight = !left;

            GameObject rm = new GameObject("Hazard Respawn Marker");
            rm.transform.parent = tp.transform;
            rm.transform.SetPosition2D(rm.transform.position.x + (left?3f:-3f), rm.transform.position.y);
            HazardRespawnMarker hrm = rm.AddComponent<HazardRespawnMarker>();
            tp.respawnMarker = hrm;
            tp.sceneLoadVisualization = vis;

        }

        private void SceneManager_Start(On.SceneManager.orig_Start orig, SceneManager self)
        {
            scenesss = self;
            if(scene== "Tutorial_01")
            {
                self.sceneType = GlobalEnums.SceneType.GAMEPLAY;
                self.mapZone = GlobalEnums.MapZone.CROSSROADS;
                self.darknessLevel = 1;
                self.saturation = 1f;
                self.ignorePlatformSaturationModifiers = false;
                self.isWindy = true;
                self.isTremorZone = false;
                self.environmentType = 0;
                self.noParticles = false;
                self.overrideParticlesWith = GlobalEnums.MapZone.CROSSROADS;
                self.defaultColor = new Color(0.5f, 0.5f, 0.5f);
                self.defaultIntensity = 1f;
                self.heroLightColor = Color.white;
            }
            if (scene == "entrance")
            {
                self.sceneType = GlobalEnums.SceneType.GAMEPLAY;
                self.mapZone = GlobalEnums.MapZone.FINAL_BOSS;
                self.darknessLevel = 50;
                self.saturation = 1f;
                self.ignorePlatformSaturationModifiers = false;
                self.isWindy = false;
                self.isTremorZone = false;
                self.environmentType = 4;
                self.noParticles = false;
                self.overrideParticlesWith = GlobalEnums.MapZone.FINAL_BOSS;
                self.defaultColor = new Color(1f, 0.8f, 0.8f);
                self.defaultIntensity = 15f;
                self.heroLightColor = Color.white;
            }
            else
            if (scene == "CreatedTown")
            {
                self.sceneType = GlobalEnums.SceneType.GAMEPLAY;
                self.mapZone = GlobalEnums.MapZone.FINAL_BOSS;
                self.darknessLevel = 0;
                self.saturation = 1f;
                self.ignorePlatformSaturationModifiers = false;
                self.isWindy = true;
                self.isTremorZone = false;
                self.environmentType = 4;
                self.noParticles = false;
                self.overrideParticlesWith = GlobalEnums.MapZone.FINAL_BOSS;
                self.defaultColor = new Color(1f, 0.8f, 0.8f);
                self.defaultIntensity = 1f;
                self.heroLightColor = Color.white;
            }
            orig(self);
        }

    }
}
