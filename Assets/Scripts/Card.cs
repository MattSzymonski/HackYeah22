using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "XXX/Card", order = 2)]
public class Card : ScriptableObject
{
    public Guid id;
    public string name;
    [ResizableTextArea] public string description;
    public Sprite image;
    public int cost;
}

