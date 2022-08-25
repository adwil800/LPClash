
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class itemSlot : MonoBehaviour, IDropHandler
{

    DragDrop dragDropScript;
    inventoryManagement invMngScript;
    Transform Julia;
    Transform floorItems;

    private void Start()
    {
        invMngScript = GameObject.Find("Julia").GetComponent<inventoryManagement>();
        Julia = GameObject.Find("Julia").GetComponent<Transform>();
        floorItems = GameObject.Find("floorItems").GetComponent<Transform>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
                //Get dragDrop script from dragged item
                dragDropScript = eventData.pointerDrag.GetComponent<DragDrop>();
                //Get parent tag to determine whether it's coming from the inventory or the charEquipment section
                string currentInvTag = dragDropScript.currentItemParent.parent.tag;
                

                //Get dropped target tag
                if(transform.tag == "Droppable")
                {
                        
                    //Inventory dropping
                    if(currentInvTag == "inventory"){ 
                    //Get item slot and find prefab
                    int slot = int.Parse(dragDropScript.currentItemParent.name);

                    GameObject item = Instantiate(invMngScript.getInvPrefab(slot - 1, invMngScript.inventory), Julia.position + (Julia.up * 1.5f) + (Julia.forward), Julia.rotation) as GameObject;
                               item.name = invMngScript.findItem(slot - 1, invMngScript.inventory).prefabName;
                               item.transform.SetParent(floorItems);

                    //No need to disable box collider on drop for weapons and shield, the instantiated prefab has only got the mesh collider active


                    //Delete dropped item
                    invMngScript.inventory.deleteItem(invMngScript.findItem(slot-1, invMngScript.inventory));

                    //Return itemslot to their original parent and center it
                    eventData.pointerDrag.transform.SetParent(dragDropScript.currentItemParent);
                    eventData.pointerDrag.transform.localPosition = new Vector2(0f, 0f);

                    invMngScript.refreshInventoryItems();
                    }
                    //CharEquipment dropping
                    else if(currentInvTag == "charEquipment"){


                        if (invMngScript.swordToBack || invMngScript.shieldToBack)
                        {
                            invMngScript.equippedWeaponsToFront();
                        }

                        //Get item slot and find prefab
                        int slot = getCharSlot(dragDropScript.currentItemParent.name);
                        //Debug.Log(slot);

                        GameObject item = Instantiate(invMngScript.getInvPrefab(slot, invMngScript.charEquipment), Julia.position + (Julia.up * 1.5f) + (Julia.forward), Julia.rotation) as GameObject;
                                   item.name = invMngScript.findItem(slot, invMngScript.charEquipment).prefabName;
                                   item.transform.SetParent(floorItems);
                  
                               //No need to disable box collider on drop for weapons and shield, the instantiated prefab has only got the mesh collider active
                    

                        //UNEQUIP ITEMS FROM PLAYER
                        restorePlayerEquipment(slot, invMngScript.findItem(slot, invMngScript.charEquipment));

                        //Refresh stats, substract
                        invMngScript.statsScript.sRefreshStats(invMngScript.findItem(slot, invMngScript.charEquipment));


                        //Delete dropped item
                        invMngScript.charEquipment.deleteItem(invMngScript.findItem(slot, invMngScript.charEquipment));

                        //Return itemslot to their original parent and center it
                        eventData.pointerDrag.transform.SetParent(dragDropScript.currentItemParent);
                        eventData.pointerDrag.transform.localPosition = new Vector2(0f, 0f);



                        invMngScript.refreshEquippedItems();





                }

                return;
                }


            Image slotFullImage =  null;
            if (transform.childCount > 0)
                slotFullImage = transform.GetChild(0).GetComponent<Image>();
            else
            {
                //Prevents transform out of bound error due to the fact that the dragged item hasn't totally landed on the correct location
                //Debug.Log("Would've errored out");

                //Clear slots if anything 
                dragDropScript.enableAllSlots(false);
                return;
            }

            //Inventory switching
            if (currentInvTag == "inventory")
            {
                //Slot ints to find the correct item in the inventory
                int fromSlot = int.Parse(dragDropScript.currentItemParent.name);
                int toSlot = int.Parse(transform.name);

                //Check if there's already an item in toSlot
                if (slotFullImage.enabled)
                {

                    Item fromSlotItem = invMngScript.findItem(fromSlot - 1, invMngScript.inventory);
                    Item toSlotItem = invMngScript.findItem(toSlot - 1, invMngScript.inventory);

                    if(fromSlotItem == null || toSlotItem == null)
                    {

                        //Prevents object reference error due to the fact that the dragged item hasn't totally landed on the correct location
                        //Debug.Log("Would've errored out 2");

                        //Clear slots if anything 2
                        dragDropScript.enableAllSlots(false);
                        return;
                    }
                    //Switch item positions
                    int slotHolder = fromSlotItem.slot;
                    fromSlotItem.slot = toSlotItem.slot;
                    toSlotItem.slot = slotHolder;

                    //Recreate new items 
                    invMngScript.inventory.addItem(fromSlotItem);
                    invMngScript.inventory.addItem(toSlotItem);
                    //Delete old items
                    invMngScript.inventory.deleteItem(fromSlotItem);
                    invMngScript.inventory.deleteItem(toSlotItem);


                    //Return fromSlotItem to their original parent and center it
                    eventData.pointerDrag.transform.SetParent(dragDropScript.currentItemParent);
                    eventData.pointerDrag.transform.localPosition = new Vector2(0f, 0f);

                    invMngScript.refreshInventoryItems();
                }
                //If theres no item in destination then just move
                else
                {

                    Item fromSlotItem = invMngScript.findItem(fromSlot - 1, invMngScript.inventory);

                    if (fromSlotItem == null)
                    {

                        //Prevents object reference error due to the fact that the dragged item hasn't totally landed on the correct location
                        //Debug.Log("Would've errored out 2");

                        //Clear slots if anything 3
                        dragDropScript.enableAllSlots(false);
                        return;
                    }

                    //Switch item positions
                    fromSlotItem.slot = toSlot - 1;

                    //Create new item with new slot
                    invMngScript.inventory.addItem(fromSlotItem);
                    //Delete old item with new slot
                    invMngScript.inventory.deleteItem(fromSlotItem);

                    //Return fromSlotItem to their original parent and center it
                    eventData.pointerDrag.transform.SetParent(dragDropScript.currentItemParent);
                    eventData.pointerDrag.transform.localPosition = new Vector2(0f, 0f);

                    invMngScript.refreshInventoryItems();



                }
            }


        }










    }

    public GameObject FindObjectInChilds(GameObject gameObject, string gameObjectName)
    {
        Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
        foreach (Transform item in children)
        {
            if (item.name == gameObjectName)
            {
                return item.gameObject;
            }
        }

        return null;
    }

    //ADD arms TO SET 02
    //Correct char inventory slot
    int getCharSlot(string slotName)
    {

        //Debug.Log(findItem(slot).prefabName.ToLower());

        switch (slotName.ToLower())
        {
            default:
            case "helmetslot":      return 0;
            case "shoulderslot":    return 1;
            case "armorslot":       return 2;
            case "pantsslot":       return 3;
            case "shoeslot":        return 4;
            case "armsslot":        return 5;
            case "swordslot":       return 6;
            case "shieldslot":      return 7;
        }

    }


    float clicked = 0;
    float clicktime = 0;
    float clickdelay = 0.5f;

    public void unequipItem(int slot)
    {
            // Detecting double click
        clicked++;

        if (clicked == 1)
            clicktime = Time.time;

        if (clicked > 1 && Time.time - clicktime < clickdelay)
        {
            // Double click detected
            clicked = 0;
            clicktime = 0;


            //Find the item to unequip 
            Item item = invMngScript.findItem(slot, invMngScript.charEquipment);
                //Unequip from visuals    
                restorePlayerEquipment(slot, item);

                item.slot = invMngScript.availableSlots(); //Assign an inventory slot

                //Send back to inventory
                invMngScript.inventory.addItem(item);
                //substract
                invMngScript.statsScript.sRefreshStats(item);
                //Remove item from char equipment
                invMngScript.charEquipment.deleteItem(item);

                //Refresh both inventories
                invMngScript.refreshInventoryItems();
                invMngScript.refreshEquippedItems();
               


        }
        else if (clicked > 2 || Time.time - clicktime > 1)
            clicked = 0;

    }


    void restorePlayerEquipment(int itemSlot, Item droppedItem)
    {

        switch (itemSlot)
        {
            //Is helmet
            case 0:
                    //Disable current head and enable base one
                    GameObject currentHead = FindObjectInChilds(Julia.gameObject, droppedItem.prefabName.Replace("_Static", ""));
                    GameObject baseHead = FindObjectInChilds(Julia.gameObject, "Chr_Head_Female_02");
                    GameObject baseHair = FindObjectInChilds(Julia.gameObject, "Chr_Hair_38");
                    if (currentHead != null && baseHead != null && baseHair != null)
                    {
                        currentHead.SetActive(false);
                        baseHead.SetActive(true);
                        baseHair.SetActive(true);
                    }
                    else
                    {
                        Debug.Log("Something has gone terribly wrong!\nRestart the game.");
                    }

                break;

            //PrefabName only contains the RIGHT side of shoulders
            //Is shoulders
            case 1:
                //Disable existing if there ever were
                    GameObject oldShoulderPieceR = FindObjectInChilds(Julia.gameObject, droppedItem.prefabName.Replace("_Static", ""));
                    GameObject oldShoulderPieceL = FindObjectInChilds(Julia.gameObject, droppedItem.prefabName.Replace("_Static", "").Replace("Right", "Left"));
                    if (oldShoulderPieceR != null && oldShoulderPieceL != null)
                    {
                        oldShoulderPieceR.SetActive(false);
                        oldShoulderPieceL.SetActive(false);
                    }
                    else
                    {
                        Debug.Log("Something has gone terribly wrong!\nRestart the game.");
                    }

                break;

            //Is torso
            case 2:
                //Disable current torso and enable base one
                    GameObject currentTorso = FindObjectInChilds(Julia.gameObject, droppedItem.prefabName.Replace("_Static", ""));
                    GameObject baseTorso = FindObjectInChilds(Julia.gameObject, "Chr_Torso_Female_03");
                    if (currentTorso != null && baseTorso != null)
                    {
                        currentTorso.SetActive(false);
                        baseTorso.SetActive(true);
                    }
                    else
                    {
                        Debug.Log("Something has gone terribly wrong!\nRestart the game.");
                    }

                break;

            //Is hips
            case 3:
                    //Disable current hips and enable base one
                    GameObject currentHips = FindObjectInChilds(Julia.gameObject, droppedItem.prefabName.Replace("_Static", ""));
                    GameObject baseHips = FindObjectInChilds(Julia.gameObject, "Chr_Hips_Female_03");
                    if (currentHips != null && baseHips != null)
                    {
                        currentHips.SetActive(false);
                        baseHips.SetActive(true);
                    }
                    else
                    {
                        Debug.Log("Something has gone terribly wrong!\nRestart the game.");
                    }

                break;

            //PrefabName only contains the RIGHT side of boots
            //Is boots
            case 4:
                //Disable existing if there ever were
                GameObject currentLegPieceR = FindObjectInChilds(Julia.gameObject, droppedItem.prefabName.Replace("_Static", ""));
                GameObject currentLegPieceL = FindObjectInChilds(Julia.gameObject, droppedItem.prefabName.Replace("_Static", "").Replace("Right", "Left"));
                GameObject baseLegPieceR = FindObjectInChilds(Julia.gameObject, "Chr_LegRight_Female_03");
                GameObject baseLegPieceL = FindObjectInChilds(Julia.gameObject, "Chr_LegLeft_Female_03");

                if (currentLegPieceR != null && currentLegPieceL != null && baseLegPieceR != null && baseLegPieceL != null)
                {
                    currentLegPieceR.SetActive(false);
                    currentLegPieceL.SetActive(false);

                    baseLegPieceR.SetActive(true);
                    baseLegPieceL.SetActive(true);
                }
                else
                {
                    Debug.Log("Something has gone terribly wrong!\nRestart the game.");
                }

                break;
            //Is arms
            case 5:
                //Disable existing if there ever were
                GameObject currentGlovePieceR = FindObjectInChilds(Julia.gameObject, droppedItem.prefabName.Replace("_Static", ""));
                GameObject currentGlovePieceL = FindObjectInChilds(Julia.gameObject, droppedItem.prefabName.Replace("_Static", "").Replace("Right", "Left"));

                //Custom lower and upper arm
                GameObject currentLowerArmPieceR = FindObjectInChilds(Julia.gameObject, droppedItem.prefabName.Replace("_Static", "").Replace("Hand", "ArmLower"));
                GameObject currentLowerArmPieceL = FindObjectInChilds(Julia.gameObject, droppedItem.prefabName.Replace("_Static", "").Replace("Hand", "ArmLower").Replace("Right", "Left"));

                GameObject currentUpperArmPieceR = FindObjectInChilds(Julia.gameObject, droppedItem.prefabName.Replace("_Static", "").Replace("Hand", "ArmUpper"));
                GameObject currentUpperArmPieceL = FindObjectInChilds(Julia.gameObject, droppedItem.prefabName.Replace("_Static", "").Replace("Hand", "ArmUpper").Replace("Right", "Left"));

                //Base arms
                GameObject baseArmPieceR = FindObjectInChilds(Julia.gameObject, "Chr_HandRight_Female_03");
                GameObject baseArmPieceL = FindObjectInChilds(Julia.gameObject, "Chr_HandLeft_Female_03");
                GameObject baseLowerArmPieceR = FindObjectInChilds(Julia.gameObject, "Chr_ArmLowerRight_Female_03");
                GameObject baseLowerArmPieceL = FindObjectInChilds(Julia.gameObject, "Chr_ArmLowerLeft_Female_03");
                GameObject baseUpperArmPieceR = FindObjectInChilds(Julia.gameObject, "Chr_ArmUpperRight_Female_03");
                GameObject baseUpperArmPieceL = FindObjectInChilds(Julia.gameObject, "Chr_ArmUpperLeft_Female_03");
                //Chr_ArmLowerRight_Female_06 Chr_ArmUpperRight_Female_06 original : Chr_ArmLowerRight_Female_03 Chr_ArmUpperRight_Female_03 
                if (currentGlovePieceR != null && currentGlovePieceL != null && currentLowerArmPieceR != null && currentLowerArmPieceL != null &&
                    currentUpperArmPieceR != null && currentUpperArmPieceL != null && baseArmPieceR != null && baseArmPieceL != null &&
                    baseLowerArmPieceR != null && baseLowerArmPieceL != null && baseUpperArmPieceR != null && baseUpperArmPieceL != null)
                {
                    currentGlovePieceR.SetActive(false);
                    currentGlovePieceL.SetActive(false);
                    currentLowerArmPieceR.SetActive(false);
                    currentLowerArmPieceL.SetActive(false);
                    currentUpperArmPieceR.SetActive(false);
                    currentUpperArmPieceL.SetActive(false);


                    baseArmPieceR.SetActive(true);
                    baseArmPieceL.SetActive(true);
                    baseLowerArmPieceR.SetActive(true);
                    baseLowerArmPieceL.SetActive(true);
                    baseUpperArmPieceR.SetActive(true);
                    baseUpperArmPieceL.SetActive(true);




                }
                else
                {
                    Debug.Log("Something has gone terribly wrong!\nRestart the game.");
                }

                break;

            //Is weapon
            case 6:
                    //Look for current weapon and destroy it
                    GameObject currentWeapon = FindObjectInChilds(Julia.gameObject, "weaponSlot");
                    if(currentWeapon != null)
                    {
                        if(currentWeapon.transform.childCount > 0)
                        {
                            Destroy(currentWeapon.transform.GetChild(0).gameObject);
                            Julia.GetComponent<Animator>().SetBool("hasWeapon", false);
                        }
                    }

                break;

            //Is shield
            case 7:
                //Look for current shield and destroy it
                GameObject currentShield = FindObjectInChilds(Julia.gameObject, "shieldSlot");
                if (currentShield != null)
                {
                    if (currentShield.transform.childCount > 0)
                    {
                        Destroy(currentShield.transform.GetChild(0).gameObject);
                        Julia.GetComponent<Animator>().SetBool("hasShield", false);
                    }
                }

                break;
        }

    }

}
