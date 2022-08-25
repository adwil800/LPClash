using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    //GRAB invScript and set a trigger when an item is being dragged
    inventoryManagement invMngScript;

    [SerializeField] private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    public GameObject inv;
    
    [HideInInspector] public Transform currentItemParent;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        invMngScript = GameObject.Find("Julia").GetComponent<inventoryManagement>();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("Begin drag");

        //Enable all slots and temporarily disable raycast so a new item can be dropped
        //Get current item parent to bring back if anything
        currentItemParent = eventData.pointerDrag.transform.parent.transform;
        enableAllSlots(true);

        //Set new parent to be the inventory so it's above everything
        eventData.pointerDrag.transform.SetParent(inv.transform.GetChild(0));

        //Assign pointerDragItem in case the dragging event is interrupted by closing the inventory

        //Set true to an item being dragged
        invMngScript.dragging = true;

        //If somehow dragging becomes false
        if (!invMngScript.dragging)
        {
            enableAllSlots(false);
        }

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        //If somehow dragging becomes false
        if (!invMngScript.dragging)
        {
            enableAllSlots(false);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;  
        
        //If the parent is not a slot the pull it back to the original slot
        if (eventData.pointerDrag.transform.parent.tag != "invSlot")
        {
            //Return fromSlotItem to their original parent and center it
            eventData.pointerDrag.transform.SetParent(currentItemParent);
            eventData.pointerDrag.transform.localPosition = new Vector2(0f, 0f);
        }

        //Set false to an item being dragged
        invMngScript.dragging = false;


        //Disable moving slot
        enableAllSlots(false);

    }

    public void OnPointerDown(PointerEventData eventData)
    {
       // Debug.Log("Clicked");
    }










     
    public void enableAllSlots(bool enabled)
    {
        //Only enable the current frame
        if(currentItemParent.parent.tag == "charEquipment")
        {
            currentItemParent.GetComponent<Image>().enabled = enabled;
            //Disable raycast target
            Transform slotRenderer = currentItemParent.GetChild(0);
            slotRenderer.GetComponent<Image>().raycastTarget = !enabled;
        }
        //Enable all slot frames
        else if(currentItemParent.parent.tag == "inventory")
        {

            for (int i = 0; i < inv.transform.GetChild(0).childCount; i++)
            {
                //Get parent gameobject
                Transform slotParent = inv.transform.GetChild(0).GetChild(i);
                slotParent.GetComponent<Image>().enabled = enabled;
                //Disable raycast target
                Transform slotRenderer = slotParent.GetChild(0);
                slotRenderer.GetComponent<Image>().raycastTarget = !enabled;
            }

        }

    }
}
