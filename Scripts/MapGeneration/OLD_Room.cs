using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : InterfaceMapGen
{
    private const int M_DOORSIZE = 5;


    public float m_perlinNoise = .45f;
    public float m_noiseScale = 0.0001f;


    private float m_blockSize = 0.5f;
    private int m_passageSize = 3;

    private Coord m_roomSize;

    private int m_cameraSize;
    private int m_roomID;
    private int m_roomType;
    private Coord m_roomWorldPosition;
    private Block[,] m_roomLayout;
    private List<Coord> m_walkableTiles;

    // 4-array, 0 - left door, 1-bottom door, 2-right door, 3-topdoor
    private bool[] m_doors;

    public float BlockSize { get => m_blockSize; set => m_blockSize = value; }
    public Coord RoomSize { get => m_roomSize; set => m_roomSize = value; }
    public Coord RoomWorldPosition { get => m_roomWorldPosition; set => m_roomWorldPosition = value; }
    public int RoomID { get => m_roomID; set => m_roomID = value; }
    public List<Coord> WalkableTiles { get => m_walkableTiles; set => m_walkableTiles = value; }
    public Block[,] RoomLayout { get => m_roomLayout; set => m_roomLayout = value; }

    public Room(int _id, int[] _roomWorldPosition, int _cameraSize, bool[] _doors, int _roomType)
    {

        RoomID = _id;
        RoomWorldPosition = new Coord(_roomWorldPosition[0], _roomWorldPosition[1]);
        WalkableTiles = new List<Coord>();
        m_cameraSize = _cameraSize;
        m_doors = _doors;
        m_roomType = _roomType;

        RoomSize = new Coord((int)(4 * m_cameraSize * (1 / BlockSize)), (int)(2 * m_cameraSize * (1 / BlockSize)));
        RoomLayout = new Block[RoomSize.x, RoomSize.y];

        GenerateRoom();
        GenerateDoors();
        DivideRegions();
    }

    private void DivideRegions()
    {

        int[,] mapCopy = new int[RoomSize.x, RoomSize.y];
        List<RoomIsland> finalIslands = new List<RoomIsland>();

        for(int i = 0; i < RoomSize.x; i++)
        {
            for(int j = 0; j < RoomSize.y; j++)
            {
                if (RoomLayout[i, j].Walkable && mapCopy[i, j] == 0)
                {
                    List<Coord> tiles = new List<Coord>();
                    List<Coord> edgeTiles = new List<Coord>();
                    Queue<Coord> queueCoords = new Queue<Coord>();
                    queueCoords.Enqueue(new Coord(i, j));
                    mapCopy[i, j] = 1;

                    while (queueCoords.Count > 0)
                    {
                        bool isCorner = false;
                        Coord tile = queueCoords.Dequeue();
                        tiles.Add(tile);

                        if (RoomLayout[tile.x + 1, tile.y].Walkable &&
                            mapCopy[tile.x + 1, tile.y] == 0)
                        {
                            queueCoords.Enqueue(new Coord(tile.x + 1, tile.y));
                            mapCopy[tile.x + 1, tile.y] = 1;
                        }
                        else
                        {
                            isCorner = true;
                        }
                        if (RoomLayout[tile.x - 1, tile.y].Walkable &&
                            mapCopy[tile.x - 1, tile.y] == 0)
                        {
                            queueCoords.Enqueue(new Coord(tile.x - 1, tile.y));
                            mapCopy[tile.x - 1, tile.y] = 1;
                        }
                        else
                        {
                            isCorner = true;
                        }
                        if (RoomLayout[tile.x, tile.y + 1].Walkable &&
                            mapCopy[tile.x, tile.y + 1] == 0)
                        {
                            queueCoords.Enqueue(new Coord(tile.x, tile.y + 1));
                            mapCopy[tile.x, tile.y + 1] = 1;
                        }
                        else
                        {
                            isCorner = true;
                        }
                        if (RoomLayout[tile.x, tile.y - 1].Walkable &&
                            mapCopy[tile.x, tile.y - 1] == 0)
                        {
                            queueCoords.Enqueue(new Coord(tile.x, tile.y - 1));
                            mapCopy[tile.x, tile.y - 1] = 1;
                        }
                        else
                        {
                            isCorner = true;
                        }

                        if (isCorner) edgeTiles.Add(tile);

                    }

                    //Delete small rooms
                    if(tiles.Count < 0)
                    {
                        foreach(Coord tile in tiles)
                        {
                            RoomLayout[tile.x, tile.y] = new Block(InterfaceMapGen.BLOCK_TYPE_HOLE, false);
                        }
                    }
                    else
                    {
                        finalIslands.Add(new RoomIsland(tiles, edgeTiles));
                    }

                }
            }
        }

        finalIslands[0].isMainRoom = true;
        finalIslands[0].isAccessibleFromMainRoom = true;
        ConnectClosestRooms(finalIslands);

    }

    private void ConnectClosestRooms(List<RoomIsland> finalRooms, bool forceAccessibilityFromMainRoom = false)
    {
        List<RoomIsland> roomListA = new List<RoomIsland>();
        List<RoomIsland> roomListB = new List<RoomIsland>();

        if (forceAccessibilityFromMainRoom)
        {
            foreach (RoomIsland room in finalRooms)
            {
                if (room.isAccessibleFromMainRoom)
                {
                    roomListB.Add(room);
                }
                else
                {
                    roomListA.Add(room);
                }
            }
        }
        else
        {
            roomListA = finalRooms;
            roomListB = finalRooms;

        }

        int bestDistance = int.MaxValue;
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        RoomIsland bestRoomA = new RoomIsland();
        RoomIsland bestRoomB = new RoomIsland();
        bool possibleConnectionFound = false;

        foreach (RoomIsland roomA in roomListA)
        {
            if (!forceAccessibilityFromMainRoom)
            {
                possibleConnectionFound = false;
                if (roomA.connectedRooms.Count > 0)
                {
                    continue;
                }
            }
            foreach (RoomIsland roomB in roomListB)
            {
                if (roomB == roomA || roomA.IsConnected(roomB))
                {
                    continue;
                }

                for (int i = 0; i < roomA.edgeTiles.Count; i++)
                {
                    for (int j = 0; j < roomB.edgeTiles.Count; j++)
                    {
                        Coord tileA = roomA.edgeTiles[i];
                        Coord tileB = roomB.edgeTiles[j];

                        int distaneAB = (int)(Mathf.Pow(tileA.x - tileB.x, 2) + Mathf.Pow(tileA.y - tileB.y, 2));
                        if (distaneAB < bestDistance || !possibleConnectionFound)
                        {
                            bestDistance = distaneAB;
                            possibleConnectionFound = true;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                            bestTileA = tileA;
                            bestTileB = tileB;
                        }
                    }
                }
            }
            if (possibleConnectionFound && !forceAccessibilityFromMainRoom)
            {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }
        if (possibleConnectionFound && forceAccessibilityFromMainRoom)
        {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(finalRooms, true);
        }

        if (!forceAccessibilityFromMainRoom)
        {
            ConnectClosestRooms(finalRooms, true);
        }

    }

    private void CreatePassage(RoomIsland roomA, RoomIsland roomB, Coord tileA, Coord tileB)
    {
        RoomIsland.ConnectRooms(roomA, roomB);
        List<Coord> line = GetLine(tileA, tileB);
        foreach (Coord c in line)
        {
            DrawCircle(c, m_passageSize);
        }
    }
    void DrawCircle(Coord c, int r)
    {
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int drawX = c.x + x;
                    int drawY = c.y + y;
                    if (IsInMapRange(drawX, drawY))
                    {
                        RoomLayout[drawX, drawY] = new Block(InterfaceMapGen.BLOCK_TYPE_GROUND, true);
                        WalkableTiles.Add(new Coord(drawX, drawY));
                    }
                }
            }
        }
    }
    List<Coord> GetLine(Coord from, Coord to)
    {
        List<Coord> line = new List<Coord>();

        int x = from.x;
        int y = from.y;

        int dx = to.x - from.x;
        int dy = to.y - from.y;

        bool inverted = false;
        int step = (int)Mathf.Sign(dx);
        int gradientStep = (int)Mathf.Sign(dy);

        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if (longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            step = (int)Mathf.Sign(dy);
            gradientStep = (int)Mathf.Sign(dx);
        }

        int gradientAccumulation = longest / 2;
        for (int i = 0; i < longest; i++)
        {
            line.Add(new Coord(x, y));

            if (inverted)
            {
                y += step;
            }
            else
            {
                x += step;
            }

            gradientAccumulation += shortest;
            if (gradientAccumulation >= longest)
            {
                if (inverted)
                {
                    x += gradientStep;
                }
                else
                {
                    y += gradientStep;
                }
                gradientAccumulation -= longest;
            }
        }

        return line;
    }
    bool IsInMapRange(int x, int y)
    {
        return x >= 1 && x < RoomSize.x - 1 && y >= 1 && y < RoomSize.y - 1;
    }
    private void GenerateRoom()
    {
        float randomX = Random.Range(0, 1000);
        float randomY = Random.Range(0, 1000);

        for (int i = 0; i < RoomSize.x; i++)
        {
            for (int j = 0; j < RoomSize.y; j++)
            {
                if (i == 0 || i == RoomSize.x - 1 || j == 0 || j == RoomSize.y - 1)
                {
                    RoomLayout[i, j] = new Block(InterfaceMapGen.BLOCK_TYPE_WALL);
                    continue;
                }

                if(m_roomType == InterfaceMapGen.ROOM_TYPE_STARTINGROOM) {
                    RoomLayout[i, j] = new Block(InterfaceMapGen.BLOCK_TYPE_GROUND, true);
                    continue;
                }
                if(m_roomType == InterfaceMapGen.ROOM_TYPE_GOLDENROOM)
                {
                    RoomLayout[i, j] = new Block(InterfaceMapGen.BLOCK_TYPE_GROUND_GOLDENROOM, true);
                    continue;
                }
                if (m_roomType == InterfaceMapGen.ROOM_TYPE_SHOP)
                {
                    RoomLayout[i, j] = new Block(InterfaceMapGen.BLOCK_TYPE_GROUND_SHOPROOM, true);
                    continue;
                }
                if (m_roomType == InterfaceMapGen.ROOM_TYPE_BOSSROOM)
                {
                    RoomLayout[i, j] = new Block(InterfaceMapGen.BLOCK_TYPE_GROUND_BOSSROOM, true);
                    continue;
                }

                float p = Mathf.PerlinNoise((i * randomX) * m_noiseScale, (j * randomY) * m_noiseScale);

                if (p < m_perlinNoise)
                {
                    RoomLayout[i, j] = new Block(InterfaceMapGen.BLOCK_TYPE_HOLE);
                    continue;
                }
                RoomLayout[i, j] = new Block(InterfaceMapGen.BLOCK_TYPE_GROUND, true);
                WalkableTiles.Add(new Coord(i, j));
            }
        }
    }
    public void GenerateDoors()
    {
        if (m_doors[0])
        {
            for (int i = -M_DOORSIZE / 2; i <= M_DOORSIZE / 2; i++)
            {
                RoomLayout[0, RoomSize.y / 2 + i] = new Block(InterfaceMapGen.BLOCK_TYPE_DOORS);
                RoomLayout[1, RoomSize.y / 2 + i] = new Block(InterfaceMapGen.BLOCK_TYPE_GROUND, true);
                RoomLayout[2, RoomSize.y / 2 + i] = new Block(InterfaceMapGen.BLOCK_TYPE_GROUND, true);
            }
        }
        if (m_doors[1])
        {
            for (int i = -M_DOORSIZE / 2; i <= M_DOORSIZE / 2; i++)
            {
                RoomLayout[RoomSize.x / 2 + i, 0] = new Block(InterfaceMapGen.BLOCK_TYPE_DOORS);
                RoomLayout[RoomSize.x / 2 + i, 1] = new Block(InterfaceMapGen.BLOCK_TYPE_GROUND, true);
                RoomLayout[RoomSize.x / 2 + i, 2] = new Block(InterfaceMapGen.BLOCK_TYPE_GROUND, true);
            }
        }
        if (m_doors[2])
        {
            for (int i = -M_DOORSIZE / 2; i <= M_DOORSIZE / 2; i++)
            {
                RoomLayout[RoomSize.x - 1, RoomSize.y / 2 + i] = new Block(InterfaceMapGen.BLOCK_TYPE_DOORS);
                RoomLayout[RoomSize.x - 2, RoomSize.y / 2 + i] = new Block(InterfaceMapGen.BLOCK_TYPE_GROUND, true);
                RoomLayout[RoomSize.x - 3, RoomSize.y / 2 + i] = new Block(InterfaceMapGen.BLOCK_TYPE_GROUND, true);
            }
        }
        if (m_doors[3])
        {
            for (int i = -M_DOORSIZE / 2; i <= M_DOORSIZE / 2; i++)
            {
                RoomLayout[RoomSize.x / 2 + i, RoomSize.y - 1] = new Block(InterfaceMapGen.BLOCK_TYPE_DOORS);
                RoomLayout[RoomSize.x / 2 + i, RoomSize.y - 2] = new Block(InterfaceMapGen.BLOCK_TYPE_GROUND, true);
                RoomLayout[RoomSize.x / 2 + i, RoomSize.y - 3] = new Block(InterfaceMapGen.BLOCK_TYPE_GROUND, true);

            }
        }
    }
}
