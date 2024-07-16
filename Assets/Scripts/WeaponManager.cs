using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Weapon;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance { get; set; }
    
    public List<GameObject> weaponSlots;
    public GameObject activeWeaponSlot;


    [Header("Ammo")]
    public int totalRifleAmmo = 0;
    public int totalPistolAmmo = 0;

    [Header("Throwable General")]
    public float throwForce = 10f;
    public GameObject throwableSpawn;
    public float ForceMultiplier = 0f;
    public float ForceMultiplierLimit = 2f;


    [Header("Lethals")]
    public GameObject grenadePrefab;
    public int lethalsCount = 0;
    public Throwable.ThrowableType equippedLethalType;
    public int maxLethals = 10;

    [Header("Tactical")]
    public GameObject smokeGrenadePrefab;
    public int tacticalsCount = 0;
    public Throwable.ThrowableType equippedTacticalType;
    public int maxTacticals = 10;

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

    private void Start()
    {
        activeWeaponSlot = weaponSlots[0];
        equippedLethalType = Throwable.ThrowableType.None;
        equippedTacticalType = Throwable.ThrowableType.None; 
    }

    private void Update()
    {
        foreach (GameObject weaponSlot in weaponSlots)
        {
            if (weaponSlot == activeWeaponSlot)
            {
                weaponSlot.SetActive(true);
            }
            else
            {
                weaponSlot.SetActive(false);
            }
        }

        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            SwitchActiveSlot(0);
        }
        if(Input.GetKeyUp(KeyCode.Alpha2)) 
        { 
            SwitchActiveSlot(1); 
        }

        //as long as holding the T/G key, it's going to increase the force multiplier
        if (Input.GetKey(KeyCode.G) || Input.GetKey(KeyCode.T))
        {
            ForceMultiplier += Time.deltaTime;

            if (ForceMultiplier > ForceMultiplierLimit)
            {
                ForceMultiplier = ForceMultiplierLimit;
            }
        }

        //lethals
        //by release the G key it's going to throw the lethal
        if (Input.GetKeyUp(KeyCode.G))
        {
            if(lethalsCount > 0)
            {
                ThrowLethal();
            }
            ForceMultiplier = 0f;
        }


        //Tactical
        //by release the t key it's going to throw the lethal
        if (Input.GetKeyUp(KeyCode.T))
        {
            if (tacticalsCount > 0)
            {
                ThrowTactical();
            }
            ForceMultiplier = 0f;
        }
    }


    #region || ---- Weapons --- ||

    public void PickUpWeapon(GameObject pickedUpWeapon)
    {
        AddWeaponIntoTheActiveSlots(pickedUpWeapon);
    }


    private void AddWeaponIntoTheActiveSlots(GameObject pickedUpWeapon)
    {

        DropCurrentWeapon(pickedUpWeapon);
        pickedUpWeapon.transform.SetParent(activeWeaponSlot.transform, false);

        Weapon weapon = pickedUpWeapon.GetComponent<Weapon>();

        pickedUpWeapon.transform.localPosition = new Vector3(weapon.spawnPosition.x, weapon.spawnPosition.y, weapon.spawnPosition.z);
        pickedUpWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation.x, weapon.spawnRotation.y, weapon.spawnRotation.z);

        weapon.isActiveWeapon = true;
        weapon.animator.enabled = true;
    }

    private void DropCurrentWeapon(GameObject pickedUpWeapon)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            var weaponToDrop = activeWeaponSlot.transform.GetChild(0).gameObject;

            weaponToDrop.transform.GetComponent<Weapon>().isActiveWeapon = false;
            weaponToDrop.transform.GetComponent<Animator>().enabled = false;

            weaponToDrop.transform.SetParent(pickedUpWeapon.transform.parent);

            //switch between weapon in hand to weapon on the floor/table/somewhere else
            weaponToDrop.transform.localPosition = pickedUpWeapon.transform.localPosition;
            weaponToDrop.transform.localRotation = pickedUpWeapon.transform.localRotation;
        }
    }

    public void SwitchActiveSlot(int slotNumber)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon currentWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            currentWeapon.isActiveWeapon = false;
        }

        activeWeaponSlot = weaponSlots[slotNumber];

        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon newWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            newWeapon.isActiveWeapon = true;
        }
    }

    #endregion

    #region || ---- Ammo --- ||
    public void PickUpAmmo(AmmoBox pickedUpAmmo)
    {
        switch (pickedUpAmmo.ammoType)
        {
            case AmmoBox.AmmoType.RifleAmmo:
                totalRifleAmmo += pickedUpAmmo.ammoAmount;
                break;
            case AmmoBox.AmmoType.PistolAmmo:
                totalPistolAmmo += pickedUpAmmo.ammoAmount;
                break;

            default:
                return;
        }
    }

    internal void DecreaseWeaponAmount(int bulletsToDecrease, Weapon.WeaponModel model)
    {
        switch (model)
        {
            case Weapon.WeaponModel.Pistol1911:
                totalPistolAmmo -= bulletsToDecrease;
                break;
            case Weapon.WeaponModel.Uzi:
                totalRifleAmmo -= bulletsToDecrease;
                break;

            default:
                return;
        }
    }

    internal int CheckAmmoLeftFor(Weapon.WeaponModel thisWeapon)
    {
        switch (thisWeapon)
        {
            case Weapon.WeaponModel.Pistol1911:
                return totalPistolAmmo;
            case Weapon.WeaponModel.Uzi:
                return totalRifleAmmo;

            default:
                return 0;
        }
    }
    #endregion


    #region || ---- Throwables --- ||

    public void PickUpThrowable(Throwable pickedUpThrowable)
    {
        switch (pickedUpThrowable.throwableType)
        {
            case Throwable.ThrowableType.Grenade:
                PickUpThrowableAsLethal(Throwable.ThrowableType.Grenade);
                break;
            case Throwable.ThrowableType.Smoke_Grenade:
                PickUpThrowableAsTactical(Throwable.ThrowableType.Smoke_Grenade);
                break;

            default:
                return;
        }
    }

    private void PickUpThrowableAsTactical(Throwable.ThrowableType tactical)
    {
        if (equippedTacticalType == tactical || equippedTacticalType == Throwable.ThrowableType.None)
        {
            equippedTacticalType = tactical;

            if (tacticalsCount < maxTacticals)
            {
                tacticalsCount += 1;
                Destroy(InteractionManager.instance.hoveredThrowable.gameObject);
                HUDManager.instance.UpdateThrowablesUI();
            }
            else
            {
                print("Tacticals limit reached");
            }
        }
        else
        {

        }
    }

    private void PickUpThrowableAsLethal(Throwable.ThrowableType lethal)
    {
        if (equippedLethalType == lethal || equippedLethalType == Throwable.ThrowableType.None)
        {
            equippedLethalType = lethal;

            if (lethalsCount < maxLethals)
            {
                lethalsCount += 1;
                Destroy(InteractionManager.instance.hoveredThrowable.gameObject);
                HUDManager.instance.UpdateThrowablesUI();
            }
            else
            {
                print("Lethals limit reached");
            }
        }
        else
        {

        }
    }

    private void ThrowTactical()
    {
        GameObject tacticalPrefab = GetThrowablePrefab(equippedTacticalType);

        GameObject throwable = Instantiate(tacticalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        Rigidbody rigidbody = throwable.GetComponent<Rigidbody>();

        rigidbody.AddForce(Camera.main.transform.forward * (throwForce * ForceMultiplier), ForceMode.Impulse);

        throwable.GetComponent<Throwable>().hasBeenThrown = true;
        tacticalsCount -= 1;


        if (tacticalsCount <= 0)
        {
            equippedTacticalType = Throwable.ThrowableType.None;
        }
        HUDManager.instance.UpdateThrowablesUI();
    }

    private void ThrowLethal()
    {
        GameObject lethalPrefab = GetThrowablePrefab(equippedLethalType);
        
        GameObject throwable = Instantiate(lethalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        Rigidbody rigidbody = throwable.GetComponent<Rigidbody>();

        rigidbody.AddForce(Camera.main.transform.forward * (throwForce * ForceMultiplier) , ForceMode.Impulse);

        throwable.GetComponent<Throwable>().hasBeenThrown = true;
        lethalsCount -= 1;

        if(lethalsCount <= 0)
        {
            equippedLethalType = Throwable.ThrowableType.None;
        }
        HUDManager.instance.UpdateThrowablesUI();
    }

    private GameObject GetThrowablePrefab(Throwable.ThrowableType equippedType)
    {
        switch (equippedType)
        {
            case Throwable.ThrowableType.Grenade:
                return grenadePrefab;
            case Throwable.ThrowableType.Smoke_Grenade:
                return smokeGrenadePrefab;

            default:
                return new();
        }
            
    }

    #endregion

}
