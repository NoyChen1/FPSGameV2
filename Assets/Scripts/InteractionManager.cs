using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance { get; set; }

    public Weapon hoveredWEapon = null;
    public AmmoBox hoveredAmmoBox = null;
    public Throwable hoveredThrowable = null;

    [SerializeField] private float maxDistance = 5f;


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

        //if some object is not "hitted" by ray, add a colider
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject objectHittedByRayCast = hit.transform.gameObject;
            print(objectHittedByRayCast.gameObject.name);
           
            //Weapon
            if (objectHittedByRayCast.GetComponent<Weapon>() && // if we hit a weapon
                !objectHittedByRayCast.GetComponent<Weapon>().isActiveWeapon && //and it's not an active weapon
                Vector3.Distance(Camera.main.transform.position, objectHittedByRayCast.transform.position) <= maxDistance) // close to the player
            {

                //disable the outline from an unselected weapon
                if (hoveredWEapon)
                {
                    hoveredWEapon.GetComponent<Outline>().enabled = false;
                }

                hoveredWEapon = objectHittedByRayCast.gameObject.GetComponent<Weapon>();
                hoveredWEapon.GetComponent<Outline>().enabled = true;
                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaponManager.instance.PickUpWeapon(objectHittedByRayCast.gameObject);
                }
            }
            else
            {
                if (hoveredWEapon)
                {
                    hoveredWEapon.GetComponent<Outline>().enabled = false;
                }
            }


            //Ammo
            if (objectHittedByRayCast.GetComponent<AmmoBox>() && //if we hit an AmmoBox
                Vector3.Distance(Camera.main.transform.position, objectHittedByRayCast.transform.position) <= maxDistance) // close to the player             
            {
                //disable the outline from an unselected AmmoBox
                if (hoveredAmmoBox)
                {
                    hoveredAmmoBox.GetComponent<Outline>().enabled = false;
                }

                hoveredAmmoBox = objectHittedByRayCast.gameObject.GetComponent<AmmoBox>();
                hoveredAmmoBox.GetComponent<Outline>().enabled = true;
               
                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaponManager.instance.PickUpAmmo(hoveredAmmoBox);
                    Destroy(objectHittedByRayCast.gameObject);
                }
            }
            else
            {
                if (hoveredAmmoBox)
                {
                    hoveredAmmoBox.GetComponent<Outline>().enabled = false;
                }
            }


            //Throwable
            if (objectHittedByRayCast.GetComponent<Throwable>() && //if we hit an AmmoBox
                Vector3.Distance(Camera.main.transform.position, objectHittedByRayCast.transform.position) <= maxDistance)// close to the player
            {

                //disable the outline from an unselected Throwable
                if (hoveredThrowable)
                {
                    hoveredThrowable.GetComponent<Outline>().enabled = false;
                }

                hoveredThrowable = objectHittedByRayCast.gameObject.GetComponent<Throwable>();
                hoveredThrowable.GetComponent<Outline>().enabled = true;

                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaponManager.instance.PickUpThrowable(hoveredThrowable);
                }
            }
            else
            {
                if (hoveredThrowable)
                {
                    hoveredThrowable.GetComponent<Outline>().enabled = false;
                }
            }
        }
    }
}
