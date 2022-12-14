using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBattle", menuName = "XXX/EnemyBattle", order = 2)]
public class EnemyBattle : ScriptableObject
{
    public Guid id;
    public string name;
    [ResizableTextArea] public string description;
    //public Sprite image;
    //public int cost;
    public List<Card> enemyCards;
    public List<Vector3> enemyPositions;
}

