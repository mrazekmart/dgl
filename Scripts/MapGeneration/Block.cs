using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    private int blockType = 0;
    private bool walkable = false;

    public int BlockType { get => blockType; set => blockType = value; }
    public bool Walkable { get => walkable; set => walkable = value; }

    public Block(int _blockType, bool _walkable = false)
    {
        BlockType = _blockType;
        Walkable = _walkable;
    }
}
