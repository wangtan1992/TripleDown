using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour,IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private Image m_icon;


    public int typeVal { get; private set;}

    public Slot slot { get; private set; }

    public void SetUp(int itemType, Slot t)
    {
        this.m_icon.sprite = UnityEditor.AssetDatabase
            .LoadAssetAtPath<Sprite>(string.Format("Assets/UnLoadResources/Textures/A_{0:D2}.png", itemType));
        this.typeVal = itemType;
        this.slot = t;
        this.slot.SetItem(this);
        transform.position = slot.transform.position;
    }


    public void Move2Target(Slot t, TweenCallback action)
    {
        var tweener = DOTween.To(()=> transform.position, 
        x => transform.position = x, t.transform.position, .5f);

        tweener.onComplete += action;
    }

    public void ChangeSlot(Slot newTarget)
    {
        this.slot = newTarget;
        slot.SetItem(this);
    }

    public void SwapPos(Item other)
    {
        if(other == null)
        {
            return;
        }
        Move2Target(other.slot, ()=>{ ChangeSlot(other.slot); });
        other.Move2Target(slot, ()=>{ ChangeSlot(slot); });
    }


    public void OnBeginDrag(PointerEventData data)
    {
        transform.position = data.position;
        transform.SetSiblingIndex(1000);
    }

    public void OnDrag(PointerEventData data)
    {
        float x = Mathf.Clamp(data.position.x, slot.transform.position.x - 100, slot.transform.position.x + 100);
        float y = Mathf.Clamp(data.position.y, slot.transform.position.y - 100, slot.transform.position.y + 100);

        transform.position = new Vector3(x, y);
    }

    public void OnEndDrag(PointerEventData data)
    {
        float dis = Vector3.Distance(data.position, slot.transform.position);
        if(dis > 100)
        {
            var moveX = data.position.x - slot.transform.position.x;
            var moveY = data.position.y - slot.transform.position.y;

            Item toSwap = null;
            if(Mathf.Abs(moveX) > Mathf.Abs(moveY))
            {

                if(moveX < 0)
                {
                    toSwap = TripleDownPanel.Instance.GetLeft(this);
                }
                else
                {
                    toSwap = TripleDownPanel.Instance.GetRight(this);
                }
            }
            else
            {

                if(moveY < 0)
                {
                    toSwap = TripleDownPanel.Instance.GetLower(this);
                }
                else
                {
                    toSwap = TripleDownPanel.Instance.GetUpper(this);
                }
            }

            if(toSwap)
            {
                SwapPos(toSwap);
            }
            else
            {
                Move2Target(slot, ()=>{ transform.SetSiblingIndex(10);});
            }
        }
        else
        {
            Move2Target(slot, ()=>{ transform.SetSiblingIndex(10);});
        }
    }
}
