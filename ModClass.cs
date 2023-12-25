using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using JetBrains.Annotations;
using Modding;
using PronoesProMod.Extensions;
using PronoesProMod.Monobehaviors;
using PronoesProMod.MonoBehaviours;
using SFCore.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;
using UObject = UnityEngine.Object;

namespace PronoesProMod
{

    [System.Serializable]
    public struct SpellSwapData
    {
        public string spellNameKey,spellDescriptionKey;
        public int unlockCost;
        public string coliseumSceneForSpell;
        public string unlockRequirements;
        public bool unlocked;

        public SpellSwapData(string _name,string _desc,string _colo,string _requirements,int _cost=10,bool _unlock=false)
        {
            spellNameKey = _name;
            spellDescriptionKey = _desc;
            coliseumSceneForSpell = _colo;
            unlockCost = _cost;
            unlockRequirements = _requirements;
            unlocked = _unlock;
        }

        public string GetName(int _spellLevel)
        {
            return spellNameKey.Replace("*", _spellLevel.ToString());
        }

        public string GetDescription(int _spellLevel)
        {
            return spellDescriptionKey.Replace("*", _spellLevel.ToString());
        }
    }

    [UsedImplicitly]
    public class PronoesProMod : Mod, ITogglableMod, IGlobalSettings<PronoesproGlobalSaveData>, ILocalSettings<PronoesproLocalSaveData>
    {

        #region Variables
        public Dictionary<string, AssetBundle> SceneBundle;
        public Dictionary<String, AssetBundle> GOBundle;
        public Dictionary<String, AssetBundle> NPCBundle;
        public Dictionary<String, AssetBundle> soundBundle;
        public Dictionary<String, AssetBundle> skinsBundle;
        public Dictionary<String, AssetBundle> spritesBundle;

        public Dictionary<string, Dictionary<string, GameObject>> preloadedObjs;
        public static Satchel.Core satchel = new Satchel.Core();

        public List<string> levelsToLoad = new List<string>() { "entrance", "createdtown", "appleminigame" };
        public List<string> objectsToLoad = new List<string>() { "portal", "music", "ui", "brokenobjs", "newattacks" };
        public List<string> npcsToLoad = new List<string>() { "npcs", "dee-coder", "de-playtest" };
        public List<string> skinsToLoad = new List<string>() { "skins", "animations" };
        public List<string> spritesToLoad = new List<string>() { "charmupgrades" };
        public string soundsToLoad = "sounds";

        public bool playerFaceLeft;
        public bool playerFalling;

        public bool fadedIn;

        //Location icons
        public string[] locationNames = new string[] { "CraftedTown" };
        public Dictionary<string, Sprite> locationIcons;

        //Spells
        public Dictionary<string, FsmState> ogDiveStates;
        public string[] ogDiveStateNames = new string[] { "Quake1 Land", "Q2 Land", "Q2 Pillar", "Quake1 Down", "Quake2 Down", "Quake Antic", "Spell End" };
        public Dictionary<string, FsmState> ogSoulStates;
        public string[] ogSoulStateNames = new string[] { "Fireball 1", "Fireball 2", "Fireball Recoil" };
        public Dictionary<string, FsmState> ogShriekStates;
        public string[] ogShriekStateNames = new string[] { "Scream Antic1", "Scream Antic2", "Scream Burst 1", "Scream Burst 2" };

        public int equipedSoulType, equipedDiveType, equipedShriekType;

        public string[] spellNames = new string[] { "SawbladeAttack", "NailAttack", "AppleAttack", "FallingNails", "GiantNail", "ShootSawbladeAttack", "NailBarrage", "Dee_explode", "ApplePieSawblade", "AppleSliceAttack", "SawbladeShout", "DeeExplosionParticles" };
        public Dictionary<string, GameObject> newSpells;
        public int[] spellUsedTimes;
        public int[] spellAmmount;

        public string[] spellSpriteNames = new string[] { "icon_apple_shriek", "icon_apple_soul", "icon_apple_dive", "icon_def_dive", "icon_def_shriek", "icon_def_soul", "icon_nailmaster_dive", "icon_nailmaster_shriek", "icon_nailmaster_soul", "icon_sawblade_dive", "icon_sawblade_soul", "icon_sawblade_shriek", "icon_pro_soul" };
        public Dictionary<string, Sprite> spellSprites;
        public SpellSwapData[] unlockableSouls = new SpellSwapData[] {
            new SpellSwapData("Soul_sawblade_*", "Soul_sawblade_*_desc","","sawblade",50),
            new SpellSwapData("Soul_nailmaster_*","Soul_nailmaster_*_desc","","nailmaster",30),
            new SpellSwapData("Soul_apple_*","Soul_apple_*_desc","","apple",75),
            new SpellSwapData("Soul_Dee_*","Soul_Dee_0_desc","","craftedTownFinished",100)};
        public SpellSwapData[] unlockableDives = new SpellSwapData[] {
            new SpellSwapData("Dive_sawblade_*","Dive_sawblade_*_desc","","sawblade",100),
            new SpellSwapData("Dive_nailmaster_*","Dive_nailmaster_*_desc","","nailmaster",90),
            new SpellSwapData("Dive_apple_*","Dive_apple_*_desc","","apple",90)
        };
        public SpellSwapData[] unlockableShrieks = new SpellSwapData[]{
            new SpellSwapData("Shriek_sawblade_*","Shriek_sawblade_*_desc","","sawblade",125),
            new SpellSwapData("Shriek_nailmaster_*","Shriek_nailmaster_*_desc","","nailmaster",100),
            new SpellSwapData("Shriek_apple_*","Shriek_apple_*_desc","","apple",200)
            };

        public static string soulInventoryNameKey="INV_NAME_SPELL_FIREBALL",shriekInventoryNameKey= "INV_NAME_SPELL_SCREAM",diveInventoryNameKey= "INV_NAME_SPELL_QUAKE";
        public static string soulInventoryDescriptionKey= "INV_DESC_SPELL_FIREBALL", shriekInventoryDescriptionKey= "INV_DESC_SPELL_SCREAM", diveInventoryDescriptionKey = "INV_DESC_SPELL_QUAKE";

        //Charms
        public Dictionary<string, FsmState> ogCharmStates;
        public Dictionary<string, Transform> charmSpawns;
        public Dictionary<string, int> charmProjectileNums;
        public Transform ogDreamShield;

        public Dictionary<string, Sprite> ogCharmSprites;

        //Skin related
        private bool proSkinUsing;
        private Dictionary<string, Texture> ogKnightSprite;

        //Save Data
        private bool proIntroDone;
        private bool loadedKnightPowersAndSkins;

        //Shop related
        private ProShopUI proShop;

        //Misc animations related
        private bool knightSpinning;

        public static GameObject levelNameDisplay;
        public static Transform spellChangeUI, proShopUI;

        public int soul, shriek, dive;



        public static string[] MeleeAttacks = new string[] { "Nail Attack" };

        public void FinishIntro() {
            proIntroDone = true;
        }

        public bool IsIntroDone() { return proIntroDone; }
        /*
        public Dictionary<string, string[]> spellNameKeys = new Dictionary<string, string[]>(){
            { "INV_NAME_SPELL_SCREAM1",new string[]{ "Soul_sawblade_0", "Soul_nailmaster_0", "Shriek_apple_0" } },
            { "INV_NAME_SPELL_SCREAM2",new string[]{ "Soul_sawblade_1", "Soul_nailmaster_1", "Shriek_apple_1" } },
            {"INV_NAME_SPELL_FIREBALL1",new string[]{ "Dive_sawblade_0", "Dive_nailmaster_0", "Soul_apple_0","Soul_Dee_0" } },
            {"INV_NAME_SPELL_FIREBALL2",new string[]{ "Dive_sawblade_1", "Dive_nailmaster_1", "Soul_apple_1","Soul_Dee_1" } },
            { "INV_NAME_SPELL_QUAKE1", new string[] { "Shriek_sawblade_0", "Shriek_nailmaster_0", "Dive_apple_0" } },
            {"INV_NAME_SPELL_QUAKE2", new string[]{ "Shriek_sawblade_1", "Shriek_nailmaster_1", "Dive_apple_1" } }
        };
        public Dictionary<string, string[]> spellDescKeys = new Dictionary<string, string[]>(){
            { "INV_DESC_SPELL_SCREAM1",new string[]{ "Soul_sawblade_0_desc", "Soul_nailmaster_0_desc", "Shriek_apple_0_desc" }},
            { "INV_DESC_SPELL_SCREAM2" ,new string[]{ "Soul_sawblade_1_desc", "Soul_nailmaster_1_desc", "Shriek_apple_1_desc" } },
            {"INV_DESC_SPELL_FIREBALL1",new string[]{ "Dive_sawblade_0_desc", "Dive_nailmaster_0_desc", "Soul_apple_0_desc","Soul_Dee_0_desc" }  },
            {"INV_DESC_SPELL_FIREBALL2",new string[]{ "Dive_sawblade_1_desc", "Dive_nailmaster_1_desc", "Soul_apple_1_desc", "Soul_Dee_1_desc" }  },
            {"INV_DESC_SPELL_QUAKE1",new string[] { "Shriek_sawblade_0_desc", "Shriek_nailmaster_0_desc", "Dive_apple_0_desc" } },
            {"INV_DESC_SPELL_QUAKE2", new string[]{ "Shriek_sawblade_1_desc", "Shriek_nailmaster_1_desc", "Dive_apple_1_desc" } }
        };*/

        public List<(string, string)> toPreload = new List<(string, string)>
        {
                ( "Cliffs_01","Cornifer Card"),
                ( "Town","_NPCs"),
                ("Tutorial_01","BlurPlane (1)"),
                ("Town","RestBench"),
                ("Funfus3_40","Station Bell"),
                ("Funfus3_40","secret sound"),
                ("Tutorial_01","_SceneManager"),
                ("Tutorial_01","TileMap"),
                ("Tutorial_01","TileMap Render Data")
        };

        public bool[] upgradedCharms;

        public InteractionPrompt interactionPropt;

        public static AudioMixer enviroMixer, musicMixer, actorsMixer, masterMixer, atmosMixer;

        /*
        string UI = "UI";

        string Essence = "INV_NAME_DREAMCORE";
        string Apply = "OPT_MENU_APPLY_BUTTON";
        string Buy = "CTRL_BUY";
        string WantToPurcchase = "SHOP_PURCHASE_CONFIRM";
        string Purchase = "SHOP_PURCHASE_COMPLETE";
        string COMPLETED = "COMPLETED";
        string Cost = "CHARM_TXT_COST";
        string Remove = "CTRL_MARKER_REMOVE";
        */

        public override List<(string, string)> GetPreloadNames()
        {
            return toPreload;
        }


        public void UnloadVariables()
        {
            fadedIn = false;

            GameObject.Destroy(levelNameDisplay);
            levelNameDisplay = null;
            charmSpawns = null;
            loadedKnightPowersAndSkins = fadedIn;
            ogKnightSprite = null;
        }

        public void UnloadCharacterSpecificVariables()
        {

        }

