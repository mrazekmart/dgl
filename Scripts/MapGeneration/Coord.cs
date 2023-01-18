using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coord
{
    public int x;
    public int y;

    public Coord()
    {

    }
    public Coord(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
    public Coord(Vector2 _pos)
    {
        x = (int)_pos.x;
        y = (int)_pos.y;
    }
    public Coord(Vector3 _pos)
    {
        x = (int)_pos.x;
        y = (int)_pos.y;
    }

    public Vector2 GetV2()
    {
        return new Vector2(x, y);
    }
    public Vector3 GetV3()
    {
        return new Vector3(x, 10, y);
    }

    public override string ToString()
    {
        string result = x + " " + y;
        return result;
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public static bool operator ==(Coord c1, Coord c2)
    {
        return (c1.x == c2.x && c1.y == c2.y);
    }

    public static bool operator !=(Coord c1, Coord c2)
    {
        return !(c1.x == c2.x && c1.y == c2.y);
    }
}
