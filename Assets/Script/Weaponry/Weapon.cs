using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    //Weapon stats
    public float damage;
    public float attackSpeed;
    public float durability;
    public float maxDurability;


    Animator characterAnimator;
    playerController controllerScript;
    //playerCrouchController crouchScript;



    //To show weapon if hidden on mouse click
    playerStats statScript;



    inventoryManagement invScript;

    void Start()
    {
        characterAnimator = GameObject.Find("Julia").GetComponent<Animator>();
        controllerScript = GameObject.Find("Julia").GetComponent<playerController>();
        statScript = GameObject.Find("Julia").GetComponent<playerStats>();

        //Handle hide weapon system
        invScript = GameObject.Find("Julia").GetComponent<inventoryManagement>();



    }



    public void comboSystem()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !controllerScript.attacking && controllerScript.rollAvailable)
        {

            if (invScript.swordToBack)
            {
                invScript.equippedWeaponsToFront();
            }

            //Player is busy
            if (controllerScript.combo == 0 && statScript.getStamina() > 10f)
            {
                statScript.deductStamina(10f);
                controllerScript.actionsBusy = true;
                controllerScript.attacking = true;
                characterAnimator.SetTrigger("" + controllerScript.combo);
            }
            //Player is busy
            else if (controllerScript.combo == 1 && statScript.getStamina() > 8f)
            {
                statScript.deductStamina(8f);
                controllerScript.actionsBusy = true;
                controllerScript.attacking = true;
                characterAnimator.SetTrigger("" + controllerScript.combo);
            }
            //Player is busy
            else if (controllerScript.combo == 2 && statScript.getStamina() > 7f)
            {
                statScript.deductStamina(7f);
                controllerScript.actionsBusy = true;
                controllerScript.attacking = true;
                characterAnimator.SetTrigger("" + controllerScript.combo);
            }

            StartCoroutine(restoreReset(1f));


        }
    }
     




    void Update()
    {
        if((!controllerScript.inventoryOpen && characterAnimator.GetBool("hasWeapon")) || (!controllerScript.inventoryOpen && invScript.swordToBack))
            comboSystem();

    }



    IEnumerator restoreReset(float time)
    {
        yield return new WaitForSeconds(time);

        invScript.stopCoroutines();
        StartCoroutine(invScript.equippedWeaponsToBack(20f));

    }



}
