using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLocation 
{
    public int x;
    public int z;

    public MapLocation(int x,int y)
    {
        this.x = x;
        this.z = y;
    }
    public Vector3 ConvertToVector3()
    {
        return new Vector3(x,0,z);
    }
    public  Vector3 ConvertToVector3(MapLocation location)
    {
        return new Vector3(location.x, 0, location.z);
    }

    public static MapLocation operator *(MapLocation a, int b)=> new MapLocation(a.x*b, a.z*b);
    public static MapLocation operator +(MapLocation a, MapLocation b) => new MapLocation(a.x + b.x, a.z + b.z);
    public float Distance(Vector3 loc1,Vector3 loc2)
    {
        return Vector3.Distance(loc1, loc2);
    }
    public override bool Equals(object obj)
    {
        if (obj == null || !this.GetType().Equals(obj.GetType()))
            return false;
        return x == ((MapLocation) obj).x &&  z == ((MapLocation) obj).z;
    }
    public override int GetHashCode()
    {
        return 0;
    }
}
