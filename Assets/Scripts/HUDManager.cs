using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class HUDManager :MonoBehaviour
{

    public static HUDManager instance { get; set; }


    [Header("Ammo")]
    public TextMeshProUGUI magazineAmmoUI;
    public TextMeshProUGUI totalAmmoUI;
    public Image AmmoTypUI;

    [Header("Weapon")]
    public Image activeWeaponUI;
    public Image unActiveWeaponUI;

    [Header("Throwbles")]
    public Image lethalUI;
    public TextMeshProUGUI lethalAmmountUI;

    public Image tacticalUI;
    public TextMeshProUGUI tacticalAmmountUI;

    public GameObject crossHair;

    public Sprite emptySlot;
    public Sprite greySlot;
    private void Awake()
    {
        if (instance != null && instance != this)
        {

            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }


    
    private void Update()
    {
        Weapon activeWeapon = WeaponManager.instance.activeWeaponSlot.GetComponentInChildren<Weapon>();
        Weapon unActiveWeapon = GetUnActiveWeaponSlot().GetComponentInChildren<Weapon>();

        if (activeWeapon)
        {
            magazineAmmoUI.text = $"{activeWeapon.bulletsLeft / activeWeapon.bulletsPerBurst}";
            totalAmmoUI.text = $"{WeaponManager.instance.CheckAmmoLeftFor(activeWeapon.thisWeapon)}";

            Weapon.WeaponModel model = activeWeapon.thisWeapon;
            AmmoTypUI.sprite = GetAmmoSprite(model);
           
            activeWeaponUI.sprite = GetWeaponSprite(model);

            if (unActiveWeapon)
            {
                unActiveWeaponUI.sprite = GetWeaponSprite(unActiveWeapon.thisWeapon);
            }
        }
        else
        {
            magazineAmmoUI.text = "";
            totalAmmoUI.text = "";

            AmmoTypUI.sprite = emptySlot;
            
            activeWeaponUI.sprite = emptySlot;
            unActiveWeaponUI.sprite= emptySlot;
        }

        if(WeaponManager.instance.lethalsCount <= 0)
        {
            lethalUI.sprite = greySlot;
        }

        if (WeaponManager.instance.tacticalsCount <= 0)
        {
            tacticalUI.sprite = greySlot;
        }

    }

    private Sprite GetWeaponSprite(Weapon.WeaponModel model)
    {
        switch(model)
        {
            case Weapon.WeaponModel.Pistol1911:
                return Resources.Load<GameObject>("PistolM1911_Weapon").GetComponent<SpriteRenderer>().sprite;
            case Weapon.WeaponModel.Uzi:
                return Resources.Load<GameObject>("Uzi_Weapon").GetComponent<SpriteRenderer>().sprite;
            
            default:
                return null;
        }
    }

    private GameObject GetUnActiveWeaponSlot()
    {
        foreach (GameObject weaponSlots in WeaponManager.instance.weaponSlots)
        {
            if(weaponSlots != WeaponManager.instance.activeWeaponSlot)
            {
                return weaponSlots;
            }
        }


        return null;
    }

    private Sprite GetAmmoSprite(Weapon.WeaponModel model)
    {
        switch (model)
        {
            case Weapon.WeaponModel.Pistol1911:
                return Resources.Load<GameObject>("Pistol_Ammo").GetComponent<SpriteRenderer>().sprite;
            case Weapon.WeaponModel.Uzi:
                return Resources.Load<GameObject>("Rifle_Ammo").GetComponent<SpriteRenderer>().sprite;
            
            default:
                return null;
        }
    }


    internal void UpdateThrowablesUI()
    {
        lethalAmmountUI.text = $"{WeaponManager.instance.lethalsCount}";
        tacticalAmmountUI.text = $"{WeaponManager.instance.tacticalsCount}";

        //Lethals
        switch (WeaponManager.instance.equippedLethalType)
        {
            case Throwable.ThrowableType.Grenade:
                lethalUI.sprite = Resources.Load<GameObject>("Grenade").GetComponent<SpriteRenderer>().sprite;
                break;

            default:
                return;
        }

        //Tactical
        switch (WeaponManager.instance.equippedTacticalType)
        {
            case Throwable.ThrowableType.Smoke_Grenade:
                tacticalUI.sprite = Resources.Load<GameObject>("Smoke_Grenade").GetComponent<SpriteRenderer>().sprite;
                break;

            default:
                return;
        }
    }
}
