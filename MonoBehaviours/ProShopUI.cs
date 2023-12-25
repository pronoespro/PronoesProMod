using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace PronoesProMod.MonoBehaviours
{
    [System.Serializable]
    public class SpellShopData
    {
        public string name;
        public string description;

        public Sprite icon;

        public SpellShopData(string _name,string _desc,Sprite _icon=null)
        {
            name = _name;
            description = _desc;
            icon = _icon;
        }
    }

    public class ProShopUI:MonoBehaviour
    {

        private Transform selectionPointParent;

        private Transform soulSelectionPoints, diveSelectionPoints, shriekSelectionPoints;
        private Transform generalSelection, soulSelection, diveSelection, shriekSelection;
        
        private Image spellIcon;
        private Text nameText,descriptionText;
        private Transform lockedImage;
        
        private int curSoulSpell, curDiveSpell, curShriekSpell;
        private int soulLevel, diveLevel, shriekLevel;

        private int selectedItemX, selectedItemY;
        private bool changedSelection=true;

        private SpellShopData[] soulSpellData = new SpellShopData[]{
            new SpellShopData("Soul_sawblade_*", "Soul_sawblade_*_desc"),
            new SpellShopData("Soul_nailmaster_*", "Soul_nailmaster_*_desc"),
            new SpellShopData("Soul_apple_*", "Soul_apple_*_desc"),
            new SpellShopData("Soul_Dee_*", "Soul_Dee_*_desc"),
        };
        private SpellShopData[] diveSpellData = new SpellShopData[]{
            new SpellShopData("Dive_sawblade_*", "Dive_sawblade_*_desc"),
            new SpellShopData("Dive_nailmaster_*", "Dive_nailmaster_*_desc"),
            new SpellShopData("Dive_apple_*", "Dive_apple_*_desc")
        };
        private SpellShopData[] shriekSpellData = new SpellShopData[]{
            new SpellShopData("Shriek_sawblade_*", "Shriek_sawblade_*_desc"),
            new SpellShopData("Shriek_nailmaster_*", "Shriek_nailmaster_*_desc"),
            new SpellShopData("Shriek_apple_*", "Shriek_apple_*_desc")
        };

        private Sprite lockedSpellSprite;
        private Sprite[] soulSprites, diveSprites, shriekSprites;
        private Transform soulImages, diveImages, shriekImages;

        public void SetLockedSprite(Sprite _lock)
        {
            lockedSpellSprite = _lock;
        }

        public void SetSpellSprites(Sprite[] _soul,Sprite[] _dive,Sprite[] _shriek)
        {
            soulSprites = _soul;
            diveSprites = _dive;
            shriekSprites = _shriek;
        }

        private void Start()
        {
            selectionPointParent = transform.Find("SelectionPoints");

            if (selectionPointParent != null)
            {
                soulSelectionPoints = selectionPointParent.Find("Souls");
                diveSelectionPoints = selectionPointParent.Find("Dives");
                shriekSelectionPoints= selectionPointParent.Find("Shrieks");
            }

            Transform _selecctions = transform.Find("Selections");
            if (_selecctions != null)
            {
                generalSelection = _selecctions.GetChild(0);
                soulSelection = _selecctions.Find("Spell selection (soul)");
                diveSelection = _selecctions.Find("Spell selection (dive)");
                shriekSelection = _selecctions.Find("Spell selection (shriek)");
            }

            curSoulSpell = PronoesProMod.Instance.GetCurEquipedSpel("Soul");
            curDiveSpell= PronoesProMod.Instance.GetCurEquipedSpel("Dive");
            curShriekSpell = PronoesProMod.Instance.GetCurEquipedSpel("Shriek");

            Transform _itemPanel = transform.Find("Items");
            if (_itemPanel != null)
            {
                Transform _inventory = _itemPanel.GetChild(0);

                if (_inventory != null)
                {
                    {
                        soulImages = _inventory.GetChild(0);

                        for (int i = 1; i < soulImages.childCount; i++)
                        {
                            SetSpellSprite(soulImages.GetChild(i), 0, i);
                        }

                    }

                    {
                        diveImages = _inventory.GetChild(1);
                        for (int i = 1; i < diveImages.childCount; i++)
                        {
                            SetSpellSprite(diveImages.GetChild(i), 1, i);
                        }
                    }

                    {
                        shriekImages = _inventory.GetChild(2);
                        for (int i = 1; i < shriekImages.childCount; i++)
                        {
                            SetSpellSprite(shriekImages.GetChild(i), 2, i);
                        }
                    }
                }

                Transform _data = _itemPanel.GetChild(1);
                if (_data != null)
                {

                    spellIcon = _data.GetChild(1).GetComponent<Image>();
                    nameText = _data.GetChild(2).GetComponent<Text>();
                    descriptionText = _data.GetChild(3).GetComponent<Text>();
                    lockedImage = _data.GetChild(4);

                    PronoesProMod.Instance.Log("");
                    PronoesProMod.Instance.Log("---=== 0 ===---");
                    PronoesProMod.Instance.Log("Done preparing UI!");
                    PronoesProMod.Instance.Log(lockedImage.name);
                    PronoesProMod.Instance.Log("---=== 0 ===---");
                    PronoesProMod.Instance.Log("");
                } 
            }
            else
            {
                PronoesProMod.Instance.Log("Not found item panel");
            }
        }

        private void ResetSpellSprites()
        {
            for (int i = 1; i < soulImages.childCount; i++)
            {
                SetSpellSprite(soulImages.GetChild(i), 0, i);
            }

            for (int i = 1; i < diveImages.childCount; i++)
            {
                SetSpellSprite(diveImages.GetChild(i), 1, i);
            }

            for (int i = 1; i < shriekImages.childCount; i++)
            {
                SetSpellSprite(shriekImages.GetChild(i), 2, i);
            }
        }

        private void SetSpellSprite(Transform _sprite,int _spellType,int _spellNum)
        {
            Image _spr = _sprite.GetChild(0).GetChild(0).GetComponent<Image>();
            if (_spr != null)
            {
                switch (_spellType)
                {
                    default:
                        if (soulLevel > 0){
                            _sprite.GetChild(0).gameObject.SetActive(false);
                        }else
                        {
                            _sprite.GetChild(0).gameObject.SetActive(true);
                            _spr.sprite =  soulSprites[_spellNum];
                        }
                        break;
                    case 1: 
                        if (diveLevel > 0){
                            _sprite.GetChild(0).gameObject.SetActive(false);
                        }else{
                            _sprite.GetChild(0).gameObject.SetActive(true);
                            _spr.sprite = diveSprites[_spellNum];
                        }
                        break;
                    case 2:
                        if (shriekLevel > 0){
                            _sprite.GetChild(0).gameObject.SetActive(false);
                        }else{
                            _sprite.GetChild(0).gameObject.SetActive(true);
                            _spr.sprite = shriekSprites[_spellNum];
                        }
                        break;
                }
            }
        }

        private void Update()
        {
            if (PlayerData.instance.quakeLevel > 0)
            {
                diveLevel = (PlayerData.instance.shadeQuakeLevel > 0) ? 2 : 1;
            }
            if (PlayerData.instance.screamLevel > 0)
            {
                shriekLevel = (PlayerData.instance.shadeScreamLevel > 0) ? 2 : 1;
            }
            if (PlayerData.instance.fireballLevel > 0)
            {
                soulLevel = (PlayerData.instance.shadeFireballLevel > 0) ? 2 : 1;
            }


            if (InputHandler.Instance.inputActions.down.WasPressed || InputHandler.Instance.inputActions.up.WasPressed)
            {
                selectedItemY = Mathf.Clamp(selectedItemY + (int)Mathf.Sign(GetMenuMovement().y), 0, 3);
                selectedItemX = Mathf.Clamp(selectedItemX, 0, GetCurrentSpellType().childCount-1);

                changedSelection = true;
                PronoesProMod.Instance.Log("Moved menu on Y");
            }
            if (InputHandler.Instance.inputActions.left.WasPressed || InputHandler.Instance.inputActions.right.WasPressed)
            {
                selectedItemX = Mathf.Clamp(selectedItemX + (int)Mathf.Sign(GetMenuMovement().x), 0, GetCurrentSpellType().childCount-1);
                changedSelection = true;
                PronoesProMod.Instance.Log("Moved menu on X");
            }

            CheckSelectorPosition();

            if (InputHandler.Instance.inputActions.menuSubmit)
            {
                SetSpells();
                PronoesProMod.Instance.Log("Changed equipped spells");
            }
            if (InputHandler.Instance.inputActions.pause || InputHandler.Instance.inputActions.menuCancel){
                PronoesProMod.Instance.HideProShop();
                PronoesProMod.Instance.Log("Finished window-shopping XD");
            }

            if (changedSelection)
            {
                ResetSpellSprites();
                ReloadTexts();

                changedSelection = false;
            }
        }

        private Vector2 GetMenuMovement()
        {
            Vector2 _dir = Vector2.zero;

            if (InputHandler.Instance.inputActions.up.WasPressed){
                _dir.y = -1;
            }else if(InputHandler.Instance.inputActions.down.WasPressed){
                _dir.y = 1;
            }

            if (InputHandler.Instance.inputActions.right.WasPressed)
            {
                _dir.x = 1;
            }else if (InputHandler.Instance.inputActions.left.WasPressed){
                _dir.x = -1;
            }

            return _dir;
        }

        private void CheckSelectorPosition()
        {
            switch (selectedItemY)
            {
                default:
                case 0:
                    generalSelection.transform.position = soulSelectionPoints.GetChild(Mathf.Min(selectedItemX,soulSelectionPoints.childCount)).position;
                    break;
                case 1:
                    generalSelection.transform.position = diveSelectionPoints.GetChild(Mathf.Min(selectedItemX, diveSelectionPoints.childCount)).position;
                    break;
                case 2:
                    generalSelection.transform.position = shriekSelectionPoints.GetChild(Mathf.Min(selectedItemX, shriekSelectionPoints.childCount)).position;
                    break;
            }

            soulSelection.position = soulSelectionPoints.GetChild(Mathf.Min(curSoulSpell, soulSelectionPoints.childCount)).position;
            diveSelection.position = diveSelectionPoints.GetChild(Mathf.Min(curDiveSpell, diveSelectionPoints.childCount)).position;
            shriekSelection.position = shriekSelectionPoints.GetChild(Mathf.Min(curShriekSpell, shriekSelectionPoints.childCount)).position;
        }

        private void ReloadTexts()
        {
            switch (selectedItemY)
            {
                default:
                case 0:
                    spellIcon.sprite= soulSpellData[selectedItemX].icon;

                    if (selectedItemX == 0){
                        nameText.text=Language.Language.Get("INV_NAME_SPELL_FIREBALL" + soulLevel.ToString());
                        descriptionText.text=Language.Language.Get("INV_DESC_SPELL_FIREBALL" + soulLevel.ToString());
                    }else{
                        nameText.text = LanguageData.englishSentences[soulSpellData[selectedItemX].name.Replace("*",(soulLevel-1).ToString())];
                        descriptionText.text = LanguageData.englishSentences[soulSpellData[selectedItemX].description.Replace("*", (soulLevel-1).ToString())];
                    }

                    if (soulLevel > 0){
                        if (selectedItemX > 0)
                        {
                            lockedImage.gameObject.SetActive(PronoesProMod.Instance.unlockableSouls[selectedItemX - 1].unlocked);
                        }else{
                            lockedImage.gameObject.SetActive(true);
                        }
                    }else
                    {
                        lockedImage.gameObject.SetActive(true);
                    }
                    break;
                case 1:
                    spellIcon.sprite = diveSpellData[selectedItemX].icon;

                    if (selectedItemX == 0)
                    {
                        nameText.text = Language.Language.Get("INV_NAME_SPELL_QUAKE" + diveLevel.ToString());
                        descriptionText.text = Language.Language.Get("INV_DESC_SPELL_QUAKE" + diveLevel.ToString());
                    }
                    else
                    {
                        nameText.text = LanguageData.englishSentences[diveSpellData[selectedItemX].name.Replace("*", (diveLevel-1).ToString())];
                        descriptionText.text = LanguageData.englishSentences[diveSpellData[selectedItemX].description.Replace("*", (diveLevel-1).ToString())];
                    }

                    if (diveLevel > 0){
                        if (selectedItemX > 0){
                            lockedImage.gameObject.SetActive(PronoesProMod.Instance.unlockableDives[selectedItemX - 1].unlocked);
                        }else{
                            lockedImage.gameObject.SetActive(true);
                        }
                    }else{
                        lockedImage.gameObject.SetActive(true);
                    }
                    break;
                case 2:
                    spellIcon.sprite = shriekSpellData[selectedItemX].icon;

                    if (selectedItemX == 0)
                    {
                        nameText.text = Language.Language.Get("INV_NAME_SPELL_SCREAM" + diveLevel.ToString());
                        descriptionText.text = Language.Language.Get("INV_DESC_SPELL_SCREAM" + diveLevel.ToString());
                    }
                    else
                    {
                        nameText.text = LanguageData.englishSentences[shriekSpellData[selectedItemX].name.Replace("*", (shriekLevel-1).ToString())];
                        descriptionText.text = LanguageData.englishSentences[shriekSpellData[selectedItemX].description.Replace("*", (shriekLevel-1).ToString())];
                    }

                    if (shriekLevel > 0)
                    {
                        if (selectedItemX > 0)
                        {
                            lockedImage.gameObject.SetActive(PronoesProMod.Instance.unlockableShrieks[selectedItemX - 1].unlocked);
                        }
                        else
                        {
                            lockedImage.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        lockedImage.gameObject.SetActive(true);
                    }
                    break;
            }
        }

        private Transform GetCurrentSpellType()
        {
            switch (selectedItemY)
            {
                default:
                case 0:
                    return soulSelectionPoints;
                case 1:
                    return diveSelectionPoints;
                case 2:
                    return shriekSelectionPoints;
            }
        }

        private void SetSpells()
        {
            switch (selectedItemY)
            {
                default:
                case 0:
                    if (selectedItemX==0 || PronoesProMod.Instance.unlockableSouls[selectedItemX-1].unlocked ||TryUnlockSpell()){
                        curSoulSpell = Mathf.Clamp(selectedItemX, 0, soulSelectionPoints.childCount - 1);
                    }
                    break;
                case 1:
                    if (selectedItemX == 0 || PronoesProMod.Instance.unlockableDives[selectedItemX-1].unlocked || TryUnlockSpell()){
                        curDiveSpell = Mathf.Clamp(selectedItemX, 0, diveSelectionPoints.childCount - 1);
                    }
                    break;
                case 2:
                    if (selectedItemX == 0 || PronoesProMod.Instance.unlockableShrieks[selectedItemX-1].unlocked ||TryUnlockSpell()){
                        curShriekSpell = Mathf.Clamp(selectedItemX, 0, shriekSelectionPoints.childCount - 1);
                    }
                    break;
            }

            PronoesProMod.Instance.equipedSoulType = curSoulSpell;
            PronoesProMod.Instance.equipedDiveType= curDiveSpell;
            PronoesProMod.Instance.equipedShriekType= curShriekSpell;
        }
        private bool TryUnlockSpell()
        {
            int _essence = PlayerData.instance.GetInt(nameof(PlayerData.instance.dreamOrbs));
            switch (selectedItemY)
            {
                default:
                case 0:
                    if(PronoesProMod.Instance.unlockableSouls[selectedItemX - 1].unlocked)
                    {
                        return true;
                    }else if (_essence>=PronoesProMod.Instance.unlockableSouls[selectedItemX-1].unlockCost)
                    {
                        PronoesProMod.Instance.unlockableSouls[selectedItemX-1].unlocked = true;
                        PlayerData.instance.dreamOrbs -= PronoesProMod.Instance.unlockableSouls[selectedItemX-1].unlockCost;
                        return true;
                    }
                    break;
                case 1:
                    if (PronoesProMod.Instance.unlockableDives[selectedItemX - 1].unlocked)
                    {
                        return true;
                    }else if (_essence>=PronoesProMod.Instance.unlockableDives[selectedItemX-1].unlockCost )
                    {
                        PronoesProMod.Instance.unlockableDives[selectedItemX-1].unlocked = true;
                        PlayerData.instance.dreamOrbs -= PronoesProMod.Instance.unlockableDives[selectedItemX-1].unlockCost;
                        return true;
                    }
                    break;
                case 2:
                    if (PronoesProMod.Instance.unlockableShrieks[selectedItemX - 1].unlocked)
                    {
                        return true;
                    }else if (_essence>=PronoesProMod.Instance.unlockableShrieks[selectedItemX-1].unlockCost )
                    {
                        PronoesProMod.Instance.unlockableShrieks[selectedItemX-1].unlocked = true;
                        PlayerData.instance.dreamOrbs -= PronoesProMod.Instance.unlockableShrieks[selectedItemX-1].unlockCost;
                        return true;
                    }
                    break;
            }
            return false;
        }

        public void HideShopUI()
        {
            StopAllCoroutines();
            StartCoroutine(HidingShopUI());
        }

        private IEnumerator HidingShopUI()
        {
            float _timer = 0;

            while (_timer < 0.5f)
            {
                _timer += Time.unscaledDeltaTime;
                yield return null;
            }

            gameObject.SetActive(false);
        }

    }
}