        #endregion

        #region Setup
        public PronoesProMod() : base("PronoesPro's mod") { }

        internal static PronoesProMod Instance;

        public override string GetVersion()
        {
            return "Darkness v0.1.0.0";
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {

            upgradedCharms = new bool[5];

            Log("Initializing");

            Instance = this;

            preloadedObjs = preloadedObjects;
            ogKnightSprite = new Dictionary<string, Texture>();

            SetupMethods();
            SetupBundles();

            InitializeFSM();
            InitializeData();

            InitializeUI();

            Log("Finished Initializing");


            /*
            foreach(AudioMixer mix in Resources.FindObjectsOfTypeAll<AudioMixer>())
            {
                Log("---Audio Mixer: "+mix.name+"---");
            }*/

            enviroMixer = Resources.FindObjectsOfTypeAll<AudioMixer>().First(x => x.name == "EnviroEffects");
            musicMixer = Resources.FindObjectsOfTypeAll<AudioMixer>().First(x => x.name == "Music");
            actorsMixer = Resources.FindObjectsOfTypeAll<AudioMixer>().First(x => x.name == "Actors");
            atmosMixer = Resources.FindObjectsOfTypeAll<AudioMixer>().First(x => x.name == "Atmos");
            masterMixer = Resources.FindObjectsOfTypeAll<AudioMixer>().First(x => x.name == "Master");

            InitializeAnimationStuff();

        }

        public void Unload()
        {
            preloadedObjs = null;

            UnloadMethods();
            UnloadBundles();

            UnloadFSM();
            UnloadVariables();
            RemoveSceneLoader();
            upgradedCharms = null;

            UnloadUI();

            //UnloadAnimationStuff();

            Instance = null;

        }

        #region knight animation


        private Dictionary<string, tk2dFullAnimation> knightAnimations;
        public tk2dSpriteAnimator heroAnimator;
        public ProPlayerAnimation playerAnim;

        public void InitializeAnimationStuff()
        {

            //Setup
            On.HeroController.Awake += SetupOnlyOnce;
            On.HeroController.Start += PlayerAnimatorSetup;


            On.HeroController.Update += SpinKnightSprite;
        }


        public void UnloadAnimationStuff()
        {
            On.HeroController.Awake -= SetupOnlyOnce;
            On.HeroController.Start -= PlayerAnimatorSetup;

            On.HeroController.Update -= SpinKnightSprite;
        }

        private void SetupOnlyOnce(On.HeroController.orig_Awake orig, HeroController self)
        {
            On.HeroController.Awake -= SetupOnlyOnce;


            if (knightAnimations == null)
            {
                knightAnimations = new Dictionary<string, tk2dFullAnimation>();

                if (skinsBundle.ContainsKey("animations") && skinsBundle["animations"].Contains("spin"))
                {

                    Texture2D _tex = skinsBundle["animations"].LoadAsset<Texture2D>("spin");
                    tk2dSpriteCollectionData _collection = tk2dAuxiliarHelper.CreateCollectionSimple(_tex, HeroController.instance.gameObject, new Vector2(_tex.width / 2 / 8, _tex.height / 4), 8);

                    tk2dFullAnimationData _anim = new tk2dFullAnimationData(_collection,
                        new tk2dAnimationDataForCreation[] {
                            new tk2dAnimationDataForCreation("spin", null, new int[] { 0,0,0,0,1,1,1,1,2,2,2,2,3,3,3,3,4,4,4,4,5,5,5,5,6,6,6,6,7,7,7,7}) }
                        );

                    knightAnimations.Add("spin",
                        new tk2dFullAnimation("spin", _anim, _collection));

                    Log("Made the spin knight");
                }
            }

            orig(self);

            foreach (string key in knightAnimations.Keys)
            {

                heroAnimator = HeroController.instance.gameObject.GetComponent<tk2dSpriteAnimator>();
                List<tk2dSpriteAnimationClip> list2 = heroAnimator.Library.clips.ToList<tk2dSpriteAnimationClip>();

                foreach (tk2dSpriteAnimationClip item in tk2dAuxiliarHelper.MakeAnimations(knightAnimations[key].colection, knightAnimations[key].animation.animationData))
                {
                    list2.Add(item);
                }
                heroAnimator.Library.clips = list2.ToArray();

            }

        }


        private void PlayerAnimatorSetup(On.HeroController.orig_Start orig, HeroController self)
        {
            if (playerAnim == null)
            {
                playerAnim = HeroController.instance.gameObject.AddComponent<ProPlayerAnimation>();
                playerAnim.heroAnimator = heroAnimator;
            }

            orig(self);
        }

        private void SpinKnightSprite(On.HeroController.orig_Update orig, HeroController self)
        {
            if (playerAnim != null && knightSpinning)
            {
                playerAnim.ChangeAnimation("spin", relinquishControl: true);
            }
            orig(self);
        }

        public void StartSpinningKnight()
        {
            knightSpinning = true;
        }

        public void EndSpinningKnight()
        {
            if (playerAnim != null && playerAnim.GetIsDoingAnimation())
            {
                playerAnim.ForceEndAnimation();
            }
            knightSpinning = false;
        }

        #endregion

        #endregion

        #region Bundles
        public void SetupBundles()
        {

            SceneBundle = new Dictionary<string, AssetBundle>();
            GOBundle = new Dictionary<string, AssetBundle>();
            NPCBundle = new Dictionary<string, AssetBundle>();
            soundBundle = new Dictionary<string, AssetBundle>();
            skinsBundle = new Dictionary<string, AssetBundle>();
            spritesBundle = new Dictionary<string, AssetBundle>();

            Assembly asm = Assembly.GetExecutingAssembly();
            Log("Searching for Levels");
            foreach (string res in asm.GetManifestResourceNames())
            {
                using (Stream s = asm.GetManifestResourceStream(res))
                {
                    if (s == null)
                    {
                        continue;
                    }
                    Log("Found asset");

                    byte[] buffer = new byte[s.Length];
                    s.Read(buffer, 0, buffer.Length);
                    s.Dispose();
                    string bundleName = Path.GetExtension(res).Substring(1);
                    if (levelsToLoad.Contains(bundleName))
                    {
                        Log("Found Level " + bundleName);
                        SceneBundle.Add(bundleName, AssetBundle.LoadFromMemory(buffer));
                    } else if (objectsToLoad.Contains(bundleName))
                    {
                        GOBundle.Add(bundleName, AssetBundle.LoadFromMemory(buffer));
                    } else if (npcsToLoad.Contains(bundleName))
                    {
                        NPCBundle.Add(bundleName, AssetBundle.LoadFromMemory(buffer));
                    } else if (bundleName == soundsToLoad)
                    {
                        soundBundle.Add(bundleName, AssetBundle.LoadFromMemory(buffer));
                    } else if (skinsToLoad.Contains(bundleName))
                    {
                        skinsBundle.Add(bundleName, AssetBundle.LoadFromMemory(buffer));
                    } else if (spritesToLoad.Contains(bundleName))
                    {
                        spritesBundle.Add(bundleName, AssetBundle.LoadFromMemory(buffer));
                    } else {
                        continue;
                    }

                }
            }
        }

        public void UnloadBundles()
        {
            if (SceneBundle != null)
            {
                SceneBundle.Clear();
                GOBundle.Clear();
                NPCBundle.Clear();
                soundBundle.Clear();
                skinsBundle.Clear();
            }

            SceneBundle = null;
            GOBundle = null;
            NPCBundle = null;
            soundBundle = null;
            skinsBundle = null;
        }

        public void RemoveSceneLoader()
        {

            SceneLoader x = GameManager.instance?.gameObject.GetComponent<SceneLoader>();
            if (x != null)
            {
                UObject.Destroy(x);
            }

        }
        #endregion

        #region Methods
        public void SetupMethods()
        {

            ModHooks.DashVectorHook += UpDash;

            ModHooks.AfterSavegameLoadHook += AfterSave;
            ModHooks.NewGameHook += NewGameStarting;
            On.HeroController.Move += SetDirection;

            ModHooks.LanguageGetHook += LanguageGet;

            ModHooks.HeroUpdateHook += TooglePowerMenu;
            On.GameManager.RefreshTilemapInfo += ModifyTilemapInfo;
        }

        public void UnloadMethods()
        {
            ModHooks.DashVectorHook -= UpDash;

            ModHooks.AfterSavegameLoadHook -= AfterSave;
            ModHooks.NewGameHook -= NewGameStarting;
            On.HeroController.Move -= SetDirection;

            ModHooks.LanguageGetHook -= LanguageGet;

            ModHooks.HeroUpdateHook -= TooglePowerMenu;
            On.GameManager.RefreshTilemapInfo -= ModifyTilemapInfo;

        }

        public static GameObject InstanciatePreloaded(string scene, string obj)
        {
            if (Instance.preloadedObjs.ContainsKey(scene) && Instance.preloadedObjs[scene].ContainsKey(obj))
            {
                GameObject instanciated = GameObject.Instantiate(Instance.preloadedObjs[scene][obj]);
                return instanciated;
            }
            return null;
        }

        private void SetDirection(On.HeroController.orig_Move orig, HeroController self, float move_direction)
        {
            if (Mathf.Abs(move_direction) > 0)
            {
                playerFaceLeft = move_direction < 0;
            }
            orig(self, move_direction);
        }

        public void AfterSave(SaveGameData obj)
        {
            AddSceneLoader();
        }

        private void NewGameStarting()
        {
            AddSceneLoader();
        }

        public bool KnightReady()
        {
            return HeroController.instance.CheckTouchingGround()
                && Mathf.Abs(HeroController.instance.GetComponent<Rigidbody2D>().velocity.y) < 0.1f
                && HeroController.instance.acceptingInput;
        }

        public void AddSceneLoader()
        {
            if (GameManager.instance.GetComponent<SceneLoader>() == null) {
                GameManager.instance.gameObject.AddComponent<SceneLoader>();
                Log("Added Scene Loader");
            }
        }

        public static bool IsGamePaused()
        {
            return GameManager.instance.isPaused || GameManager.instance.inventoryFSM.GetBoolVariable("Open").Value;
        }

        public void TooglePowerMenu()
        {
            if (PlayerData.instance.atBench && !IsGamePaused())
            {
                if (spellChangeUI != null)
                {
                    spellChangeUI.gameObject.SetActive(true);
                    UpdateSpellUI();
                }
                if (InputHandler.Instance.inputActions.superDash.WasReleased)
                {
                    equipedDiveType = (equipedDiveType + 1) % 4;
                }
                if (InputHandler.Instance.inputActions.quickCast.WasReleased)
                {
                    equipedSoulType = (equipedSoulType + 1) % 5;
                }
                if (InputHandler.Instance.inputActions.dash.WasReleased)
                {
                    equipedShriekType = (equipedShriekType + 1) % 4;
                }
            }
            else
            {
                if (spellChangeUI != null)
                {
                    spellChangeUI.gameObject.SetActive(false);
                }
            }
            if (InputHandler.Instance.inputActions.textSpeedup.WasReleased)
            {
                levelNameDisplay.transform.Find("DialogBox").SendMessage("ContinueConversation");
            }
        }

        public static bool HasCharm(int charmNum)
        {
            return PlayerData.instance.GetBool("equippedCharm_" + charmNum.ToString());
        }

        public void UpdateSpellUI()
        {

            if (spellChangeUI == null || spellSprites == null)
            {
                return;
            }

            string chosenIconName = "icon_";
            switch (soulType)
            {
                case 0:
                    chosenIconName += "def";
                    break;
                case 1:
                    chosenIconName += "sawblade";
                    break;
                case 2:
                    chosenIconName += "nailmaster";
                    break;
                case 3:
                    chosenIconName += "apple";
                    break;
                case 4:
                    chosenIconName += "pro";
                    break;
            }
            chosenIconName += "_soul";
            if (spellSprites.ContainsKey(chosenIconName))
            {
                Image spellIcon = spellChangeUI.Find("Soul").GetComponent<Image>();
                spellIcon.sprite = spellSprites[chosenIconName];
            }

            chosenIconName = "icon_";
            switch (diveType)
            {
                case 0:
                    chosenIconName += "def";
                    break;
                case 1:
                    chosenIconName += "sawblade";
                    break;
                case 2:
                    chosenIconName += "nailmaster";
                    break;
                case 3:
                    chosenIconName += "apple";
                    break;
                case 4:
                    chosenIconName += "pro";
                    break;
            }
            chosenIconName += "_dive";
            if (spellSprites.ContainsKey(chosenIconName))
            {
                Image spellIcon = spellChangeUI.Find("Dive").GetComponent<Image>();
                spellIcon.sprite = spellSprites[chosenIconName];
            }

            chosenIconName = "icon_";
            switch (shriekType)
            {
                case 0:
                    chosenIconName += "def";
                    break;
                case 1:
                    chosenIconName += "sawblade";
                    break;
                case 2:
                    chosenIconName += "nailmaster";
                    break;
                case 3:
                    chosenIconName += "apple";
                    break;
                case 4:
                    chosenIconName += "pro";
                    break;
            }
            chosenIconName += "_shriek";
            if (spellSprites.ContainsKey(chosenIconName))
            {
                Image spellIcon = spellChangeUI.Find("Shriek").GetComponent<Image>();
                spellIcon.sprite = spellSprites[chosenIconName];
            }

        }


        public Vector2 UpDash(Vector2 vel)
        {
            if (PlayerData.instance.GetBool("equippedCharm_31") && vel.y == 0 && upgradedCharms[0] && Mathf.Abs(HeroController.instance.vertical_input) > 0.1f)
            {
                return vel * 0.75f + new Vector2(0f, Mathf.Abs(vel.x) * 30f * Time.deltaTime);
            }
            return vel;
        }

        private void ModifyTilemapInfo(On.GameManager.orig_RefreshTilemapInfo orig, GameManager self, string targetScene)
        {
            if (self.tilemap == null)
            {
                Log("Tilemap not found");
                InstanciatePreloaded("Tutorial_01", "TileMap");
                InstanciatePreloaded("Tutorial_01", "TileMap Render Data");
            }

            orig(self, targetScene);

            if (targetScene == "entrance")
            {
                self.tilemap.width = 60;
                self.tilemap.height = 60;
                self.sceneWidth = 60;
                self.sceneHeight = 60;
                GameObject.FindObjectOfType<GameMap>().SetManualTilemap(0, 0, 60, 60);
            }
            else if (targetScene == "CreatedTown")
            {
                self.tilemap.width = 545;
                self.tilemap.height = 100;
                self.sceneWidth = 545;
                self.sceneHeight = 100;
                if (GameObject.FindObjectOfType<GameMap>() != null)
                {
                    GameObject.FindObjectOfType<GameMap>().SetManualTilemap(0, 0, 545, 100);
                }
            }
            else if (targetScene == "appleminigame")
            {
                if (self.tilemap != null)
                {
                    self.tilemap.width = 72;
                    self.tilemap.height = 33;
                    self.sceneWidth = 72;
                    self.sceneHeight = 33;
                }
                if (GameObject.FindObjectOfType<GameMap>() != null)
                {
                    GameObject.FindObjectOfType<GameMap>().SetManualTilemap(0, 0, 72, 33);
                }
            }
        }

        public void DestroySceneLoader()
        {
            SceneManager scene = GameManager.instance?.GetComponent<SceneManager>();
            if (scene != null)
            {
                GameObject.Destroy(scene);
            }
        }
        #endregion

        #region Powers and Upgrades

        public int GetCurEquipedSpel(string _type)
        {
            switch (_type.ToLower())
            {
                default:
                    return equipedSoulType;
                case "dive":
                    return equipedDiveType;
                case "shriek":
                    return equipedShriekType;
            }
        }

        public void InitializeFSM()
        {
            On.HeroController.Awake += ReadySpells;
            On.HeroController.Update += HeroController_Update;
            On.HeroController.Update += CharmSpriteSwap;
        }

        public void UnloadFSM()
        {
            On.HeroController.Awake -= ReadySpells;
            On.HeroController.Update -= HeroController_Update;
            On.HeroController.Update -= CharmSpriteSwap;
        }

        private void CharmSpriteSwap(On.HeroController.orig_Update orig, HeroController self)
        {
            if (CharmIconList.Instance != null && CharmIconList.Instance.spriteList != null && CharmIconList.Instance.spriteList.Length > 0)
            {
                if (ogCharmSprites == null || !ogCharmSprites.ContainsKey("dash"))
                {
                    ogCharmSprites = new Dictionary<string, Sprite>();
                    ogCharmSprites.Add("dash", CharmIconList.Instance.spriteList[31]);
                    ogCharmSprites.Add("shield", CharmIconList.Instance.spriteList[38]);
                }
                if (CharmIconList.Instance != null && CharmIconList.Instance.spriteList != null && spritesBundle.ContainsKey("charmupgrades"))
                {
                    if (spritesBundle["charmupgrades"].Contains("dashpro"))
                    {
                        CharmIconList.Instance.spriteList[31] = (upgradedCharms[0]) ? spritesBundle["charmupgrades"].LoadAsset<Sprite>("dashpro") : ogCharmSprites["dash"];
                    }
                    if (spritesBundle["charmupgrades"].Contains("deeshield"))
                    {
                        CharmIconList.Instance.spriteList[38] = (upgradedCharms[1]) ? spritesBundle["charmupgrades"].LoadAsset<Sprite>("deeshield") : ogCharmSprites["shield"];
                    }
                }
            }
            orig(self);
        }

        public void SetKnightProSkin()
        {
            proSkinUsing = true;
            tk2dSprite sprite = HeroController.instance.GetComponent<tk2dSprite>();
            if (sprite != null) {
                SetSkinOnTK2DSprite(sprite, "Base", "Gen-Knight");
            }
            HeroController.instance.gameObject.FindGameObjectInChildren("SD Crystal Burst GL").GetComponent<ParticleSystemRenderer>().material.mainTexture = null;
            HeroController.instance.gameObject.FindGameObjectInChildren("SD Crystal Burst GR").GetComponent<ParticleSystemRenderer>().material.mainTexture = null;
            HeroController.instance.gameObject.FindGameObjectInChildren("SD Crystal Burst W").GetComponent<ParticleSystemRenderer>().material.mainTexture = null;

            PronoesProMod.Instance.LocalSaveData.proKnightSkin = true;

            OnSaveLocal();

        }

        public void SetSkinOnTK2DSprite(tk2dSprite sprite, string name, string skinPartName, bool loadFromOgs = false)
        {
            if (sprite == null)
            {
                return;
            }

            Texture s = sprite.GetCurrentSpriteDef().material.mainTexture, skin;

            if (!ogKnightSprite.ContainsKey(name) || ogKnightSprite[name] != null)
            {
                return;
            }

            if (!loadFromOgs)
            {
                ogKnightSprite.Add(name, s);
                if (!PronoesProMod.Instance.skinsBundle.ContainsKey("skins"))
                {
                    return;
                }
                skin = PronoesProMod.Instance.skinsBundle["skins"].LoadAsset<Texture>(skinPartName);
            } else {
                skin = ogKnightSprite[name];
            }
            sprite.GetCurrentSpriteDef().material.mainTexture = skin;

        }

        public void RemoveKnightProSkin()
        {
            tk2dSprite sprite = HeroController.instance.GetComponent<tk2dSprite>();
            if (sprite != null)
            {
                SetSkinOnTK2DSprite(sprite, "Base", "Gen-Knight", true);
            }
            HeroController.instance.gameObject.FindGameObjectInChildren("SD Crystal Burst GL").GetComponent<ParticleSystemRenderer>().material.mainTexture = null;
            HeroController.instance.gameObject.FindGameObjectInChildren("SD Crystal Burst GR").GetComponent<ParticleSystemRenderer>().material.mainTexture = null;
            HeroController.instance.gameObject.FindGameObjectInChildren("SD Crystal Burst W").GetComponent<ParticleSystemRenderer>().material.mainTexture = null;

        }

        #region spells
        private static void ReadySpells(On.HeroController.orig_Awake orig, HeroController self)
        {
            // Call orig so the original OnEnable function happens - otherwise things will break
            orig(self);

            Instance.newSpells = new Dictionary<string, GameObject>();
            Instance.spellUsedTimes = new int[Instance.spellNames.Length];
            Instance.spellAmmount = new int[Instance.spellNames.Length];

            Instance.CreateSpell(0, 1, self, ref Instance.newSpells);
            Instance.CreateSpell(1, 6, self, ref Instance.newSpells);
            Instance.CreateSpell(2, 2, self, ref Instance.newSpells);
            Instance.CreateSpell(3, 1, self, ref Instance.newSpells);
            Instance.CreateSpell(4, 1, self, ref Instance.newSpells);
            Instance.CreateSpell(5, 15, self, ref Instance.newSpells);
            Instance.CreateSpell(6, 6, self, ref Instance.newSpells);
            Instance.CreateSpell(7, 3, self, ref Instance.newSpells);
            Instance.CreateSpell(8, 4, self, ref Instance.newSpells);
            Instance.CreateSpell(9, 20, self, ref Instance.newSpells);
            Instance.CreateSpell(10, 2, self, ref Instance.newSpells);
            Instance.CreateSpell(11, 2, self, ref Instance.newSpells);

        }

        public void CreateSpell(int type, int ammount, HeroController self, ref Dictionary<string, GameObject> GOList)
        {
            AudioSource s = self.GetComponent<AudioSource>();


            if (GOBundle.ContainsKey("newattacks"))
            {
                GameObject prefav = GOBundle["newattacks"].LoadAsset<GameObject>(spellNames[type]);
                DamageEnemies dmg;
                DamageHero heroDmg;
                AppleAttack apple;

                if (prefav != null)
                {
                    spellAmmount[type] = ammount;
                    for (int i = 0; i < ammount; i++)
                    {
                        GameObject spell = GameObject.Instantiate(prefav, self.transform.position, Quaternion.identity);
                        GameObject.DontDestroyOnLoad(spell);
                        GOList.Add(spellNames[type] + i.ToString(), spell);
                        DeactivateAfter deact;
                        SawbladeAttack saw;

                        foreach (AudioSource audio in spell.GetComponents<AudioSource>())
                        {
                            audio.outputAudioMixerGroup = s.outputAudioMixerGroup;
                        }
                        foreach (AudioSource audio in spell.GetComponentsInChildren<AudioSource>())
                        {
                            audio.outputAudioMixerGroup = s.outputAudioMixerGroup;
                        }

                        switch (type)
                        {
                            case 0:
                                saw = spell.AddComponent<SawbladeAttack>();
                                saw.dmgs = new List<DamageEnemies>();
                                for (int c = 0; c < spell.transform.childCount; c++)
                                {
                                    dmg = spell.transform.GetChild(c).gameObject.AddComponent<DamageEnemies>();
                                    saw.dmgs.Add(dmg);
                                    dmg.damageDealt = Math.Max(1, PlayerData.instance.nailDamage / 10);
                                    dmg.circleDirection = true;
                                    dmg.attackType = AttackTypes.Spell;
                                    dmg.ignoreInvuln = true;
                                    dmg.magnitudeMult = 1;
                                }
                                break;
                            case 1:

                                spell.AddComponent<FlyingNailAttack>();

                                dmg = spell.AddComponent<DamageEnemies>();
                                dmg.damageDealt = Math.Max(1, PlayerData.instance.nailDamage / 3);
                                dmg.attackType = AttackTypes.Spell;
                                dmg.ignoreInvuln = false;
                                dmg.magnitudeMult = 1;

                                break;
                            case 2:

                                spell.AddComponent<BouncingObject>();

                                dmg = spell.transform.GetChild(0).gameObject.AddComponent<DamageEnemies>();
                                dmg.damageDealt = Math.Max(1, PlayerData.instance.nailDamage / 3);
                                dmg.attackType = AttackTypes.Spell;
                                dmg.ignoreInvuln = false;
                                dmg.magnitudeMult = 1;

                                apple = dmg.gameObject.AddComponent<AppleAttack>();
                                apple.targetGO = spell;

                                break;
                            case 3:
                                spell.AddComponent<FallingNailAttack>();
                                for (int c = 0; c < spell.transform.childCount; c++)
                                {
                                    dmg = spell.transform.GetChild(c).gameObject.AddComponent<DamageEnemies>();
                                    dmg.damageDealt = Math.Max(1, PlayerData.instance.nailDamage / 4);
                                    dmg.attackType = AttackTypes.Spell;
                                    dmg.ignoreInvuln = false;
                                    dmg.magnitudeMult = 1;
                                }
                                break;
                            case 4:
                                deact = spell.AddComponent<DeactivateAfter>();
                                deact.timer = 0.75f;

                                dmg = spell.transform.GetChild(0).gameObject.AddComponent<DamageEnemies>();
                                dmg.damageDealt = Math.Max(1, PlayerData.instance.nailDamage);
                                dmg.direction = 0;
                                dmg.attackType = AttackTypes.Nail;
                                dmg.magnitudeMult = 1;
                                dmg.ignoreInvuln = false;

                                break;
                            case 5:

                                GoAlongWall goAlong = spell.AddComponent<GoAlongWall>();
                                goAlong.normalMoveSpeed = 1.5f;
                                goAlong.fastMoveSpeed = 5f;

                                saw = spell.AddComponent<SawbladeAttack>();
                                saw.attackAnim = "Sawblade_shrink";
                                saw.collisionMult = 1f;
                                saw.destroyTime = 8.5f;
                                saw.dmgs = new List<DamageEnemies>();

                                dmg = spell.transform.GetChild(0).gameObject.AddComponent<DamageEnemies>();
                                dmg.damageDealt = 1;
                                dmg.attackType = AttackTypes.Spell;
                                dmg.ignoreInvuln = false;
                                dmg.magnitudeMult = 1;
                                saw.dmgs.Add(dmg);
                                break;
                            case 6:

                                deact = spell.AddComponent<DeactivateAfter>();
                                deact.timer = 0.5f;

                                for (int c = 0; c < spell.transform.childCount; c++)
                                {
                                    dmg = spell.transform.GetChild(c).gameObject.AddComponent<DamageEnemies>();
                                    dmg.damageDealt = PlayerData.instance.nailDamage;
                                    dmg.attackType = AttackTypes.Spell;
                                    dmg.ignoreInvuln = false;
                                    dmg.magnitudeMult = 0.1f;
                                }

                                break;
                            case 7:

                                dmg = spell.transform.GetChild(0).gameObject.AddComponent<DamageEnemies>();
                                dmg.damageDealt = PlayerData.instance.nailDamage * 2;
                                dmg.attackType = AttackTypes.Spell;
                                dmg.ignoreInvuln = false;
                                dmg.magnitudeMult = 0.1f;

                                DeeExplosionAttack deexplosion = dmg.gameObject.AddComponent<DeeExplosionAttack>();
                                deexplosion.rb = deexplosion.GetComponent<Rigidbody2D>();
                                deexplosion.dissapearOnCollision = true;

                                break;
                            case 8:
                                deact = spell.AddComponent<DeactivateAfter>();
                                deact.timer = 0.75f;

                                dmg = spell.transform.GetChild(0).gameObject.AddComponent<DamageEnemies>();
                                dmg.damageDealt = 5;
                                dmg.attackType = AttackTypes.Spell;
                                dmg.ignoreInvuln = false;
                                dmg.magnitudeMult = 1f;
                                break;
                            case 9:

                                deact = spell.AddComponent<DeactivateAfter>();
                                deact.timer = 3f;

                                dmg = spell.transform.GetChild(0).gameObject.AddComponent<DamageEnemies>();
                                dmg.damageDealt = PlayerData.instance.nailDamage / 10;
                                dmg.attackType = AttackTypes.Spell;
                                dmg.ignoreInvuln = false;
                                dmg.magnitudeMult = 1f;

                                apple = dmg.gameObject.AddComponent<AppleAttack>();
                                apple.targetGO = spell;
                                break;
                            case 10:

                                saw = spell.AddComponent<SawbladeAttack>();
                                saw.collisionMult = 1f;
                                saw.destroyTime = 4f;
                                saw.maxCollide = 15;
                                saw.dmgs = new List<DamageEnemies>();

                                for (int child = 0; child < spell.transform.childCount; child++)
                                {
                                    dmg = spell.transform.GetChild(child).gameObject.AddComponent<DamageEnemies>();
                                    dmg.damageDealt = PlayerData.instance.nailDamage / 2;
                                    dmg.attackType = AttackTypes.Spell;
                                    dmg.ignoreInvuln = false;
                                    dmg.magnitudeMult = 1;
                                    saw.dmgs.Add(dmg);
                                }
                                break;
                            case 11:
                                dmg = spell.AddComponent<DamageEnemies>();
                                dmg.attackType = AttackTypes.Spell;
                                dmg.circleDirection = true;
                                dmg.damageDealt = 15;
                                dmg.ignoreInvuln = true;

                                heroDmg = spell.AddComponent<DamageHero>();
                                heroDmg.damageDealt = 1;
                                heroDmg.shadowDashHazard = false;

                                deact = spell.AddComponent<DeactivateAfter>();
                                deact.timer = 0.75f;

                                {
                                    QuickExplosion explodies = spell.AddComponent<QuickExplosion>();
                                }

                                break;
                        }
                    }
                }
            }
        }

        private void HeroController_Update(On.HeroController.orig_Update orig, HeroController self)
        {
            // Call orig so the original OnEnable function happens - otherwise things will break
            orig(self);

            // Execute your code here - I like making it a separate function but you could just do it all here if you prefer
            if (self.gameObject.name == "Knight")
            {

                if (!loadedKnightPowersAndSkins)
                {
                    loadedKnightPowersAndSkins = true;

                    soulType = soul;
                    shriekType = shriek;
                    diveType = dive;

                    if (proSkinUsing)
                    {
                        SetKnightProSkin();
                    }
                }

                Instance.IncreaseCDashSpeed(self.superDash);
                Instance.UpgradeDreamShield(self.fsm_orbitShield);
                Instance.SpellSwap(self.spellControl);

            }

        }

        public int diveType, soulType, shriekType;

        public void SpellSwap(PlayMakerFSM fsm)
        {
            if (ogDiveStates == null)
            {
                ogDiveStates = new Dictionary<string, FsmState>();
                for (int i = 0; i < ogDiveStateNames.Length; i++)
                {
                    ogDiveStates.Add(ogDiveStateNames[i], new FsmState(fsm.GetFsmState(ogDiveStateNames[i])));
                }
            }
            GameObject createdSpell;
            switch (equipedDiveType)
            {
                default:
                case 0:
                    if (diveType != 0)
                    {
                        Log("Normal dive... buuuh");
                        RestoreDive(fsm);
                        diveType = 0;
                    }
                    break;
                case 1:
                    if (diveType != 1)
                    {
                        RestoreDive(fsm);
                        if (unlockableDives[equipedDiveType-1].unlocked)
                        {
                            Log("Super dive with sawblades!");
                            if (newSpells.ContainsKey(spellNames[0] + "0"))
                            {
                                Log("Sawblades!");
                                fsm.RemoveAction("Quake1 Land", 12);
                                fsm.InsertMethod("Quake1 Land", () =>
                                {
                                    createdSpell = ActivateSpell(0);
                                    createdSpell.transform.localScale = new Vector3(1, 1, 1);
                                    createdSpell.SendMessage("SetCollisionsMultiplier", 1f);
                                    Log("It should work...");
                                }, 12);

                                fsm.RemoveAction("Q2 Land", 11);
                                fsm.InsertMethod("Q2 Land", () =>
                                {
                                    createdSpell = ActivateSpell(0);
                                    createdSpell.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                                    createdSpell.SendMessage("SetCollisionsMultiplier", 2f);
                                    Log("It should work...");
                                }, 11);
                                fsm.RemoveAction("Q2 Land", 8);
                                fsm.RemoveAction("Q2 Pillar", 3);
                                fsm.RemoveAction("Q2 Pillar", 2);
                            }
                        }
                        else
                        {
                            Log("Can't use, not unlocked");
                            equipedDiveType = 2;
                        }
                        diveType = 1;
                    }
                    break;
                case 2:
                    if (diveType != 2)
                    {
                        RestoreDive(fsm);
                        if (unlockableDives[equipedDiveType-1].unlocked)
                        {
                            Log("Nailmaster dive!");
                            if (newSpells.ContainsKey(spellNames[0] + "0"))
                            {

                                fsm.InsertMethod("Quake Antic", () =>
                                {
                                    playerFalling = true;
                                    createdSpell = ActivateSpell(3);
                                    createdSpell.GetComponent<AudioSource>().Play();
                                }, 2);

                                Log("Flying Nails!");

                                fsm.InsertMethod("Quake1 Land", () =>
                                {
                                    playerFalling = false;
                                }, 11);

                                fsm.RemoveAction("Q2 Land", 11);
                                fsm.InsertMethod("Q2 Land", () =>
                                {
                                    playerFalling = false;
                                    createdSpell = ActivateSpell(4);
                                    createdSpell.transform.position = HeroController.instance.transform.position + new Vector3(0, -1f, 0);
                                    createdSpell.GetComponent<Animator>().SetTrigger("Appear");
                                    createdSpell.GetComponent<AudioSource>().Play();
                                    Log("It should work...");
                                }, 11);
                                fsm.RemoveAction("Q2 Land", 8);
                                fsm.RemoveAction("Q2 Pillar", 3);
                                fsm.RemoveAction("Q2 Pillar", 2);

                                fsm.AddMethod("Spell End", () =>
                                {
                                    playerFalling = false;
                                });
                            }
                        }
                        else
                        {
                            Log("Can't use, not unlocked");
                            equipedDiveType = 3;
                        }
                        diveType = 2;
                    }
                    break;
                case 3:
                    if (diveType != 3)
                    {
                        RestoreDive(fsm);
                        if (unlockableDives[equipedDiveType-1].unlocked)
                        {
                            Log("Nailmaster dive!");
                            if (newSpells.ContainsKey(spellNames[0] + "0"))
                            {

                                Log("Apple Pies!");

                                fsm.RemoveAction("Quake1 Land", 11);
                                fsm.InsertMethod("Quake1 Land", () =>
                                {
                                    createdSpell = ActivateSpell(8);
                                    createdSpell.transform.position = HeroController.instance.transform.position + new Vector3(0, -1f, 0);
                                    createdSpell.GetComponentInChildren<Animator>().SetTrigger("Appear");
                                    createdSpell.transform.localScale = new Vector3(-Mathf.Abs(createdSpell.transform.localScale.x) / 2f, 0.5f, 0.5f);

                                    createdSpell = ActivateSpell(8);
                                    createdSpell.transform.position = HeroController.instance.transform.position + new Vector3(0, -1f, 0);
                                    createdSpell.GetComponentInChildren<Animator>().SetTrigger("Appear");
                                    createdSpell.transform.localScale = new Vector3(-Mathf.Abs(createdSpell.transform.localScale.x) / 2f, 0.5f, 0.5f);
                                }, 11);

                                fsm.RemoveAction("Q2 Land", 11);
                                fsm.InsertMethod("Q2 Land", () =>
                                {
                                    createdSpell = ActivateSpell(8);
                                    createdSpell.transform.position = HeroController.instance.transform.position + new Vector3(0, -1f, 0);
                                    createdSpell.GetComponentInChildren<Animator>().SetTrigger("Appear");

                                    createdSpell = ActivateSpell(8);
                                    createdSpell.transform.position = HeroController.instance.transform.position + new Vector3(0, -1f, 0);
                                    createdSpell.GetComponentInChildren<Animator>().SetTrigger("Appear");
                                    createdSpell.transform.localScale = new Vector3(-Mathf.Abs(createdSpell.transform.localScale.x), 1, 1);
                                    Log("It should work...");
                                }, 11);

                                fsm.RemoveAction("Q2 Land", 8);
                                fsm.RemoveAction("Q2 Pillar", 3);
                                fsm.RemoveAction("Q2 Pillar", 2);

                            }
                        }
                        else
                        {
                            Log("Can't use, not unlocked");
                            equipedDiveType = 0;
                        }
                        diveType = 3;
                    }
                    break;
            }

            if (ogSoulStates == null)
            {
                ogSoulStates = new Dictionary<string, FsmState>();
                for (int i = 0; i < ogSoulStateNames.Length; i++)
                {
                    ogSoulStates.Add(ogSoulStateNames[i], new FsmState(fsm.GetFsmState(ogSoulStateNames[i])));
                }
            }
            switch (equipedSoulType)
            {
                default:
                case 0:
                    if (soulType != 0)
                    {
                        Log("Normal soul... buuuh");
                        RestoreSoul(fsm);
                        soulType = 0;
                    }
                    break;
                case 1:
                    if (soulType != 1)
                    {
                        RestoreSoul(fsm);
                        if (unlockableSouls[equipedSoulType-1].unlocked)
                        {
                            Log("Sawblade soul!");

                            fsm.RemoveAction("Fireball 1", 3);
                            fsm.InsertMethod("Fireball 1", () =>
                            {
                                createdSpell = ActivateSpell(5);
                                createdSpell.transform.position = HeroController.instance.transform.position;
                                createdSpell.GetComponent<AudioSource>().Play();
                                createdSpell.SendMessage("ChangeMoveDir", new Vector2(playerFaceLeft ? 1 : -1, 3));
                            }, 3);

                            fsm.RemoveAction("Fireball 2", 3);
                            fsm.InsertMethod("Fireball 2", () =>
                            {
                                createdSpell = ActivateSpell(5);
                                createdSpell.transform.position = HeroController.instance.transform.position;
                                createdSpell.GetComponent<AudioSource>().Play();
                                createdSpell.SendMessage("ChangeMoveDir", new Vector2(-1, 3));

                                createdSpell = ActivateSpell(5);
                                createdSpell.transform.position = HeroController.instance.transform.position;
                                createdSpell.GetComponent<AudioSource>().Play();
                                createdSpell.SendMessage("ChangeMoveDir", new Vector2(1, 3));
                                createdSpell.SendMessage("ChangeRotateDir", -90);
                            }, 3);

                            fsm.RemoveAction("Fireball Recoil", 9);
                        }
                    }
                    else{
                        Log("Not bought :C");
                        equipedSoulType = 2;
                    }
                            soulType = 1;
                    break;
                case 2:
                    if (soulType != 2)
                    {
                        RestoreSoul(fsm);
                        if (unlockableSouls[equipedSoulType-1].unlocked)
                        {
                            Log("Nailmaster soul!");

                            fsm.RemoveAction("Fireball 1", 3);
                            fsm.InsertMethod("Fireball 1", () =>
                            {
                                createdSpell = ActivateSpell(1);
                                createdSpell.GetComponent<AudioSource>().Play();
                                FlyingNailAttack fly = createdSpell.GetComponent<FlyingNailAttack>();
                                fly.destination = HeroController.instance.transform.position + new Vector3((playerFaceLeft ? -1 : 1) * 5, 0);
                                fly.flyDirection = new Vector2((playerFaceLeft ? 1 : -1) * 10, 5);

                                createdSpell = ActivateSpell(1);
                                fly = createdSpell.GetComponent<FlyingNailAttack>();
                                fly.destination = HeroController.instance.transform.position + new Vector3((playerFaceLeft ? -1 : 1) * 5, 0);
                                fly.flyDirection = new Vector2((playerFaceLeft ? 1 : -1) * 10, 0);

                                createdSpell = ActivateSpell(1);
                                fly = createdSpell.GetComponent<FlyingNailAttack>();
                                fly.destination = HeroController.instance.transform.position + new Vector3((playerFaceLeft ? -1 : 1) * 5, 0);
                                fly.flyDirection = new Vector2((playerFaceLeft ? 1 : -1) * 10, -5);
                            }, 3);

                            fsm.RemoveAction("Fireball 2", 3);
                            fsm.InsertMethod("Fireball 2", () =>
                            {
                                createdSpell = ActivateSpell(1);
                                createdSpell.GetComponent<AudioSource>().Play();
                                FlyingNailAttack fly = createdSpell.GetComponent<FlyingNailAttack>();
                                fly.destination = HeroController.instance.transform.position + new Vector3((playerFaceLeft ? -1 : 1) * 15, 0);
                                fly.flyDirection = new Vector2((playerFaceLeft ? 1 : -1) * 30, 15);

                                createdSpell = ActivateSpell(1);
                                fly = createdSpell.GetComponent<FlyingNailAttack>();
                                fly.destination = HeroController.instance.transform.position + new Vector3((playerFaceLeft ? -1 : 1) * 15, 0);
                                fly.flyDirection = new Vector2((playerFaceLeft ? 1 : -1) * 30, 0);

                                createdSpell = ActivateSpell(1);
                                fly = createdSpell.GetComponent<FlyingNailAttack>();
                                fly.destination = HeroController.instance.transform.position + new Vector3((playerFaceLeft ? -1 : 1) * 15, 0);
                                fly.flyDirection = new Vector2((playerFaceLeft ? 1 : -1) * 30, -15);

                            }, 3);

                            fsm.RemoveAction("Fireball Recoil", 9);
                        }else
                        {
                            Log("Not bought :C");
                            equipedSoulType = 3;
                        }
                        soulType = 2;
                    }
                    break;
                case 3:
                    if (soulType != 3)
                    {
                        RestoreSoul(fsm);
                        if (unlockableSouls[equipedSoulType-1].unlocked)
                        {
                            Log("Apple slices for everyone!");

                            fsm.RemoveAction("Fireball 1", 3);
                            fsm.InsertMethod("Fireball 1", () =>
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    createdSpell = ActivateSpell(9);

                                    createdSpell.transform.position = HeroController.instance.transform.position;
                                    createdSpell.GetComponent<Rigidbody2D>().velocity = new Vector2((playerFaceLeft ? -1 : 1) * UnityEngine.Random.Range(3f, 7f), 1);
                                    createdSpell.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                                    createdSpell.transform.GetChild(0).localPosition = Vector3.zero;
                                    createdSpell.transform.GetChild(0).rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-360, 360));
                                }
                            }, 3);

                            fsm.RemoveAction("Fireball 2", 3);
                            fsm.InsertMethod("Fireball 2", () =>
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    createdSpell = ActivateSpell(9);

                                    createdSpell.transform.position = HeroController.instance.transform.position;
                                    createdSpell.GetComponent<Rigidbody2D>().velocity = new Vector2((playerFaceLeft ? -1 : 1) * UnityEngine.Random.Range(7f, 15f), 1);
                                    createdSpell.transform.localScale = new Vector3(1, 1, 1);
                                    createdSpell.transform.GetChild(0).localPosition = Vector3.zero;
                                    createdSpell.transform.GetChild(0).rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-360, 360));
                                }

                            }, 3);

                            fsm.RemoveAction("Fireball Recoil", 9);
                        }
                        else
                        {
                            Log("Not bought :C");
                            equipedSoulType = 4;
                        }
                        soulType = 3;
                    }
                    break;
                case 4:
                    if (soulType != 4)
                    {
                        RestoreSoul(fsm);

                        if (unlockableSouls[equipedSoulType-1].unlocked)
                        {
                            Log("Dee-mon souls!");

                            fsm.RemoveAction("Fireball 1", 3);
                            fsm.InsertMethod("Fireball 1", () =>
                            {
                                createdSpell = ActivateSpell(7);
                                createdSpell.transform.position = HeroController.instance.transform.position;
                                createdSpell.GetComponentInChildren<ParticleSystem>().Play();
                            //createdSpell.GetComponent<AudioSource>().Play();
                            createdSpell.transform.GetChild(0).gameObject.SetActive(true);
                                createdSpell.transform.GetChild(0).localPosition = Vector3.zero;
                                createdSpell.transform.localScale = new Vector3(playerFaceLeft ? -1 : 1, 1, 1);

                                DeeExplosionAttack atk = createdSpell.transform.GetChild(0).GetComponent<DeeExplosionAttack>();
                                if (atk != null)
                                {
                                    atk.ChangeMoveDir(new Vector2(playerFaceLeft ? 7f : -7f, 3f));
                                    atk.ResetAttack();
                                }
                            }, 3);

                            fsm.RemoveAction("Fireball 2", 3);
                            fsm.InsertMethod("Fireball 2", () =>
                            {

                                createdSpell = ActivateSpell(7);
                                createdSpell.transform.position = HeroController.instance.transform.position;
                                createdSpell.GetComponentInChildren<ParticleSystem>().Play();
                            //createdSpell.GetComponent<AudioSource>().Play();
                            createdSpell.transform.GetChild(0).gameObject.SetActive(true);
                                createdSpell.transform.GetChild(0).localPosition = Vector3.zero;
                                createdSpell.transform.localScale = new Vector3(playerFaceLeft ? -1 : 1, 1, 1);

                                DeeExplosionAttack atk = createdSpell.transform.GetChild(0).GetComponent<DeeExplosionAttack>();
                                if (atk != null)
                                {
                                    atk.ChangeMoveDir(new Vector2(playerFaceLeft ? 7f : -7f, 3f));
                                    atk.ResetAttack();
                                }
                            }, 3);

                            fsm.RemoveAction("Fireball Recoil", 9);
                        }else
                        {
                            Log("Dee is not by your side yet, but maybe some day!");
                            equipedSoulType = 0;
                        }
                        soulType = 4;
                    }
                    break;
            }

            if (ogShriekStates == null)
            {
                ogShriekStates = new Dictionary<string, FsmState>();
                for (int i = 0; i < ogShriekStateNames.Length; i++)
                {
                    ogShriekStates.Add(ogShriekStateNames[i], new FsmState(fsm.GetFsmState(ogShriekStateNames[i])));
                }
            }
            switch (equipedShriekType)
            {
                default:
                case 0:
                    if (shriekType != 0)
                    {
                        Log("Normal shriek... buuuh");
                        RestoreShriek(fsm);
                        shriekType = 0;
                    }
                    break;
                case 1:
                    if (shriekType != 1)
                    {
                        RestoreShriek(fsm);
                        if (unlockableShrieks[equipedShriekType - 1].unlocked)
                        {
                            Log("Sawblade shriek!");

                            fsm.RemoveAction("Scream Burst 1", 7);
                            fsm.RemoveAction("Scream Burst 1", 6);

                            fsm.RemoveAction("Scream Burst 1", 2);
                            fsm.InsertMethod("Scream Burst 1", () =>
                            {
                                createdSpell = ActivateSpell(10);

                                if (createdSpell.activeInHierarchy)
                                {
                                    createdSpell.SetActive(false);
                                }

                                createdSpell.SetActive(true);
                                createdSpell.transform.position = HeroController.instance.transform.position + new Vector3(0, 2, 0);
                                createdSpell.GetComponent<Animator>().SetInteger("shoutLvl", 0);
                            }, 2);

                            fsm.RemoveAction("Scream Burst 2", 8);
                            fsm.RemoveAction("Scream Burst 2", 7);

                            fsm.RemoveAction("Scream Burst 2", 3);
                            fsm.InsertMethod("Scream Burst 2", () =>
                            {
                                createdSpell = ActivateSpell(10);

                                if (createdSpell.activeInHierarchy)
                                {
                                    createdSpell.SetActive(false);
                                }

                                createdSpell.SetActive(true);

                                createdSpell.transform.position = HeroController.instance.transform.position + new Vector3(0, 2, 0);
                                createdSpell.GetComponent<Animator>().SetInteger("shoutLvl", 1);
                            }, 3);
                        }else{
                            Log("Can't scream that way, not bought the vocal chords to do it");
                            equipedShriekType = 2;
                        }
                        shriekType = 1;
                    }
                    break;
                case 2:
                    if (shriekType != 2)
                    {
                        RestoreShriek(fsm);

                        if (unlockableShrieks[equipedShriekType - 1].unlocked)
                        {
                            Log("Nailmaster shriek!");

                            fsm.RemoveAction("Scream Burst 1", 7);
                            fsm.RemoveAction("Scream Burst 1", 6);

                            fsm.RemoveAction("Scream Burst 1", 2);
                            fsm.InsertMethod("Scream Burst 1", () =>
                            {
                                createdSpell = ActivateSpell(6);

                                createdSpell.transform.position = HeroController.instance.transform.position + new Vector3(0, 2, 0);
                                createdSpell.GetComponent<Animator>().Play("NailBarrage_attack");
                            }, 2);

                            fsm.RemoveAction("Scream Burst 2", 8);
                            fsm.RemoveAction("Scream Burst 2", 7);

                            fsm.RemoveAction("Scream Burst 2", 3);
                            fsm.InsertMethod("Scream Burst 2", () =>
                            {
                                createdSpell = ActivateSpell(6);

                                createdSpell.transform.position = HeroController.instance.transform.position + new Vector3(0, 2, 0);
                                createdSpell.transform.localScale = new Vector3(-1.25f, 1.25f, 1.25f);
                                createdSpell.GetComponent<Animator>().Play("NailBarrage_attack");

                                createdSpell = ActivateSpell(6);

                                createdSpell.transform.localScale = new Vector3(-2f, 2f, 2f);
                                createdSpell.transform.position = HeroController.instance.transform.position + new Vector3(0, 2, 0);
                                createdSpell.GetComponent<Animator>().Play("NailBarrage_attack");
                            }, 3);
                        }else
                        {
                            Log("Can't scream that way, not bought the vocal chords to do it");
                            equipedShriekType = 3;
                        }
                        shriekType = 2;
                    }
                    break;
                case 3:
                    if (shriekType != 3)
                    {
                        RestoreShriek(fsm);
                        if (unlockableShrieks[equipedShriekType - 1].unlocked)
                        {
                            Log("Apple shriek!");

                            fsm.RemoveAction("Scream Burst 1", 7);
                            fsm.RemoveAction("Scream Burst 1", 6);

                            fsm.RemoveAction("Scream Burst 1", 2);
                            fsm.InsertMethod("Scream Burst 1", () =>
                            {
                                createdSpell = ActivateSpell(2);

                                createdSpell.transform.position = HeroController.instance.transform.position;
                                createdSpell.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 50);
                                createdSpell.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
                            }, 2);

                            fsm.RemoveAction("Scream Burst 2", 8);
                            fsm.RemoveAction("Scream Burst 2", 7);

                            fsm.RemoveAction("Scream Burst 2", 3);
                            fsm.InsertMethod("Scream Burst 2", () =>
                            {
                                createdSpell = ActivateSpell(2);

                                createdSpell.transform.position = HeroController.instance.transform.position;
                                createdSpell.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 50);
                            }, 3);
                        }
                        else
                        {
                            equipedShriekType = 0;
                            Log("Can't scream that way, not bought the vocal chords to do it");
                        }

                        shriekType = 3;
                    }
                    break;
            }

        }

        public GameObject ActivateSpell(int type)
        {
            if (type < spellNames.Length)
            {
                GameObject targetSpell = newSpells[spellNames[type] + spellUsedTimes[type]];
                targetSpell.SetActive(true);
                spellUsedTimes[type] = (spellUsedTimes[type] + 1) % spellAmmount[type];
                return targetSpell;
            }
            return null;
        }

        public void RestoreDive(PlayMakerFSM fsm)
        {

            for (int i = 0; i < ogDiveStateNames.Length; i++)
            {
                if (ogDiveStates.ContainsKey(ogDiveStateNames[i])) {
                    RestoreAction(fsm, ogDiveStates[ogDiveStateNames[i]]);
                }
            }

        }

        public void RestoreSoul(PlayMakerFSM fsm)
        {
            for (int i = 0; i < ogSoulStateNames.Length; i++)
            {
                if (ogSoulStates.ContainsKey(ogSoulStateNames[i]))
                {
                    RestoreAction(fsm, ogSoulStates[ogSoulStateNames[i]]);
                }
            }
        }

        public void RestoreShriek(PlayMakerFSM fsm)
        {
            for (int i = 0; i < ogShriekStateNames.Length; i++)
            {
                if (ogShriekStates.ContainsKey(ogShriekStateNames[i]))
                {
                    RestoreAction(fsm, ogShriekStates[ogShriekStateNames[i]]);
                }
            }
        }

        public void RestoreAction(PlayMakerFSM fsm, FsmState og)
        {
            int actionLength;
            for (int i = 0; i < fsm.FsmStates.Length; i++)
            {
                actionLength = fsm.FsmStates[i].Actions.Length;
                if (fsm.FsmStates[i].Name == og.Name)
                {
                    for (int a = actionLength - 1; a >= 0; a--)
                    {
                        fsm.RemoveAction(fsm.FsmStates[i].Name, a);
                    }
                    for (int a = 0; a < og.Actions.Length; a++)
                    {
                        fsm.AddAction(fsm.FsmStates[i].Name, og.Actions[a]);
                    }
                    //Log("Restored state " + og.Name);
                }
            }
        }
        #endregion

        public void IncreaseCDashSpeed(PlayMakerFSM fsm)
        {
            if (upgradedCharms[0])
            {
                fsm.FsmVariables.FindFsmFloat("Charge Time").Value = (PlayerData.instance.GetBool("equippedCharm_31") ? 0.1f : 0.8f);
                fsm.FsmVariables.FindFsmFloat("Superdash Speed").Value = (PlayerData.instance.GetBool("equippedCharm_31") ? 75f : 30f);
                fsm.FsmVariables.FindFsmFloat("Superdash Speed neg").Value = -fsm.FsmVariables.FindFsmFloat("Superdash Speed").Value;
                fsm.FsmVariables.FindFsmFloat("Y Speed").Value = (PlayerData.instance.GetBool("equippedCharm_31") ? 13f : 0f);
            }
        }

        public void UpgradeDreamShield(PlayMakerFSM fsm)
        {

            if (upgradedCharms[1] && !BossSequenceController.BoundCharms)
            {
                if (ogCharmStates == null)
                {
                    ogCharmStates = new Dictionary<string, FsmState>();
                }
                if (!ogCharmStates.ContainsKey("DreamShield_spawn"))
                {
                    ogCharmStates.Add("DreamShield_spawn", new FsmState(fsm.GetFsmState("Spawn")));
                    ogCharmStates.Add("DreamShield_idle", new FsmState(fsm.GetFsmState("Idle")));
                    ogCharmStates.Add("DreamShield_slash", new FsmState(fsm.GetFsmState("Send Slash Event")));
                }

                //Log("Dee Shield control");

                foreach (string key in ogCharmStates.Keys)
                {
                    RestoreAction(fsm, ogCharmStates[key]);
                }

                fsm.RemoveAction("Spawn", 0);
                fsm.RemoveAction("Spawn", 0);
                fsm.RemoveAction("Spawn", 0);

                fsm.AddMethod("Spawn", () =>
                {
                    //Log("Create Dee Shield");
                    CreateDeeShield();
                });

                fsm.AddMethod("Idle", () =>
                {
                    CreateDeeShield();

                    if (charmSpawns == null)
                    {
                        charmSpawns = new Dictionary<string, Transform>();
                    }
                    if (charmSpawns.ContainsKey("DeeShield"))
                    {
                        charmSpawns["DeeShield"].position = HeroController.instance.transform.position;
                    }
                });

                fsm.AddMethod("Send Slash Event", () => {

                    CreateDeeShield();

                    if (charmSpawns.ContainsKey("DeeShield"))
                    {
                        DeeShield shield = charmSpawns["DeeShield"].GetComponent<DeeShield>();
                        shield.Slash();
                    }
                });
            }
            else
            {
                if (ogCharmStates != null)
                {
                    foreach (string key in ogCharmStates.Keys)
                    {
                        RestoreAction(fsm, ogCharmStates[key]);
                    }
                }

                if (charmSpawns == null)
                {
                    charmSpawns = new Dictionary<string, Transform>();
                }
                if (charmSpawns.ContainsKey("DeeShield"))
                {
                    charmSpawns["DeeShield"].gameObject.SetActive(false);
                }
            }
        }

        public void CreateDeeShield()
        {
            if (ogDreamShield == null && GameObject.Find("Orbit Shield(Clone)") != null)
            {
                ogDreamShield = GameObject.Find("Orbit Shield(Clone)").transform;
            }
            if (ogDreamShield != null)
            {
                ogDreamShield.gameObject.SetActive(false);
                if (charmSpawns == null)
                {
                    charmSpawns = new Dictionary<string, Transform>();
                }
                if (!charmSpawns.ContainsKey("DeeShield"))
                {
                    if (GOBundle.ContainsKey("newattacks"))
                    {
                        Transform prefav = GOBundle["newattacks"].LoadAsset<GameObject>("DeeShield").transform;
                        Transform shield = GameObject.Instantiate(prefav);
                        shield.gameObject.AddComponent<DeeShield>();
                        shield.gameObject.AddComponent<NonBouncer>();

                        shield.Find("LazerStopper").gameObject.AddComponent<NonBouncer>();

                        charmSpawns.Add("DeeShield", shield);

                    }
                }
                else
                {
                    if (charmSpawns["DeeShield"] == null)
                    {
                        charmSpawns.Remove("DeeShield");
                        if (GOBundle.ContainsKey("newattacks"))
                        {
                            Transform prefav = GOBundle["newattacks"].LoadAsset<GameObject>("DeeShield").transform;
                            Transform shield = GameObject.Instantiate(prefav);
                            shield.gameObject.AddComponent<DeeShield>();
                            shield.gameObject.AddComponent<NonBouncer>();

                            shield.Find("LazerStopper").gameObject.AddComponent<NonBouncer>();

                            charmSpawns.Add("DeeShield", shield);
                        }
                    }
                    else
                    {
                        charmSpawns["DeeShield"].gameObject.SetActive(true);
                    }

                }
            }
        }

        public void CreateDeeShieldProjectile(Vector2 pos, Quaternion rot)
        {
            if (charmSpawns == null)
            {
                charmSpawns = new Dictionary<string, Transform>();
            }

            if (charmSpawns.ContainsKey("DeeShield_proj_0") && charmSpawns["DeeShield_proj_0"] == null) {
                for (int i = 0; i < 3; i++) {
                    charmSpawns.Remove("DeeShield_proj_" + i.ToString());
                }
            }

            if (!charmSpawns.ContainsKey("DeeShield_proj_0"))
            {
                for (int i = 0; i < 3; i++)
                {
                    List<Collider2D> colliders = new List<Collider2D>();

                    Transform prefav = GOBundle["newattacks"].LoadAsset<GameObject>("DeeShield_projectile").transform;
                    Transform shield = GameObject.Instantiate(prefav, pos, rot);

                    DeeShield_Projectile proj = shield.gameObject.AddComponent<DeeShield_Projectile>();
                    shield.gameObject.AddComponent<NonBouncer>();

                    DeeShield_ProjectileCollision[] cols = new DeeShield_ProjectileCollision[2];

                    DamageEnemies dmg = shield.Find("StrongAttack").gameObject.AddComponent<DamageEnemies>();

                    cols[0] = dmg.gameObject.AddComponent<DeeShield_ProjectileCollision>();
                    cols[0].parent = proj.transform;

                    dmg.attackType = AttackTypes.NailBeam;
                    dmg.damageDealt = 2;

                    dmg.gameObject.AddComponent<NonBouncer>();
                    colliders.Add(dmg.GetComponent<Collider2D>());

                    /* Weak attack */
                    dmg = shield.Find("WeakAttack").gameObject.AddComponent<DamageEnemies>();

                    cols[1] = dmg.gameObject.AddComponent<DeeShield_ProjectileCollision>();
                    cols[1].parent = proj.transform;

                    dmg.attackType = AttackTypes.NailBeam;
                    dmg.damageDealt = 1;

                    dmg.gameObject.AddComponent<NonBouncer>();

                    colliders.Add(dmg.GetComponent<Collider2D>());



                    charmSpawns.Add("DeeShield_proj_" + i.ToString(), shield);
                    shield.gameObject.SetActive(i == 0);
                    if (charmProjectileNums == null)
                    {
                        charmProjectileNums = new Dictionary<string, int>();
                        charmProjectileNums.Add("DeeShield", 1);
                    }

                    proj.colliders = colliders.ToArray();

                }
            } else {
                if (charmProjectileNums == null)
                {
                    charmProjectileNums = new Dictionary<string, int>();
                    charmProjectileNums.Add("DeeShield", 0);
                }
                else
                {
                    if (charmProjectileNums.ContainsKey("DeeShield")) {
                        charmProjectileNums["DeeShield"] = (charmProjectileNums["DeeShield"] + 1) % 3;
                    } else {
                        charmProjectileNums.Add("DeeShield", 0);
                    }
                }

                if (charmSpawns.ContainsKey("DeeShield_proj_" + (charmProjectileNums["DeeShield"].ToString())) && charmSpawns["DeeShield_proj_" + (charmProjectileNums["DeeShield"].ToString())] != null) {
                    charmSpawns["DeeShield_proj_" + (charmProjectileNums["DeeShield"].ToString())].gameObject.SetActive(true);
                    charmSpawns["DeeShield_proj_" + (charmProjectileNums["DeeShield"].ToString())].position = pos;
                    charmSpawns["DeeShield_proj_" + (charmProjectileNums["DeeShield"].ToString())].rotation = rot;
                }
            }
        }

        #endregion

        #region Saving and Loading

        #region Global varialbles
        public static PronoesproGlobalSaveData GlobalSaveData { get; set; } = new PronoesproGlobalSaveData();
        public void OnLoadGlobal(PronoesproGlobalSaveData s) => GlobalSaveData = s;
        public PronoesproGlobalSaveData OnSaveGlobal() => GlobalSaveData;
        #endregion

        #region Local variables
        public PronoesproLocalSaveData LocalSaveData { get; set; } = new PronoesproLocalSaveData();

        public bool ToggleButtonInsideMenu => throw new NotImplementedException();

        public void OnLoadLocal(PronoesproLocalSaveData s) => LoadData(s);
        public PronoesproLocalSaveData OnSaveLocal() => LocalSaveData;
        #endregion

        public void InitializeData()
        {
            ModHooks.SavegameLoadHook += slot =>
              {
                  LocalSaveData = new PronoesproLocalSaveData();

                  LocalSaveData.equipedQuake = diveType;
                  LocalSaveData.equipedShriek = shriekType;
                  LocalSaveData.equipedSouls = soulType;

                  LocalSaveData.proKnightSkin = proSkinUsing;
                  LocalSaveData.proInttroductionDone = proIntroDone;
              };
        }

        public void LoadData(PronoesproLocalSaveData s)
        {
            Log("Loading data...");
            LocalSaveData = s;

            /*unlockedSouls = LocalSaveData.unlockedSouls;
            unlockedDives = LocalSaveData.unlockedDives;
            unlockedShrieks = LocalSaveData.unlockedShrieks;
            */

            dive = LocalSaveData.equipedQuake;
            shriek = LocalSaveData.equipedShriek;
            soul = LocalSaveData.equipedSouls;

            proSkinUsing = LocalSaveData.proKnightSkin;
            proIntroDone = LocalSaveData.proInttroductionDone;
        }

        #endregion

        #region UI
        public void InitializeUI()
        {
            AssetBundle ab = PronoesProMod.Instance.GOBundle["ui"];
            if (ab.Contains("UI"))
            {
                levelNameDisplay = GameObject.Instantiate(ab.LoadAsset<GameObject>("UI"));
                GameObject.DontDestroyOnLoad(levelNameDisplay);

                DialogBox box = levelNameDisplay.Find("DialogBox").gameObject.AddComponent<DialogBox>();

                box.anim = box.GetComponent<Animator>();

                Text txt = box.transform.Find("Dialog").GetComponent<Text>();
                box.dialogTxt = txt;
                txt.text = "";

                Transform nameplate = box.transform.Find("NamePlate");

                txt = nameplate.transform.Find("SuperName").GetComponent<Text>();
                box.nameSuperTxt = txt;
                txt.text = "";

                txt = nameplate.transform.Find("Name").GetComponent<Text>();
                box.nameTxt = txt;
                txt.text = "";

                txt = nameplate.transform.Find("SubName").GetComponent<Text>();
                box.nameSubTxt = nameplate.transform.Find("SubName").GetComponent<Text>();
                txt.text = "";

                box.gameObject.SetActive(false);

                Log("Initializing spell-swap UI");
                spellChangeUI = levelNameDisplay.transform.Find("SpellChange");
                proShopUI = levelNameDisplay.transform.Find("ProMenu");
                proShop = proShopUI.gameObject.AddComponent<ProShopUI>();

                Log("Initializing Pro shop icons");
                proShop.SetLockedSprite(ab.LoadAsset<Sprite>("other_ProMenu"));


                txt = spellChangeUI.Find("Soul").Find("Text").GetComponent<Text>();
                txt.text = "Soul (press " + Language.Language.Get("BUTTON_QCAST", "MainMenu") + " to change)";

                txt = spellChangeUI.Find("Dive").Find("Text").GetComponent<Text>();
                txt.text = "Dive (press " + Language.Language.Get("BUTTON_DASH", "MainMenu") + " to change)";

                txt = spellChangeUI.Find("Shriek").Find("Text").GetComponent<Text>();
                txt.text = "Shriek (press " + Language.Language.Get("BUTTON_SUPER_DASH", "MainMenu") + " to change)";
                Log("Finished initializing spell-swap UI");

                spellChangeUI.gameObject.SetActive(false);

                spellSprites = new Dictionary<string, Sprite>();

                Log("Starting to get spell swap sprites");
                for (int i = 0; i < spellSpriteNames.Length; i++)
                {
                    Sprite foundSprite = ab.LoadAsset<Sprite>(spellSpriteNames[i]);
                    if (foundSprite != null)
                    {
                        spellSprites.Add(spellSpriteNames[i], foundSprite);
                        Log("found sprite " + spellSpriteNames[i]);
                    }
                }

                proShop.SetSpellSprites(GetSpellSprites("soul"), GetSpellSprites("dive"), GetSpellSprites("shriek"));
                proShopUI.gameObject.SetActive(false);

                string _desLocName;
                locationIcons = new Dictionary<string, Sprite>();
                for (int i = 0; i < locationNames.Length; i++)
                {
                    _desLocName = locationNames[i] + "Icon";
                    if (ab.Contains(_desLocName))
                    {
                        Sprite foundSprite = ab.LoadAsset<Sprite>(_desLocName);
                        Log("Found icon " + _desLocName);
                        locationIcons.Add(_desLocName, foundSprite);
                    }else{
                        Log("Icon " + locationNames[i] + " not found");
                    }
                }
            }else{
                Log("Level Names not found");
            }

            On.SaveSlotBackgrounds.GetBackground_MapZone += GetCraftedTownBG;
            On.SaveGameData.ctor += SaveLocation;
        }

        private void SaveLocation(On.SaveGameData.orig_ctor orig, SaveGameData self, PlayerData playerData, SceneData sceneData)
        {
            if (playerData.respawnScene == "CreatedTown"){
                playerData.mapZone = (GlobalEnums.MapZone)69420;
            }
            orig(self, playerData, sceneData);
        }

        private AreaBackground GetCraftedTownBG(On.SaveSlotBackgrounds.orig_GetBackground_MapZone orig, SaveSlotBackgrounds self, GlobalEnums.MapZone mapZone)
        {
            if (mapZone == (GlobalEnums.MapZone)69420){
                AreaBackground _bg = new AreaBackground();

                _bg.areaName = (GlobalEnums.MapZone)69420;
                _bg.backgroundImage =locationIcons["CraftedTownIcon"];

                return _bg;
            }
            return orig(self, mapZone);
        }

        public Sprite[] GetSpellSprites(string _ending)
        {
            List<Sprite> _sprites=new List<Sprite>();
            foreach(string _key in spellSprites.Keys)
            {
                if (_key.EndsWith(_ending))
                {
                    _sprites.Add(spellSprites[_key]);
                }
            }
            return _sprites.ToArray();
        }

        public void ShowLevelName(int titleType)
        {
            if (levelNameDisplay != null)
            {
                Animator anim = levelNameDisplay.GetComponent<Animator>();
                if (anim != null)
                {
                    anim.SetTrigger("Show");
                    anim.SetInteger("TitleType", titleType);
                }
                else
                {
                    Instance.Log("Entrance animation not found");
                }
            }
        }

        public void ShowDialogBox(string nameSuper, string name, string NameSub, string[] dialog, string[] sounds, float dialogSpeed = 1)
        {
            if (levelNameDisplay != null)
            {
                Transform box = levelNameDisplay.transform.Find("DialogBox");
                box.gameObject.SetActive(true);

                DialogBox dialBox = box.GetComponent<DialogBox>();

                if (dialBox != null)
                {
                    dialBox.SetDialog(new string[] { name, nameSuper, NameSub }, dialog, sounds, dialogSpeed);
                    dialBox.onConversationContinue.RemoveAllListeners();
                    dialBox.onConversationEnd.RemoveAllListeners();
                    dialBox.onConversationStart.RemoveAllListeners();
                }
            }
        }

        public void ChangeDialogEvents(UnityEvent start, UnityEvent next, UnityEvent end)
        {
            if (levelNameDisplay != null) {
                Transform box = levelNameDisplay.transform.Find("DialogBox");
                DialogBox dialBox = box.GetComponent<DialogBox>();

                if (dialBox != null)
                {
                    if (start != null) {
                        dialBox.onConversationStart.AddListener(start.Invoke);
                    } else {
                        dialBox.onConversationStart.RemoveAllListeners();
                    }

                    if (next != null)
                    {
                        dialBox.onConversationContinue.AddListener(next.Invoke);
                    } else {
                        dialBox.onConversationContinue.RemoveAllListeners();
                    }

                    if (end != null)
                    {
                        dialBox.onConversationEnd.AddListener(end.Invoke);
                    } else {
                        dialBox.onConversationEnd.RemoveAllListeners();
                    }
                }
            }
        }

        public void StartInteraction(Vector2 pos, string prompt)
        {
            if (interactionPropt != null)
            {
                interactionPropt.StartInteractable(pos);
                interactionPropt.SetInteractionPrompt(prompt);
            }
        }

        bool showShopUI;
        public void ShowProShop(){
            if (!showShopUI)
            {
                showShopUI = true;
                proShopUI.gameObject.SetActive(true);
                Animator _anim = levelNameDisplay.GetComponent<Animator>();
                if (_anim != null)
                {
                    _anim.SetBool("ShowShop", true);
                }

                PlayerData.instance.SetBool("disablePause", true);
                HeroController.instance.SendMessage("RelinquishControl");
                HeroController.instance.SendMessage("StopAnimationControl");

                HeroController.instance.GetComponent<tk2dSpriteAnimator>().Play("Idle");
                HeroController.instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
        }

        public void HideProShop()
        {
            if (showShopUI)
            {
                showShopUI = false;
                Animator _anim = levelNameDisplay.GetComponent<Animator>();
                if (_anim != null)
                {
                    _anim.SetBool("ShowShop", false);
                }

                PlayerData.instance.SetBool("disablePause", false);
                HeroController.instance.SendMessage("RegainControl");
                HeroController.instance.SendMessage("StartAnimationControl");

                proShop.HideShopUI();
            }
        }

        public void EndInteraction()
        {
            if (interactionPropt != null)
            {
                interactionPropt.EndInteractable();
            }
        }

        public static bool IsMidDialog()
        {
            if (levelNameDisplay!=null)
            {
                Transform dialogTrans = levelNameDisplay.transform.Find("DialogBox");
                if (dialogTrans != null){
                    DialogBox box = dialogTrans.GetComponent<DialogBox>();
                    if (box != null)
                    {
                        return box.IsMidDialog;
                    }
                }
            }
            return false;
        }

        public void CustomSceneFadeInsis()
        {
            if (!fadedIn)
            {
                if (levelNameDisplay != null)
                {
                    Transform t = levelNameDisplay.transform.Find("OverEverything");
                    Animator anim = t.GetComponentInChildren<Animator>();
                    for (int i = 0; i < t.transform.childCount; i++)
                    {
                        if (t.transform.GetChild(i).name == "SceneFade")
                        {
                            anim = t.transform.GetChild(i).GetComponent<Animator>();
                        }
                    }
                    if (anim != null)
                    {
                        anim.SetTrigger("FadeIn");
                        fadedIn = true;
                        Instance.Log("Entrance animation found " + anim.name);
                    }
                    else
                    {
                        Instance.Log("Entrance animation not found");
                    }
                }
            }
        }

        public void CustomSceneFadeOutsis()
        {
            if (fadedIn)
            {
                if (levelNameDisplay != null)
                {
                    Animator anim = levelNameDisplay.transform.Find("OverEverything").GetComponentInChildren<Animator>();
                    for (int i = 0; i < levelNameDisplay.transform.Find("OverEverything").transform.childCount; i++)
                    {
                        if (levelNameDisplay.transform.Find("OverEverything").transform.GetChild(i).name == "SceneFade")
                        {
                            anim = levelNameDisplay.transform.Find("OverEverything").transform.GetChild(i).GetComponent<Animator>();
                        }
                    }
                    if (anim != null)
                    {
                        anim.SetTrigger("FadeOut");
                        fadedIn = false;
                        Instance.Log("Entrance animation found " + anim.name);
                    }
                    else
                    {
                        Instance.Log("Entrance animation not found");
                    }
                }
            }
        }

        public void UnloadUI()
        {
            GameObject.Destroy(levelNameDisplay);
            levelNameDisplay = null;
        }

        #endregion

        #region Language

        private string LanguageGet(string key, string sheetTitle, string orig)
        {
            string gottenValue;
            if (LanguageData.englishSentences.TryGetValue(key, out gottenValue))
            {
                return gottenValue;
            }

            if (upgradedCharms[0])
            {
                if (key == "CHARM_DESC_31" && LanguageData.englishSentences.ContainsKey("DashmasterUpgrade_Desc"))
                {
                    return LanguageData.englishSentences["DashmasterUpgrade_Desc"];
                }
                if (key == "CHARM_NAME_31" && LanguageData.englishSentences.ContainsKey("DashmasterUpgrade_Name"))
                {
                    return LanguageData.englishSentences["DashmasterUpgrade_Name"];
                }
            }

            if (upgradedCharms[1])
            {
                if (key == "CHARM_DESC_38" && LanguageData.englishSentences.ContainsKey("DeeShieldUpgrade_Desc"))
                {
                    return LanguageData.englishSentences["DeeShieldUpgrade_Desc"];
                }
                if (key == "CHARM_NAME_38" && LanguageData.englishSentences.ContainsKey("DeeShieldUpgrade_Name"))
                {
                    return LanguageData.englishSentences["DeeShieldUpgrade_Name"];
                }
            }


            if (sheetTitle == "UI")
            {
                if (key.ToLower().EndsWith("_og"))
                {
                    Language.Language.orig_Get(key.Replace("_og", ""), sheetTitle);
                }
                else
                {
                    if (key.ToLower().Contains("fireball") && soulType > 0)
                    {
                        if (key.StartsWith(soulInventoryNameKey))
                        {
                            return LanguageData.englishSentences[unlockableSouls[soulType - 1].spellNameKey];
                        }
                        else if (key.StartsWith(soulInventoryDescriptionKey))
                        {
                            return LanguageData.englishSentences[unlockableSouls[soulType - 1].spellDescriptionKey];
                        }
                    }
                    else if (key.ToLower().Contains("quake") && soulType > 0)
                    {
                        if (key.StartsWith(diveInventoryNameKey))
                        {
                            return LanguageData.englishSentences[unlockableDives[diveType - 1].spellNameKey];
                        }
                        else if (key.StartsWith(diveInventoryDescriptionKey))
                        {
                            return LanguageData.englishSentences[unlockableDives[diveType - 1].spellDescriptionKey];
                        }
                    }
                    else if (key.ToLower().Contains("scream") && soulType > 0)
                    {
                        if (key.StartsWith(shriekInventoryNameKey))
                        {
                            return LanguageData.englishSentences[unlockableShrieks[shriekType - 1].spellNameKey];
                        }
                        else if (key.StartsWith(shriekInventoryDescriptionKey))
                        {
                            return LanguageData.englishSentences[unlockableShrieks[shriekType - 1].spellDescriptionKey];
                        }
                    }
                }
            }

            return orig;

        }

        #endregion

    }

    #region Data classes
    public class PronoesproGlobalSaveData
    {

    }

    public class PronoesproLocalSaveData
    {
        public bool[] unlockedSouls, unlockedDives, unlockedShrieks;
        public int equipedSouls, equipedShriek, equipedQuake;

        public bool proKnightSkin;
        public bool proInttroductionDone;
    }
    #endregion

}