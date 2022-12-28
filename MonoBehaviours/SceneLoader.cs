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

        //Entrance
        public string[] entranceGOs = new string []{ "Portal", "GroundObjs", "Ground","crates_d", "Background", "Spikes_spk" };
        //Town
        public string[] townGOs = new string[] { "TownGround","Decorations", "crates_d", "NoBounce_nb", "Spikes_spk","Lamps_l", "Basket", "DeeCart_points", "Temple", "ultimate bench" };
        public DialogPromptInteractableObject[] townInteractables = new DialogPromptInteractableObject[] { new DialogPromptInteractableObject("ultimate bench", new string[] { "UltimateBench1", "UltimateBench2" },new string[] { ""},"Rest")};
        public Dictionary<string, DialogSettings[]> townNPCdialogs = new Dictionary<string, DialogSettings[]> {
            {"Pro", new DialogSettings[] { new DialogSettings(DialogSettings.GetDefaultMask(), new string[] { "prono_welcome_0", "prono_welcome_1", "prono_welcome_2" }, new string[] { "kahmo"}, "Pronoespro_MAIN", "Pronoespro_SUB", "Pronoespro_SUPER", 2f,true,true,inteactionDisplay:"Talk" ),
                new DialogSettings(DialogSettings.GetDefaultMask(), new string[] { "prono_upgrade_charm_dash_0","prono_upgrade_charm_dash_1","prono_upgrade_charm_dash_2" }, new string[] { "kahmo" }, "Pronoespro_MAIN", "Pronoespro_SUPER", "Pronoespro_SUB", 4f,false,true,dialogRequirements:new string[]{ "Charm:31"},inteactionDisplay:"Talk"),
                new DialogSettings(DialogSettings.GetDefaultMask(), new string[] { "prono_welcome_3" }, new string[] { "kahmo" }, "Pronoespro_MAIN", "Pronoespro_SUPER", "Pronoespro_SUB", 2.5f,false,false,inteactionDisplay:"Talk") } }};
        //Apple minigame
        public string[] appleMiniGOs = new string[] { "Objects", "TownBackground", "default", "Grid" };
        
        //General
        public string hitParticlesName = "Bounce_hit",spikePariclesName = "Spike_hit",successParticlesName="Success";
        public ParticleSystem hitParticles,spikeParticles,successParticles;
        public static Transform interactionPrompt;

        public Dictionary<string, int> appleAmmountsPerScene = new Dictionary<string, int>() { { "CreatedTown", 13 },{ "appleminigame", 1} };

        public void Start()
        {
            On.GameManager.EnterHero += GameManager_EnterHero;
            On.SceneManager.Start += SceneManager_Start;
        }

        public void OnDestroy()
        {
            On.GameManager.EnterHero -= GameManager_EnterHero;
            On.SceneManager.Start -= SceneManager_Start;
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

        public void LoadUI()
        {
            AssetBundle ab = PronoesProMod.Instance.GOBundle["ui"];
            if (ab.Contains("UI"))
            {
                interactionPrompt = GameObject.Instantiate(ab.LoadAsset<GameObject>("InteractPrompt"),new Vector3(0,0,-1),Quaternion.identity).transform;
                interactionPrompt.gameObject.AddComponent<InteractionPrompt>();
                PronoesProMod.Instance.interactionPropt = interactionPrompt.GetComponent<InteractionPrompt>();
            }
        }

        private void GameManager_EnterHero(On.GameManager.orig_EnterHero orig, GameManager self, bool additiveGateSearch)
        {

            scene = self.sceneName;

            if (self!=null && self.sceneName == "Tutorial_01")
            {
                if (PlayerData.instance.hasDreamNail)
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
            }
            else if (self.sceneName == "entrance")
            {
                LoadUI();
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
                LoadUI();
                if (PronoesProMod.Instance.preloadedObjs.ContainsKey("Tutorial_01") && PronoesProMod.Instance.preloadedObjs["Tutorial_01"].ContainsKey("_SceneManager"))
                {
                    GameObject go = GameObject.Instantiate(PronoesProMod.Instance.preloadedObjs["Tutorial_01"]["_SceneManager"]);
                }

                LoadBlurPlane(new Vector3(420, 30, 7), new Vector3(100, 17, 17));
                CreateBench(new Vector3(78, 10.75f, 1), new Vector3(1, 1, 1), 0);

                CreateGateway("left", new Vector2(1, 16), new Vector2(4, 8), "entrance", "right", false, false, GameManager.SceneLoadVisualizations.Default);
                
                PronoCustomNPC pro= LoadCharacter("npcs", new Vector3(105,13.75f,1),"Pro");

                if (pro != null){
                    //pro.dialogs[0].onEnd.AddListener(() => NextDialogOfNPC(pro));

                    pro.dialogs[1].onEnd.AddListener(() => UpgradeCharm(0));
                    PronoesProMod.Instance.Log("Added charm upgrade OwO");

                    pro.dialogs[2].onEnd.AddListener(() => SetProKnightSkin());
                    pro.dialogs[2].onEnd.AddListener(() => SetProKnightSkin());
                    PronoesProMod.Instance.Log("Added costume upgrade! YAY!");
                    //pro.dialogs[1].onStart.AddListener(() => NextDialogOfNPC(pro));
                }

                //CreateGateway("right",new Vector2(271, 10.28f), new Vector2(5, 1), "appleminigame", "left", true,false,GameManager.SceneLoadVisualizations.Default);

                LoadObjects(townGOs);
                LoadInteractables(townInteractables);

                if (PronoesProMod.Instance.GOBundle == null)
                {
                    PronoesProMod.Instance.Log("No GOBundle");
                }

                if (PronoesProMod.Instance.GOBundle.ContainsKey("music"))
                {
                    AssetBundle ab = PronoesProMod.Instance.GOBundle["music"];
                    AudioClip clip = ab.LoadAsset<AudioClip>("town");
                    MusicChanger.PlayBackgroundMusicForScene(clip);
                }

                StartCoroutine(ShowTitle(1));
            }
            else if (self.sceneName== "appleminigame")
            {
                LoadUI();
                PronoesProMod.Instance.Log("Started creating minigame");
                if (PronoesProMod.Instance.preloadedObjs.ContainsKey("Tutorial_01") && PronoesProMod.Instance.preloadedObjs["Tutorial_01"].ContainsKey("_SceneManager"))
                {
                    GameObject go = GameObject.Instantiate(PronoesProMod.Instance.preloadedObjs["Tutorial_01"]["_SceneManager"]);
                }

                LoadBlurPlane(new Vector3(420, 30, 7), new Vector3(100, 17, 17));
                CreateGateway("left", new Vector2(10, 16), new Vector2(5, 1), "CreatedTown", "right", true, true, GameManager.SceneLoadVisualizations.Default);

                LoadObjects(appleMiniGOs);

                if (PronoesProMod.Instance.GOBundle.ContainsKey("music"))
                {
                    AssetBundle ab = PronoesProMod.Instance.GOBundle["music"];
                    AudioClip clip = ab.LoadAsset<AudioClip>("town");
                    MusicChanger.PlayBackgroundMusicForScene(clip);
                }

                PronoesProMod.Instance.Log("Should have finished loading...");
                //StartCoroutine(ShowTitle(2));
            }
            if (PronoesProMod.Instance != null && PronoesProMod.Instance.fadedIn)
            {
                PronoesProMod.Instance.CustomSceneFadeOutsis();
                PronoesProMod.Instance.Log("Fading out in " + scene + "!");
            }
            orig(self,additiveGateSearch);
        }

        public void LoadInteractables(DialogPromptInteractableObject[] interactables)
        {

            if (PronoesProMod.Instance.GOBundle.ContainsKey("UI"))
            {
                AssetBundle ab = PronoesProMod.Instance.GOBundle["UI"];

                if (ab.Contains("InteractionPrompts"))
                {
                    GameObject inter = ab.LoadAsset<GameObject>("InteractionPrompts");
                    PronoesProMod.Instance.interactionPropt=inter.AddComponent<InteractionPrompt>();
                }

            }

            foreach(DialogPromptInteractableObject inter in interactables)
            {
                GameObject interObj = GameObject.Find(inter.objName);
                if (interObj != null)
                {
                    PronoCustomNPC npc= interObj.AddComponent<PronoCustomNPC>();
                    npc.dialogs = new DialogSettings[] { new DialogSettings(DialogSettings.GetDefaultMask(), inter.objDialog, inter.dialogSounds,"","","",2, startOnCollision: false,false,inteactionDisplay: inter.interactionPropt) };

                    interObj.AddComponent<NonBouncer>();
                }
            }
        }

        public void LoadObjects(string[] objNames)
        {
            GameObject hitPart = GameObject.Find(hitParticlesName);
            if (hitPart != null)
            {
                hitParticles = hitPart.GetComponentInChildren<ParticleSystem>();
            }
            hitPart = GameObject.Find(spikePariclesName);
            if (hitPart != null)
            {
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
                            BouncingObject bounce = foundGO.GetChild(c).gameObject.AddComponent<BouncingObject>();
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
                            Lamp lamp = foundGO.GetChild(child).gameObject.AddComponent<Lamp>();
                            if (spikeParticles != null)
                            {
                                lamp.particles = spikeParticles;
                            }
                        }
                    }
                    if (objNames[i].Contains("Basket"))// && appleAmmountsPerScene.ContainsKey(scene))
                    {
                        AppleBasket basket = foundGO.gameObject.AddComponent<AppleBasket>();
                        basket.applesToReward = appleAmmountsPerScene[scene];
                        basket.correctParticles = successParticles;
                    }
                    if (objNames[i].Contains("_points") && foundGO.childCount > 0)
                    {
                        int closestPoint = 0;
                        List<GameObject> objs = new List<GameObject>();
                        for (int obj = 0; obj < foundGO.childCount; obj++)
                        {
                            objs.Add(foundGO.GetChild(obj).gameObject);
                            if (Vector2.Distance(objs[obj].transform.position, HeroController.instance.transform.position) < Vector2.Distance(objs[closestPoint].transform.position, HeroController.instance.transform.position))
                            {
                                closestPoint = obj;
                            }
                        }

                        if (PronoesProMod.Instance.NPCBundle.ContainsKey("npcs") && PronoesProMod.Instance.NPCBundle["npcs"].Contains("dee_cart"))
                        {
                            GameObject cart = GameObject.Instantiate(PronoesProMod.Instance.NPCBundle["npcs"].LoadAsset<GameObject>("dee_cart"), objs[closestPoint].transform.position, Quaternion.identity);
                            DeeTransportation transport = cart.AddComponent<DeeTransportation>();
                            transport.points = objs.ToArray();
                            transport.curPoint = closestPoint;
                        }
                    }

                    if (foundGO.GetComponentInChildren<AudioSource>() != null)
                    {
                        foreach (AudioSource source in foundGO.GetComponentsInChildren<AudioSource>())
                        {
                            source.outputAudioMixerGroup = PronoesProMod.enviroMixer.outputAudioMixerGroup;
                        }
                    }
                    if (foundGO.GetComponent<AudioSource>() != null)
                    {
                        foreach (AudioSource source in foundGO.GetComponents<AudioSource>())
                        {
                            source.outputAudioMixerGroup = PronoesProMod.enviroMixer.outputAudioMixerGroup;
                        }
                    }
                }
            }
        }

        public PronoCustomNPC LoadCharacter(string type, Vector3 position, string npcName)
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
                PronoCustomNPC customNPC = go.AddComponent<PronoCustomNPC>();

                if (customNPC!=null && townNPCdialogs.ContainsKey(npcName))
                {
                    customNPC.SetDialogSettings(townNPCdialogs[npcName]);
                }else{
                    PronoesProMod.Instance.Log("Failed adding dialog");
                }

                return customNPC;
            }
            return null;
        }

        public void CreateTileMap()
        {
            PronoesProMod.InstanciatePreloaded("Tutorial_01", "TileMap");
            PronoesProMod.InstanciatePreloaded("Tutorial_01", "TileMap Render Data");
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

        public void NextDialogOfNPC(PronoCustomNPC npc)
        {
            npc.NextDialog();
            PronoesProMod.Instance.Log("Next Dialog!");
        }

        public void UpgradeCharm(int charmNum)
        {
            PronoesProMod.Instance.upgradedCharms[charmNum] = true;
        }

        public void SetProKnightSkin()
        {
            PronoesProMod.Instance.SetKnightProSkin();
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

        public void CreateDoor(string name,Vector2 pos, Vector2 size, string destination, string gate,bool enterLeft, bool onlyOut, GameManager.SceneLoadVisualizations vis)
        {
            GameObject door = new GameObject(name);
            door.transform.SetPosition2D(pos);
            door.layer = LayerMask.NameToLayer("TransitionGate");

            Door_Prono dp = door.AddComponent<Door_Prono>();

            if (!onlyOut)
            {
                BoxCollider2D col = door.AddComponent<BoxCollider2D>();
                col.size = size;
                col.isTrigger = true;

                dp.entryPoint = gate;
                dp.targetScene = destination;
            }

            dp.sceneLoadVisualization = vis;

            door.AddComponent<NonBouncer>();

            GameObject rm = new GameObject("Hazard Respawn Marker");
            rm.transform.SetPosition2D(rm.transform.position.x + (enterLeft ? 3f : -3f), rm.transform.position.y);
            HazardRespawnMarker hrm = rm.AddComponent<HazardRespawnMarker>();

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
            orig(self);

            if (scene == "entrance")
            {
                self.sceneType = GlobalEnums.SceneType.GAMEPLAY;
                self.mapZone = GlobalEnums.MapZone.FINAL_BOSS;
                self.darknessLevel = 0;
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
            else if (scene == "CreatedTown")
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
            }else if (scene == "appleminigame")
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
        }

    }
}
