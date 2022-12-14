using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Place", menuName = "XXX/Place", order = 2)]
public class Place : ScriptableObject
{
    public Guid id;
    public string name;
    public int year;
    [ResizableTextArea] public string description;
    public Sprite image;
}
