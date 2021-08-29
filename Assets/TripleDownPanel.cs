using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TripleDownPanel : MonoBehaviour
{
    [SerializeField]
    private int m_rawCount;

    [SerializeField]
    private int m_columnCount;

    [SerializeField]
    [Range(4,10)]
    private int m_itemTypeRange;

    [SerializeField]
    private Button m_replay;

    [SerializeField]
    private Slot m_baseSlot;

    [SerializeField]
    private Item m_baseItem;    

    [SerializeField]
    private Transform m_board;

    [SerializeField]
    private Transform m_mask;

    [SerializeField]
    private GridLayoutGroup m_layOut;

    //private int[,] itemVals;

    private Slot[,] slots;

    private List<Item> itemList;
    private int round;

    public static TripleDownPanel Instance{ get; private set;}

    void Start()
    {
        Instance = this;
        m_columnCount = 7;
        m_rawCount = 6;
        //itemVals = new int[m_rawCount, m_columnCount];
        slots = new Slot[m_columnCount, m_rawCount];
        itemList = new List<Item>(m_columnCount * m_rawCount);
        m_layOut.constraintCount = m_columnCount;
        //InitItemsValue();
        this.StartCoroutine(InitSlots());

        m_replay.onClick.AddListener(()=>
        {
            this.StartCoroutine(UpdateBorad());
        });
    }

    IEnumerator InitSlots()
    {
        for(int i = 0; i < m_columnCount; i++)
        {
            var slot = GameObject.Instantiate(m_baseSlot);
            slot.gameObject.name = string.Format("slot{0},{1}", i, m_rawCount - 1);
            slot.transform.SetParent(m_board);
            slot.transform.localScale = Vector3.one;
            slot.gameObject.SetActive(true);
            slots[i,m_rawCount-1] = slot;
            slot.SetPos(i,m_rawCount-1);
        }

        yield return null;

        for(int i = 0; i < m_columnCount; i++)
        {
            var item = GameObject.Instantiate<Item>(m_baseItem);
            item.gameObject.name = string.Format("item{0},{1}", i, m_rawCount - 1);
            itemList.Add(item);
            item.transform.parent = this.m_mask;
            item.SetUp(Random.Range(1, m_itemTypeRange), slots[i, m_rawCount-1]);
            item.gameObject.SetActive(true);
        }



        for(int i = m_rawCount - 2; i >= 0; i--)
        {
            for(int j = 0; j < m_columnCount; j++)
            {
                var slot = GameObject.Instantiate(m_baseSlot);
                slot.gameObject.name = string.Format("slot{0},{1}", j, i);
                slot.transform.SetParent(m_board);
                slot.transform.localScale = Vector3.one;
                slot.gameObject.SetActive(true);
                slots[j,i] = slot;
                slot.SetPos(j,i);

                yield return null;

                var item = GameObject.Instantiate<Item>(m_baseItem);
                item.gameObject.name = string.Format("item{0},{1}", j, i);
                itemList.Add(item);
                item.transform.parent = this.m_mask;
                item.SetUp(Random.Range(1, m_itemTypeRange), slot);
                item.gameObject.SetActive(true);
            }
            yield return null;
        }

        this.StartCoroutine(UpdateBorad());
    }

    public IEnumerator UpdateBorad()
    {
        var rets = CheckBorad();
        Dictionary<int,int> columCounter = new Dictionary<int, int>(); 
        HashSet<Item> items = new HashSet<Item>();
        while(rets.Count > 0)
        {

            for(int i = 0; i < rets.Count; i++)
            {
                var list = rets[i];
                var target = list[0];
                for(int j = 0; j < list.Count; j++)
                {
                    var item = list[j];
                    int x = item.slot.x;
                    int y = item.slot.y;


                    if(items.Contains(item))
                    {
                        continue;
                    }

                    items.Add(item);

                    if(columCounter.ContainsKey(x))
                    {
                        columCounter[x]++;
                    }
                    else
                    {
                        columCounter.Add(x,1);
                    }

                    for(int index = y + 1; index < m_rawCount; index ++)
                    {
                        var s = slots[x,index];
                        if(s.isLinked)
                        {
                            continue;    
                        }
                        int targetY = s.item.targetSlot == null 
                            ? index - 1 : s.item.targetSlot.y - 1; 
                        s.item.SetTarget(slots[x, targetY]);
                        items.Add(s.item);
                    }
                
                    int t = columCounter[x];
                    item.Move2Slot(target.slot,()=>
                    {
                        item.SetTarget(slots[x, m_rawCount - t]);
                        item.SetUpValue(Random.Range(1, m_itemTypeRange));
                    });
                }
            }

            yield return new WaitForSeconds(.5f);
            yield return null;

            foreach(var i in items)
            {
                i.Move2Target();
                i.slot.UpdateLinked(false);
            }

            yield return new WaitForSeconds(.5f);
            yield return null;

            columCounter.Clear();
            items.Clear();

            rets = CheckBorad();

        }
    }

    public Item GetLeft(Slot slot)
    {
        int x = slot.x - 1;
        if(x >= 0)
        {
            return slots[x, slot.y].item;
        }

        return null;
    }

    public Item GetRight(Slot slot)
    {
        int x = slot.x + 1;
        if(x < m_columnCount)
        {
            return slots[x, slot.y].item;
        }

        return null;
    }

    public Item GetUpper(Slot slot)
    {
        int y = slot.y + 1;
        if(y < m_rawCount)
        {
            return slots[slot.x, y].item;
        }

        return null;
    }


    public Item GetLower(Slot slot)
    {
        int y = slot.y - 1;
        if(y >= 0)
        {
            return slots[slot.x, y].item;
        }

        return null;
    }   

    private List<List<Item>> CheckBorad()
    {
        round++;

        List<List<Item>> lists = new List<List<Item>>();
        for(int j = 0; j < m_rawCount; j++)
        {
            for(int i = 0; i < m_columnCount; i++)
            {
                var slot = slots[i,j];
                if(slot.checkRound == round)
                {
                    continue;
                }

                slot.UpdateCheckRound(round);
                var right = GetRight(slot);
                var up = GetUpper(slot);
                int counter = 1;

                List<Item> sameList = null;
                while(right!= null && right.typeVal == slot.item.typeVal)
                {
                    if(right == slot.item)
                    {
                        Debug.LogError(right);
                        break;
                    }

                    counter++;

                    if(counter > 20)
                    {
                        Debug.LogError(counter);
                        break;
                    }

                    if(counter == 3)
                    {
                        sameList = new List<Item>();
                        sameList.Add(slot.item);
                        sameList.Add(right);
                        right.slot.UpdateCheckRound(round);
                        var second = slots[i+1,j];
                        second.UpdateCheckRound(round);
                        sameList.Add(second.item);
                    }
                    else if(counter > 3)
                    {
                        sameList.Add(right);
                        right.slot.UpdateCheckRound(round);
                    }
                    right = GetRight(right.slot);
                }

                if(counter >= 3)
                {
                    i += counter -1;
                }

                counter = 1;
                while(up!= null && up.typeVal == slot.item.typeVal)
                {
                    if(up == slot.item)
                    {
                        Debug.LogError(up);
                        break;
                    }
                    counter++;

                    if(counter > 20)
                    {
                        Debug.LogError(counter);
                        break;
                    }

                    if(counter == 3)
                    {
                        if(sameList == null)
                        {
                            sameList = new List<Item>();
                            sameList.Add(slot.item);
                        }
                        sameList.Add(up);
                        var second = slots[i,j+1];
                        second.UpdateCheckRound(round);
                        up.slot.UpdateCheckRound(round);
                        sameList.Add(second.item);
                    }
                    else if(counter > 3)
                    {
                        sameList.Add(up);
                        up.slot.UpdateCheckRound(round);
                    }
                    up = GetUpper(up.slot);                   
                }

                if(sameList != null)
                {
                    sameList.ForEach(item => item.slot.UpdateLinked());
                    lists.Add(sameList);
                } 
            }
        }

        return lists;
    } 

}
