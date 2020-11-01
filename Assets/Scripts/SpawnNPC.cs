using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNPC : MonoBehaviour
{
    public int npcCount;
    private List<NPC> npcList = new List<NPC>();
    
    void Start()
    {
        for (int i = 0; i < npcCount; i++)
        {
            npcList.Add(new NPC());
        }
    }
}
