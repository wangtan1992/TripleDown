using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{

    public Item item{ get; private set;}
    // Start is called before the first frame update

    public int x { get; private set;}
    public int y { get; private set;}

    private bool hasInitPos = false;

    public void SetItem(Item _item)
    {
        this.item = _item;
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
