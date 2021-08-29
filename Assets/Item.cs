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

    //当前属于哪个槽位
    public Slot slot { get; private set; }

    public Slot targetSlot { get; private set; }

    public void SetUp(int itemType, Slot t)
    {
        this.m_icon.sprite = UnityEditor.AssetDatabase
            .LoadAssetAtPath<Sprite>(string.Format("Assets/UnLoadResources/Textures/A_{0:D2}.png", itemType));
        this.typeVal = itemType;
        this.slot = t;
        this.slot.SetItem(this);
        transform.position = slot.transform.position;
    }


    public void Move2Slot(Slot t, TweenCallback action)
    {
        var tweener = DOTween.To(()=> transform.position, 
        x => transform.position = x, t.transform.position, .5f);

        tweener.onComplete += action;
    }

    public void Move2Target()
    {
        var tweener = DOTween.To(()=> transform.position, 
        x => transform.position = x, targetSlot.transform.position, .5f);

        tweener.onComplete += ()=> 
        {
            slot = targetSlot;
            targetSlot.SetItem(this);
            targetSlot = null;
        };        
    }

    public void ChangeSlot(Slot newTarget)
    {
        this.slot = newTarget;
        slot.SetItem(this);
    }

    public void SetTarget(Slot t)
    {
        if(this.slot.isLinked)
        {
            transform.position = new Vector3
            (
                t.transform.position.x, 
                t.transform.position.y + 1000,
                transform.position.z
            );            
        }

        this.targetSlot = t;
    }

    public void SetUpValue(int val)
    {
        this.typeVal = val;
        this.m_icon.sprite = UnityEditor.AssetDatabase
            .LoadAssetAtPath<Sprite>(
                string.Format("Assets/UnLoadResources/Textures/A_{0:D2}.png", 
                typeVal));
    }

    public IEnumerator SwapPos(Item other)
    {
        if(other == null)
        {
            yield break;
        }
        var mySlot = slot;

        bool swapFinish1 = false;
        bool swapFinish2 = false;
        Move2Slot(other.slot, ()=>{ 
            ChangeSlot(other.slot);
            swapFinish1 = true;
        });
        other.Move2Slot(slot, ()=>{ 
            other.ChangeSlot(mySlot);
            swapFinish2 = true;
        });

        yield return new WaitUntil(()=> swapFinish1 && swapFinish2);

        yield return TripleDownPanel.Instance.UpdateBorad();
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
                    toSwap = TripleDownPanel.Instance.GetLeft(slot);
                }
                else
                {
                    toSwap = TripleDownPanel.Instance.GetRight(slot);
                }
            }
            else
            {

                if(moveY < 0)
                {
                    toSwap = TripleDownPanel.Instance.GetLower(slot);
                }
                else
                {
                    toSwap = TripleDownPanel.Instance.GetUpper(slot);
                }
            }

            if(toSwap)
            {
                this.StartCoroutine(SwapPos(toSwap));
            }
            else
            {
                Move2Slot(slot, ()=>{ transform.SetSiblingIndex(10);});
            }
        }
        else
        {
            Move2Slot(slot, ()=>{ transform.SetSiblingIndex(10);});
        }
    }
}
