using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{


    //FIX THE PICKUP SYSTEM BECAUSE IF YOU FIND MORE THAN ONE TARGET AT RANGE IT PICKS THEM ALL AND FIND A ROLLING ANIMATION


    public float damageBlock;
    public float durability;
    public float maxDurability;



    Animator characterAnimator;
    playerController controllerScript;
    playerStats statScript;

    inventoryManagement invScript;

    void Start()
    {
        characterAnimator = GameObject.Find("Julia").GetComponent<Animator>();
        controllerScript = GameObject.Find("Julia").GetComponent<playerController>();
        statScript = GameObject.Find("Julia").GetComponent<playerStats>();
        //Handle hide shield system
        invScript = GameObject.Find("Julia").GetComponent<inventoryManagement>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!controllerScript.actionsBusy && !controllerScript.inventoryOpen)// || (controllerScript.actionsBusy && crouchScript.isCrouching))
        {
            if ((Input.GetKeyDown(KeyCode.Mouse1) && controllerScript.Grounded && characterAnimator.GetBool("hasShield")) ||
                (Input.GetKeyDown(KeyCode.Mouse1) && controllerScript.Grounded && invScript.shieldToBack))
                {
                //Check for inPlayerBack bool
                if (invScript.shieldToBack)
                {
                    invScript.equippedWeaponsToFront();
                } 
                 

                characterAnimator.SetBool("block", true);
                //Disables movement and health regen
                controllerScript.actionsBusy = true;
                statScript.regenHea = false;
            }
            
        }

        //Stopped using shield
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {

            //Reset hide weapon timeout routine
           // pickupShieldScript.resetRoutine = true;
            StartCoroutine(restoreReset(1f));

            characterAnimator.SetBool("block", false);
            //Enables movement
            controllerScript.actionsBusy = false;
            statScript.regenHea = true;

        }



    } 

    IEnumerator restoreReset(float time)
    {
        yield return new WaitForSeconds(time);
         
        invScript.stopCoroutines();
        StartCoroutine(invScript.equippedWeaponsToBack(20f));

    }

}
