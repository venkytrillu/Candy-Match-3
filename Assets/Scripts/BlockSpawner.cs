using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public GameObject blockPrefab;
   
    void Start()
    {
       InvokeRepeating("CallBlocks", 0.5f,1);
    }

    void CallBlocks()
    {
        Instantiate(blockPrefab, transform.position, Quaternion.identity);
    }
}//class






























