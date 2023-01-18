using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapScript : MonoBehaviour, InterfaceMapGen
{
    private RawImage m_rawImage;
    private Texture2D m_texture;
    private int m_minimapRoomSize = 40;

    private void Awake()
    {
        m_rawImage = GetComponent<RawImage>();
        m_texture = new Texture2D(200, 200, TextureFormat.ARGB32, false);
    }
    public void DrawMinimap(Coord currentRoom, RoomGen[,] floorLayout)
    {
        int iOff = 0;
        int jOff = 0;
        for (int i = currentRoom.x - 2; i <= currentRoom.x + 2; i++)
        {
            for (int j = currentRoom.y - 2; j <= currentRoom.y + 2; j++)
            {
                if (floorLayout[i, j] == null)
                {
                    DrawRoomInTexture(iOff * m_minimapRoomSize, jOff * m_minimapRoomSize);
                }
                else
                {
                    bool isCurrentRoom = false;
                    if (i == currentRoom.x && j == currentRoom.y) isCurrentRoom = true;
                    DrawRoomInTexture(iOff * m_minimapRoomSize, jOff * m_minimapRoomSize, floorLayout[i, j].roomType, isCurrentRoom);
                }
                jOff++;
            }
            iOff++;
        }

        m_texture.Apply();
        m_rawImage.texture = m_texture;
    }
    private void DrawRoomInTexture(int xOff, int yOff, int roomType, bool isCurrentRoom = false)
    {
        for (int i = 0; i < m_minimapRoomSize; i++)
        {
            for (int j = 0; j < m_minimapRoomSize; j++)
            {

                if (i == 0 || i == m_minimapRoomSize - 1 || j == 0 || j == m_minimapRoomSize - 1)
                {
                    if (isCurrentRoom) m_texture.SetPixel(i + xOff, j + yOff, Color.red);
                    else m_texture.SetPixel(i + xOff, j + yOff, Color.black);
                    continue;
                }

                switch (roomType)
                {
                    case InterfaceMapGen.ROOM_TYPE_NORMALROOM:
                    case InterfaceMapGen.ROOM_TYPE_STARTINGROOM:
                        m_texture.SetPixel(i + xOff, j + yOff, Color.green);
                        break;
                    case InterfaceMapGen.ROOM_TYPE_BOSSROOM:
                        m_texture.SetPixel(i + xOff, j + yOff, Color.magenta);
                        break;
                    case InterfaceMapGen.ROOM_TYPE_SHOP:
                        m_texture.SetPixel(i + xOff, j + yOff, Color.gray);
                        break;
                    case InterfaceMapGen.ROOM_TYPE_GOLDENROOM:
                        m_texture.SetPixel(i + xOff, j + yOff, Color.yellow);
                        break;
                }
            }
        }
    }
    private void DrawRoomInTexture(int xOff, int yOff)
    {
        for (int i = 0; i < m_minimapRoomSize; i++)
        {
            for (int j = 0; j < m_minimapRoomSize; j++)
            {
                Color color = new Color(1, 1, 1, 0);
                m_texture.SetPixel(i + xOff, j + yOff, color);
            }
        }
    }
}
