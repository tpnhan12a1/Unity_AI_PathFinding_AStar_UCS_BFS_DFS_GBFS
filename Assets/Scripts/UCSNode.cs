using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UCSNode : Node
{
    public float cost;
    public UCSNode(MapLocation location,float cost, GameObject nodeObject, Node parent) : base(location,nodeObject, parent)
    {
        this.cost = cost;
    }
    public override bool Equals(object obj)
    {
        if (obj == null || !this.GetType().Equals(obj.GetType()))
            return false;
        return _location.Equals(((UCSNode)obj)._location);
    }
    public override int GetHashCode()
    {
        return 0;
    }
}
