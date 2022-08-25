using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerCrouchController : MonoBehaviour
{

    Animator characterAnimator;
    playerController controllerScript;
    CharacterController charController;

    [HideInInspector] public bool isCrouching = false;
    bool crouchAvailable = true;
    void Start()
    {
        characterAnimator = GetComponent<Animator>();
        controllerScript = GetComponent<playerController>();
        charController = GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {


        //GRAB ALL ANIMATIONS AND GET THEM AS "INPLACE" FROM MIXAMO, THEN APPLY ROOTMOTION TO THE ANIMATOR COMPONENT IN JULIA
        
        //Prevents spamming C
        if(crouchAvailable)
        {
                    //If it's crouching and is not busy (movement available)
                if (!isCrouching && !controllerScript.actionsBusy)
                {
                    if (Input.GetKeyDown(KeyCode.C))
                    {

                    //Resize player collider
                    charController.height = 1.4f;
                    charController.center = new Vector3(0f, 0.75f, 0f);


                    //lower speed
                    controllerScript.MoveSpeed = 1.5f;
                        controllerScript.SprintSpeed = 3f;

                        //Prevents spamming C
                        crouchAvailable = false;
                        characterAnimator.SetBool("crouching", true);
                        //controllerScript.actionsBusy = true;
                        isCrouching = true;
                        controllerScript.isCrouching = true;
                        StartCoroutine(timer2(0.4f));
                }
                }
                else //CHECK THIS BECAUSE YOU CAN STAND UP WHEN BLOCKING AND CROUCHING or JUST BLOCKING WHICH ENABLES MOVEMENT
                {
                    if (Input.GetKeyDown(KeyCode.C) && !controllerScript.actionsBusy)
                    {
                    
                        //Resize player collider
                        charController.height = 1.8f;
                        charController.center = new Vector3(0f, 0.93f, 0f);
                        //Restore speed
                        controllerScript.MoveSpeed = 2.0f;
                        controllerScript.SprintSpeed = 5.335f;

                        characterAnimator.SetBool("crouching", false);
                        StartCoroutine(timer(0.2f));

                    }
                }
        }

       


    }


    IEnumerator timer(float time)
    {
        yield return new WaitForSeconds(time);
        //controllerScript.actionsBusy = false;
        isCrouching = false;
        controllerScript.isCrouching = false;
    }

    IEnumerator timer2(float time)
    {
        yield return new WaitForSeconds(time);
        //controllerScript.actionsBusy = false;
        crouchAvailable = true;
    }




}
