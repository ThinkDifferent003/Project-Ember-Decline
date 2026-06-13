using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedItem 
{
    private string _id;
    private int _count;
    public string ID
    {
        get => _id; 
        set => _id = value;
    }  
    public int Count
    {
        get => _count;
        set => _count = value;
    }
}
