using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonGenerationData.asset", menuName = "DungeonGenerationData/Dungeon Data")]
public class DungeonGenerationData : ScriptableObject
{
    public int numberOfCrawlers;
    public int iterationMin;
    public int iterationMax;
    // Data structure that holds room names and the probability of them spawning
    public List<RoomData> roomData;
    
    // class that holds the room name and the probability of it spawning
    [Serializable]
    public class RoomData
    {
        public string roomName;
        public int probability;
        public float probabilityModifier;
        public bool singleRoom;
    }
}
