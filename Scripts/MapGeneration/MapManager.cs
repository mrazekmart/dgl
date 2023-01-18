using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour, InterfaceMapGen
{
    // floor size
    [SerializeField] private int m_floorSizeX = 31;
    [SerializeField] private int m_floorSizeY = 31;

    // blocks prefabs
    [SerializeField] private GameObject m_blockBlack;
    [SerializeField] private GameObject m_blockGround_BossRoom;
    [SerializeField] private GameObject m_blockDoor;

    [SerializeField] private GameObject m_map;
    [SerializeField] private GameObject m_weaponUpgradeMachine;


    private Coord roomSize = new Coord(20, 10);


    // camera thingy
    [SerializeField] private int m_cameraSize = 10;
    [SerializeField] private Camera m_camera;

    // set all rooms active at the beggining
    [SerializeField] private bool m_setAllRoomsActive = false;

    [SerializeField] private int m_numberOfGoldenRooms = 1;
    [SerializeField] private int m_numberOfShopRooms = 1;

    private SpawnEnemiesInRoom m_spawnRandomEnemyScript;
    private MiniMapScript m_miniMapScript;


    // empty game objects holding tiles for every room
    private List<GameObject> m_mapRoomsContainers;

    // main player :P
    private Transform m_mainPlayer;

    // layout of the whole floor - i am keeping this just because of the doors :D
    private RoomGen[,] m_floorLayout;

    // layout of each room
    private RoomTemplate[,] m_roomsLayout;

    // number of rooms per floor
    public int m_numberOfRooms = 10;
    //private int m_numberOfRoomsRange = 2;

    // room coord of middle (starting) room
    private Coord m_middleFloorCoord;

    // current room in which main player is atm
    public RoomTemplate m_currentRoom;


    private void Awake()
    {
        m_spawnRandomEnemyScript = GetComponent<SpawnEnemiesInRoom>();
        m_miniMapScript = GameObject.Find("MiniMap").GetComponent<MiniMapScript>();
        m_mainPlayer = GameObject.Find("MainPlayer").GetComponent<Transform>();
        m_mapRoomsContainers = new List<GameObject>();
        m_camera = Camera.main;

        m_floorLayout = new RoomGen[m_floorSizeX, m_floorSizeY];
        m_roomsLayout = new RoomTemplate[m_floorSizeX, m_floorSizeY];
        m_middleFloorCoord = new Coord(m_floorSizeX / 2, m_floorSizeY / 2);

        GenerateMapLayout();
        GenerateRooms();
        MateralizeRooms();

        // set current room to be a middle room
        m_currentRoom = m_roomsLayout[m_middleFloorCoord.x, m_middleFloorCoord.y];
        // set current room to be active
        m_mapRoomsContainers[m_currentRoom.RoomID].SetActive(true);
        // set camera position to current room
        m_camera.transform.position = new Vector3(m_middleFloorCoord.x * m_cameraSize * 4 + m_cameraSize * 2,
            m_middleFloorCoord.y * m_cameraSize * 2 + m_cameraSize,
            m_camera.transform.position.z);
        // set player to current room
        m_mainPlayer.transform.position = new Vector3(m_middleFloorCoord.x * m_cameraSize * 4 + m_cameraSize * 2,
            m_middleFloorCoord.y * m_cameraSize * 2 + m_cameraSize,
            0);
        // draw minimap
        m_miniMapScript.DrawMinimap(m_currentRoom.RoomWorldPosition, m_floorLayout);
    }

    private void FixedUpdate()
    {
        // room coords of player
        Coord playerPosWorld = new Coord((int)(m_mainPlayer.position.x / (m_cameraSize * 4)),
            (int)(m_mainPlayer.position.y / (m_cameraSize * 2)));

        // if player is entering different room - change current room
        if (playerPosWorld != m_currentRoom.RoomWorldPosition)
        {
            DespawnEnemiesInNewRoom();
            ChangeCurrentRoom(playerPosWorld);
            SpawnEnemiesInNewRoom();
        }
    }

    public void OnEnemyKilled()
    {
        m_currentRoom.OnEnemyKilled();
    }
    private void DespawnEnemiesInNewRoom()
    {
        if (m_currentRoom.EnemiesInRoom != null && !m_currentRoom.IsCleared)
        {
            m_spawnRandomEnemyScript.DespawnEnemies(m_currentRoom.EnemiesInRoomGameObjects);
        }
    }
    private void SpawnEnemiesInNewRoom()
    {
        if (m_currentRoom.EnemiesInRoom != null && !m_currentRoom.IsCleared)
        {
            m_spawnRandomEnemyScript.SpawnEnemies(m_currentRoom);
        }
    }
    /*
     *  Changes current room, setting previous room not active and new room active, centering camera to new room
     *  in: Coord pos - room coords of new room
     * 
     */
    private void ChangeCurrentRoom(Coord pos)
    {
        if (!m_setAllRoomsActive) m_mapRoomsContainers[m_currentRoom.RoomID].SetActive(false);

        Coord diff = new Coord(pos.x - m_currentRoom.RoomWorldPosition.x,
            pos.y - m_currentRoom.RoomWorldPosition.y);

        m_currentRoom = m_roomsLayout[pos.x, pos.y];

        Vector3 newCameraPosition = new Vector3(m_camera.transform.position.x + diff.x * m_cameraSize * 4,
            m_camera.transform.position.y + diff.y * 20, m_camera.transform.position.z);
        m_camera.transform.position = newCameraPosition;


        m_mapRoomsContainers[m_currentRoom.RoomID].SetActive(true);
        m_miniMapScript.DrawMinimap(m_currentRoom.RoomWorldPosition, m_floorLayout);


    }
    /*
     * Takes floor layout and already generated rooms in it and places blocks appropriately + sorting them in containers
     * 
     */
    private void MateralizeRooms()
    {
        for (int i = 0; i < m_floorSizeX; i++)
        {
            for (int j = 0; j < m_floorSizeY; j++)
            {
                if (m_roomsLayout[i, j] != null)
                {
                    RoomTemplate currRoom = m_roomsLayout[i, j];
                    Transform currRoomParent = m_mapRoomsContainers[currRoom.RoomID].GetComponent<Transform>();
                    MateralizeRoom(i, j, currRoom, currRoomParent);
                }
            }
        }
    }
    /*
     * Places blocks in Room
     * 
     */
    private void MateralizeRoom(int i, int j, RoomTemplate currRoom, Transform currRoomParent)
    {
        Block[,] currRoomLayout = currRoom.RoomLayout;
        Dictionary<Block, Vector3> specialBlocks = new Dictionary<Block, Vector3>();

        Vector3 roomOffSet = new Vector3((i * currRoom.RoomSize.x),
            (j * currRoom.RoomSize.y),
            0);

        for (int k = 0; k < currRoomLayout.GetLength(0); k++)
        {
            for (int l = 0; l < currRoomLayout.GetLength(1); l++)
            {
                int blockType = currRoomLayout[k, l].BlockType;
                Vector3 blockPostion = new Vector3(k * (currRoom.BlockSize) + currRoom.BlockSize / 2,
                    l * (currRoom.BlockSize) + currRoom.BlockSize / 2,
                    0);

                GameObject blockGameObject;

                switch (blockType)
                {
                    case InterfaceMapGen.BLOCK_TYPE_WALL:
                    case InterfaceMapGen.BLOCK_TYPE_HOLE:
                        blockGameObject = BlockHandlerSingleton.Instance.GetRandomGroundStoneBlock();
                        CreateBlock(roomOffSet + blockPostion, blockGameObject, currRoomParent);
                        break;
                    case InterfaceMapGen.BLOCK_TYPE_GROUND:
                    case InterfaceMapGen.BLOCK_TYPE_GROUND_GOLDENROOM:
                    case InterfaceMapGen.BLOCK_TYPE_GROUND_SHOPROOM:
                    case InterfaceMapGen.BLOCK_TYPE_GROUND_BOSSROOM:
                        blockGameObject = BlockHandlerSingleton.Instance.GetRandomGroundBlock();
                        CreateBlock(roomOffSet + blockPostion, blockGameObject, currRoomParent);
                        break;
                    case InterfaceMapGen.BLOCK_TYPE_DOORS:
                        CreateBlock(roomOffSet + blockPostion, m_blockDoor, currRoomParent);
                        break;
                    case InterfaceMapGen.BLOCK_TYPE_GROUND_SHOPROOM_UPGRADEWEAPONMACHINE:
                    case InterfaceMapGen.BLOCK_TYPE_GROUND_SPAWNITEM:
                        blockGameObject = BlockHandlerSingleton.Instance.GetRandomGroundBlock();
                        CreateBlock(roomOffSet + blockPostion, blockGameObject, currRoomParent);
                        specialBlocks.Add(currRoomLayout[k, l], blockPostion);
                        break;
                    default:
                        break;
                }
            }
        }

        //handle special blocks/items
        foreach(KeyValuePair<Block, Vector3> kvp in specialBlocks)
        {
            int blockType = kvp.Key.BlockType;
            switch (blockType)
            {
                case InterfaceMapGen.BLOCK_TYPE_GROUND_SPAWNITEM:
                    SpawnRandomItem(roomOffSet + kvp.Value);
                    break;
                case InterfaceMapGen.BLOCK_TYPE_GROUND_SHOPROOM_UPGRADEWEAPONMACHINE:
                    Instantiate(m_weaponUpgradeMachine, roomOffSet + kvp.Value, Quaternion.identity, currRoomParent);
                    break;
                default:
                    break;
            }
        }
    }    
    /*
     * Spawns random item on given position
     * 
     */
    private void SpawnRandomItem(Vector3 position)
    {
        //picking random item
        int itemsCount = InventoryHandlerSingleton.Instance.m_itemList.m_itemList.Length;
        int randomItemIndex = Random.Range(0, itemsCount);
        string itemName = InterfaceItems.ITEMS_NAME_FOLDER_PREFIX + InterfaceItems.ITEMS_NAME_PREFIX + randomItemIndex;

        //instantiating new item and setting it's position
        GameObject randomItem = (GameObject)Instantiate(Resources.Load(itemName));
        randomItem.transform.position = position;
    }
    /*
     * GenerateRooms from floor layout
     * 
     */
    private void GenerateRooms()
    {
        for (int i = 0; i < m_floorSizeX; i++)
        {
            for (int j = 0; j < m_floorSizeY; j++)
            {
                if (m_floorLayout[i, j] != null)
                {
                    Coord roomWorldPosition = new Coord(i, j);

                    bool[] roomDoors = HandleDoors(i, j);
                    int roomType = m_floorLayout[i, j].roomType;
                    int roomID = m_floorLayout[i, j].roomId;

                    RoomTemplate newRoom = InstantiateRoom(roomType, roomID, roomWorldPosition, roomDoors);

                    string newRoomContainerName = InterfaceMapGen.ROOM_CONTAINER_NAME + roomID;
                    GameObject newRoomContainerGO = new GameObject(newRoomContainerName);
                    newRoomContainerGO.transform.parent = m_map.transform;
                    newRoomContainerGO.SetActive(m_setAllRoomsActive);
                    m_mapRoomsContainers.Add(newRoomContainerGO);

                    m_roomsLayout[i, j] = newRoom;
                }
            }
        }
    }

    private RoomTemplate InstantiateRoom(int roomType, int roomID, Coord roomWorldPosition, bool[] roomDoors)
    {
        RoomTemplate newRoom = null;
        switch (roomType)
        {
            case InterfaceMapGen.ROOM_TYPE_STARTINGROOM:
                newRoom = new StartingRoom(roomID, roomWorldPosition, m_cameraSize, roomDoors);
                break;
            case InterfaceMapGen.ROOM_TYPE_NORMALROOM:
                newRoom = new StandardRoom(roomID, roomWorldPosition, m_cameraSize, roomDoors, Random.Range(1, 10));
                break;
            case InterfaceMapGen.ROOM_TYPE_GOLDENROOM:
                newRoom = new GoldenRoom(roomID, roomWorldPosition, m_cameraSize, roomDoors);
                break;
            case InterfaceMapGen.ROOM_TYPE_SHOP:
                newRoom = new ShopRoom(roomID, roomWorldPosition, m_cameraSize, roomDoors);
                break;
            case InterfaceMapGen.ROOM_TYPE_BOSSROOM:
                newRoom = new BossRoom(roomID, roomWorldPosition, m_cameraSize, roomDoors, 1);
                break;
            default:
                newRoom = new StandardRoom(roomID, roomWorldPosition, m_cameraSize, roomDoors, Random.Range(1, 10));
                break;
        }
        return newRoom;
    }
    /*
     * Check if current room is connected to other room and generate doors accordingly
     * 
     */
    private bool[] HandleDoors(int x, int y)
    {
        bool[] result = new bool[4];

        if (x == 0) result[0] = false;
        else if (m_floorLayout[x - 1, y] != null) result[0] = true;
        else result[0] = false;

        if (y == 0) result[1] = false;
        else if (m_floorLayout[x, y - 1] != null) result[1] = true;
        else result[1] = false;

        if (x == m_floorSizeX - 1) result[2] = false;
        else if (m_floorLayout[x + 1, y] != null) result[2] = true;
        else result[2] = false;

        if (y == m_floorSizeY - 1) result[3] = false;
        else if (m_floorLayout[x, y + 1] != null) result[3] = true;
        else result[3] = false;

        return result;
    }

    private void GenerateMapLayout()
    {
        List<Coord> availableRooms = new List<Coord>();
        List<Coord> closedRooms = new List<Coord>();

        Coord startingRoom = new Coord(m_floorSizeX / 2, m_floorSizeY / 2);


        m_floorLayout[startingRoom.x, startingRoom.y] = new RoomGen(InterfaceMapGen.ROOM_TYPE_STARTINGROOM, 0);
        closedRooms.Add(startingRoom);

        availableRooms.Add(new Coord(startingRoom.x + 1, startingRoom.y));
        availableRooms.Add(new Coord(startingRoom.x - 1, startingRoom.y));
        availableRooms.Add(new Coord(startingRoom.x, startingRoom.y + 1));
        availableRooms.Add(new Coord(startingRoom.x, startingRoom.y - 1));


        //int numberOfRooms = (int)Random.Range(m_numberOfRooms - m_numberOfRoomsRange, m_numberOfRooms + m_numberOfRoomsRange) - 1;
        int numberOfRooms = m_numberOfRooms;
        List<int> alreadyTakenIndices = new List<int>();
        alreadyTakenIndices.Add(0);
        alreadyTakenIndices.Add(numberOfRooms - 1);


        //generating position of golden rooms
        int numberOfGoldenRooms = m_numberOfGoldenRooms;
        List<int> goldenRoomsIndices = new List<int>(numberOfGoldenRooms);
        while (numberOfGoldenRooms > 0)
        {
            int newRoomIndice = Random.Range(0, numberOfRooms);
            while (alreadyTakenIndices.Contains(newRoomIndice))
            {
                newRoomIndice++;
                if (newRoomIndice == numberOfRooms)
                {
                    newRoomIndice = 0;
                }
            }
            goldenRoomsIndices.Add(newRoomIndice);
            alreadyTakenIndices.Add(newRoomIndice);
            numberOfGoldenRooms--;
        }

        //generating position of shop rooms
        int numberOfShopRooms = m_numberOfShopRooms;
        List<int> shopRoomsIndices = new List<int>(numberOfShopRooms);
        while (numberOfShopRooms > 0)
        {
            int newRoomIndice = Random.Range(0, numberOfRooms);
            while (alreadyTakenIndices.Contains(newRoomIndice))
            {
                newRoomIndice++;
                if (newRoomIndice == numberOfRooms)
                {
                    newRoomIndice = 0;
                }
            }
            shopRoomsIndices.Add(newRoomIndice);
            alreadyTakenIndices.Add(newRoomIndice);
            numberOfShopRooms--;
        }

        //leaving out starting room
        for (int roomIndex = 1; roomIndex < numberOfRooms; roomIndex++)
        {

            int avalableRoomsListLength = availableRooms.Count;
            int nextRoomI = Random.Range(0, avalableRoomsListLength);
            Coord nextRoom = availableRooms[nextRoomI];


            if (goldenRoomsIndices.Contains(roomIndex))
            {
                m_floorLayout[nextRoom.x, nextRoom.y] = new RoomGen(InterfaceMapGen.ROOM_TYPE_GOLDENROOM, roomIndex);
            }
            else if (shopRoomsIndices.Contains(roomIndex))
            {
                m_floorLayout[nextRoom.x, nextRoom.y] = new RoomGen(InterfaceMapGen.ROOM_TYPE_SHOP, roomIndex);
            }
            else if (roomIndex == numberOfRooms - 1)
            {
                m_floorLayout[nextRoom.x, nextRoom.y] = new RoomGen(InterfaceMapGen.ROOM_TYPE_BOSSROOM, roomIndex);
                break;
            }
            else
            {
                m_floorLayout[nextRoom.x, nextRoom.y] = new RoomGen(InterfaceMapGen.ROOM_TYPE_NORMALROOM, roomIndex);
            }


            availableRooms.RemoveAt(nextRoomI);

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;
                    if (Mathf.Abs(i) == Mathf.Abs(j)) continue;
                    if (nextRoom.x + i < 0 || nextRoom.x + i > m_floorSizeX - 1) continue;
                    if (nextRoom.y + j < 0 || nextRoom.y + j > m_floorSizeY - 1) continue;

                    Coord nextAvailableRoom = new Coord(nextRoom.x + i, nextRoom.y + j);

                    bool addAvailableRoom = true;

                    foreach (Coord room in closedRooms)
                    {
                        if (room.x == nextAvailableRoom.x && room.y == nextAvailableRoom.y)
                        {
                            addAvailableRoom = false;
                            break;
                        }
                    }
                    if (addAvailableRoom)
                    {
                        foreach (Coord room in availableRooms)
                        {
                            if (room.x == nextAvailableRoom.x && room.y == nextAvailableRoom.y)
                            {
                                addAvailableRoom = false;
                                break;
                            }
                        }
                    }
                    if (addAvailableRoom) availableRooms.Add(nextAvailableRoom);
                }
            }
            closedRooms.Add(nextRoom);
        }

    }
    private void CreateBlock(Vector3 position, GameObject block, Transform parent)
    {
        GameObject wall = Instantiate(block, position, Quaternion.identity, parent);
        wall.SetActive(true);
    }
}
