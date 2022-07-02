using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GBFSNode : Node
{
    public float distanceToGoal;
    public GBFSNode(MapLocation location, float distanceToGoal, GameObject nodeObject, Node parent) : base(location, nodeObject, parent)
    {
        this.distanceToGoal = distanceToGoal;
    }
    public override bool Equals(object obj)
    {
        if (obj == null || !this.GetType().Equals(obj.GetType()))
            return false;
        return _location.Equals(((GBFSNode)obj)._location);
    }
    public override int GetHashCode()
    {
        return 0;
    }
}
