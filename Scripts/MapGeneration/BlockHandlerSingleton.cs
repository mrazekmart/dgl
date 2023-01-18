using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHandlerSingleton : MonoBehaviour
{
    private List<GameObject> m_groundBlocks = new List<GameObject>();
    private List<GameObject> m_groundStoneBlocks = new List<GameObject>();
    public static BlockHandlerSingleton Instance
    {
        get; private set;
    }
    public List<GameObject> GroundBlocks { get => m_groundBlocks; set => m_groundBlocks = value; }
    public List<GameObject> GroundStoneBlocks { get => m_groundStoneBlocks; set => m_groundStoneBlocks = value; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        LoadGroundObjects();
    }
    private void LoadGroundObjects()
    {
        int i = 1;
        GameObject groundBlock = null;
        while (true)
        {
            string groundBlockName = InterfaceItems.BLOCK_GROUND_FOLDER_PREFIX + InterfaceItems.BLOCK_GROUND_NAME_PREFIX + i;
            i++;
            GameObject go = (GameObject)Resources.Load(groundBlockName);
            if (go == null) break;
            groundBlock = Instantiate(go);
            GroundBlocks.Add(groundBlock);
        }
        i = 1;
        while (true)
        {
            string groundBlockName = InterfaceItems.BLOCK_GROUND_FOLDER_PREFIX + InterfaceItems.BLOCK_GROUND_STONE_NAME_PREFIX + i;
            i++;
            GameObject go = (GameObject)Resources.Load(groundBlockName);
            if (go == null) break;
            groundBlock = Instantiate(go);
            GroundStoneBlocks.Add(groundBlock);
        }
    }

    public GameObject GetRandomGroundBlock()
    {
        int randomGroundBlock = Random.Range(0, GroundBlocks.Count);
        //making better odds for 0
        if(randomGroundBlock == 2 || randomGroundBlock == 3)
            if(Random.Range(0, 20) != 0) randomGroundBlock = 0;


        return GroundBlocks[randomGroundBlock];
    }
    public GameObject GetRandomGroundStoneBlock()
    {
        int randomGroundBlock = Random.Range(0, GroundStoneBlocks.Count);
        return GroundStoneBlocks[randomGroundBlock];
    }
}
