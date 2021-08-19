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
    [Range(5,10)]
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
    private GridLayoutGroup m_layOut;

    //private int[,] itemVals;

    private Slot[,] slots;

    private List<Item> itemList;

    public static TripleDownPanel Instance{ get; private set;}

    void Start()
    {
        Instance = this;
        //itemVals = new int[m_rawCount, m_columnCount];
        slots = new Slot[m_rawCount, m_columnCount];
        itemList = new List<Item>(m_columnCount * m_rawCount);
        m_layOut.constraintCount = m_rawCount;
        //InitItemsValue();
        this.StartCoroutine(InitSlots());
    }

    IEnumerator InitSlots()
    {
        for(int i = 0; i < m_columnCount; i++)
        {
            var slot = GameObject.Instantiate(m_baseSlot);
            slot.gameObject.name = string.Format("slot0,{0}", i);
            slot.transform.SetParent(m_board);
            slot.transform.localScale = Vector3.one;
            slot.gameObject.SetActive(true);
            slots[0,i] = slot;
            slot.SetPos(0,i);
        }

        yield return null;

        for(int i = 0; i < m_columnCount; i++)
        {
            var item = GameObject.Instantiate<Item>(m_baseItem);
            item.gameObject.name = string.Format("item0,{0}", i);
            itemList.Add(item);
            item.transform.parent = this.transform;
            item.SetUp(Random.Range(1, m_itemTypeRange), slots[0,i]);
            item.gameObject.SetActive(true);
        }



        for(int i = 1; i < m_rawCount; i++)
        {
            for(int j = 0; j < m_columnCount; j++)
            {
                var slot = GameObject.Instantiate(m_baseSlot);
                slot.gameObject.name = string.Format("slot{0},{1}", i, j);
                slot.transform.SetParent(m_board);
                slot.transform.localScale = Vector3.one;
                slot.gameObject.SetActive(true);
                slots[i,j] = slot;
                slot.SetPos(i,j);

                yield return null;

                var item = GameObject.Instantiate<Item>(m_baseItem);
                item.gameObject.name = string.Format("item{0},{1}", i, j);
                itemList.Add(item);
                item.transform.parent = this.transform;
                item.SetUp(Random.Range(1, m_itemTypeRange), slot);
                item.gameObject.SetActive(true);
            }
            yield return null;
        }
    }

    public Item GetLeft(Item item)
    {
        int x = item.slot.x - 1;
        if(x >= 0)
        {
            return slots[x, item.slot.y].item;
        }

        return null;
    }

    public Item GetRight(Item item)
    {
        int x = item.slot.x + 1;
        if(x < m_rawCount)
        {
            return slots[x, item.slot.y].item;
        }

        return null;
    }

    public Item GetUpper(Item item)
    {
        int y = item.slot.y + 1;
        if(y < m_columnCount)
        {
            return slots[item.slot.x, y].item;
        }

        return null;
    }


    public Item GetLower(Item item)
    {
        int y = item.slot.y - 1;
        if(y >= 0)
        {
            return slots[item.slot.x, y].item;
        }

        return null;
    }    

}
