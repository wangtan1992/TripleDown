using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{

    [SerializeField]
    private Image m_bg;

    public Item item{ get; private set;}
    // Start is called before the first frame update

    public int x { get; private set;}
    public int y { get; private set;}
    public int checkRound {get; private set;}

    public bool isLinked{get; private set;}

    private bool hasInitPos = false;

    public void SetItem(Item _item)
    {
        this.item = _item;
    }

    public void UpdateCheckRound(int round )
    {
        this.checkRound = round;
    }

    public void UpdateLinked(bool linked = true)
    {
        if(linked)
        {
            m_bg.color = Color.black;
        }
        else
        {
            m_bg.color = Color.white;
        }
        this.isLinked = linked;
    }

    public void SetPos(int x, int y)
    {
        if(hasInitPos)
        {
            return;
        }
        hasInitPos = true;
        this.x = x;
        this.y = y;
    }
}
