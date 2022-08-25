using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class playerStats : MonoBehaviour
{


    float maxStamina = 100;
    float maxHealth = 100;
    float currentStamina = 100;
    float currentHealth = 100;

    public int currentCoins = 0;

    public float healthRegenRate;
    public float staminaRegenRate;

    referenceCarrier reference;
    playerController controllerScript;
    Animator charAnimator;

    
    
   // inventoryManagement invScript;
    public TextMeshProUGUI playerStatsL, playerStatsR, currentCoinsText;


    //Time
    /*RUNS ONCE EVERY SECOND
    private float time = 0f;
    private float interpolationPeriod = 1f;
    */

    //STAMINA 30 times a second, HEALTH 30 times a second
    private float regenTime = 0f;
    private float regenInterpolationTime = 0.0333f;
    //private float currentTime = 0f;
   // private float InterpolationPeriod = 1f;

    [HideInInspector] public bool regenSta = true;
    [HideInInspector] public bool regenHea = true;
    [HideInInspector] public bool run = true;

    void Start()
    {
        reference = GameObject.Find("Player").GetComponent<referenceCarrier>();
        controllerScript = GetComponent<playerController>();
        charAnimator = GetComponent<Animator>();
       // invScript = GetComponent<inventoryManagement>();
    }

    void FixedUpdate()
    {

        regenTime += Time.deltaTime;
        //Runs 30 times every second
        if (regenTime >= regenInterpolationTime)
        {
            regenTime -= regenInterpolationTime; 
            //The player is not busy
            if (!controllerScript.actionsBusy)
            {
                /*STAMINA****************************************************/
                //Decrease stamina by N if sprinting
                if (controllerScript._input.sprint && (controllerScript._input.move.x != 0 || controllerScript._input.move.y != 0) && currentStamina > 0 && run)
                {

                    if (currentStamina < 1)
                    {
                        run = false;
                        StopAllCoroutines();
                        StartCoroutine(cantRun(5f));
                    }

                    //Disable left shift and hit again to be able to run
                    if (run)
                    {
                        //Spends full sprintStamina per second while running
                        currentStamina -= controllerScript.sprintStamina / 30;

                    }

                    regenSta = false;
                    StopAllCoroutines();
                    StartCoroutine(regenStamina(3f));
                }
                //Regen stamina if no action is ocurring and stamina is below max
                 else if(currentStamina < maxStamina  && regenSta)
                {
                    //Regen full staminaRegenRate 
                    currentStamina += staminaRegenRate/30;
                    //Update current health if goes over
                    if (currentStamina > maxStamina)
                        currentStamina = maxStamina;
                }


                /*HEALTH****************************************************/
                {
                    //If the player is not hit and moving they regen half hp
                    //If the player is not hit and not moving they regen full hp
                    //If the player is hit  or well is in combat then do not regen any hp


                    //If player is not doing any actions, is moving and health over 1 and under 100
                    //Not hit, moving regen half hp, regenHea sets a delay if any action is performed
                }
                if (currentHealth < maxHealth && currentHealth > 1 && regenHea && (controllerScript._input.move.x != 0 || controllerScript._input.move.y != 0))
                {
                    //Regen half HP if moving
                    currentHealth += (healthRegenRate / 30) / 2;
                    //Update current health if goes over
                    if (currentHealth > maxHealth)
                        currentHealth = maxHealth;

                }
                //Is not hit, no actions and not moving regen full HP
                else if (currentHealth < maxHealth && currentHealth > 1 && regenHea
                    && controllerScript._input.move.x == 0 && controllerScript._input.move.y == 0)
                {
                    currentHealth += (healthRegenRate / 30);
                    //Update current health if goes over
                    if (currentHealth > maxHealth)
                        currentHealth = maxHealth; 
                } 

            }

        }
       
        //If the player rolls, attack or jumps then delay health regen and stamina by 3 seconds
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKey(KeyCode.Space) || (Input.GetKeyDown(KeyCode.Mouse0) && charAnimator.GetBool("hasWeapon")))
        {
            StopAllCoroutines();
            regenHea = false;
            regenSta = false;
            StartCoroutine(regenStamina(3f));
            StartCoroutine(regenHealth(3f));
        }





        /*  //Runs 1 time every 1 seconds 
          currentTime += Time.deltaTime;
           if (currentTime >= InterpolationPeriod)
           {
               currentTime -= InterpolationPeriod; 

          }*/

        reference.healthBar.value = currentHealth / maxHealth;
        reference.staminaBar.value = currentStamina / maxStamina;

        setStats();

    }


    float defense = 0, attSpeed = 0, damage = 0, shieldBlockDamage = 0;

    //Adds to stats
    public void refreshStats(Item itemEquipped)
    {


        //Get charEquipment
            //Get item stats list from item
                foreach (string itemStats in itemEquipped.stats)
                {
                       
                    string [] stat = itemStats.Split(':');

                    //Find the stats
                    if (stat[0].ToLower() == "defense")
                    {
                        defense += float.Parse(stat[1]);
                    }
                    else if (stat[0].ToLower() == "block damage")
                    {
                        shieldBlockDamage += float.Parse(stat[1]);
                    }
                    else if (stat[0].ToLower() == "damage")
                    {
                        damage += float.Parse(stat[1]);
                    }
                    else if (stat[0].ToLower() == "attack speed")
                    {
                        attSpeed += float.Parse(stat[1]);
                    }
                    else if (stat[0].ToLower() == "hp")
                    {
                        maxHealth += float.Parse(stat[1]);
                    }



                }

    }

    //Substracts from stats
    public void sRefreshStats(Item itemEquipped)
    {


        //Get charEquipment
        //Get item stats list from item
        foreach (string itemStats in itemEquipped.stats)
        {

            string[] stat = itemStats.Split(':');

            //Find the stats
            if (stat[0].ToLower() == "defense")
            {
                defense -= float.Parse(stat[1]);
            }
            else if (stat[0].ToLower() == "block damage")
            {
                shieldBlockDamage -= float.Parse(stat[1]);
            }
            else if (stat[0].ToLower() == "damage")
            {
                damage -= float.Parse(stat[1]);
            }
            else if (stat[0].ToLower() == "attack speed")
            {
                attSpeed -= float.Parse(stat[1]);
            }
            else if (stat[0].ToLower() == "hp")
            {
                maxHealth -= float.Parse(stat[1]);
            }



        }

        //Update current health
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
            reference.healthBar.value = currentHealth / maxHealth;
        }



    }


    void setStats()
    {
        //Update the UI stats
        //PlayerStatsL = Defense: 5 digits max, Att.Speed: 5 digits max, Damage: 5 digits max, Dmg.Block: 3 digits max
        playerStatsL.text = $"Defense: {defense}\nAtt.Speed: {attSpeed}\nDamage: {damage}\nDmg.Block: {shieldBlockDamage}";
        //PlayerStatsR = HP: 4digits/4digits, St: 4digits/4digits, HP regen: 2 digits, St regen: 2 digits
        playerStatsR.text = $"HP: {Mathf.FloorToInt(currentHealth)}/{maxHealth} \nSt: {Mathf.FloorToInt(currentStamina)}/{maxStamina}\nHP regen: {healthRegenRate}/s\nSt regen: {healthRegenRate}/s";
        //Set current coins
        currentCoinsText.text = currentCoins.ToString();
    }


    FINISH POTIONS AND COINS, ADD PREFABS FOR POTIONS, SINGLE COIN FALLS TO THE VOID, DISABLE POTION RARITY.












































































    public IEnumerator regenStamina(float time)
    {
        yield return new WaitForSeconds(time);
        regenSta = true;
    }
    public IEnumerator cantRun(float time)
    {
        yield return new WaitForSeconds(time);
        run = true;
    }

    public IEnumerator regenHealth(float time)
    {
        yield return new WaitForSeconds(time);
        regenHea = true;
    }




    //STAMINA
    public float getStamina()
    {
        return currentStamina;
    }
    public void deductStamina(float deductedPoints)
    {
        currentStamina -= deductedPoints;
        reference.staminaBar.value = currentStamina / maxStamina;
    }


    //HEALTH
    public float getHealth()
    {
        return currentHealth;
    }
    public void deductHealth(float deductedPoints)
    {
        currentHealth -= deductedPoints;
        reference.healthBar.value = currentHealth / maxHealth;
    }

}
