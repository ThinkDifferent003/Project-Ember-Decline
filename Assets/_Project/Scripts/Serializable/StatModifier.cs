using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType { Health, Attack, Defense, Stamina}
public enum ModifierType { Flat, Percentage}
[System.Serializable]
public class StatModifier
{
    public StatType Type;
    public ModifierType Modifier;
    public float Value;
}
