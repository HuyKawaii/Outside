using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEconManager : MonoBehaviour
{
    private int goldAmount;
    
    public void GainGold(int goldGain)
    {
        goldAmount += goldGain;
    }

    public bool SpendGold(int goldSpent)
    {
        if (goldAmount < goldSpent) return false;
        goldAmount -= goldSpent;
        return true;
    }

    public int GetGoldAmount()
    {
        return goldAmount;
    }
}
