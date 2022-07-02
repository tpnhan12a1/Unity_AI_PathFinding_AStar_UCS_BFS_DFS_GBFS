using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode : Node
{
    private float _G;
    private float _H;
    private float _F;

    public float G { get { return _G; } }
    public float H { get { return _H; } }
    public float F { get { return _F; } }

    public AStarNode(MapLocation location, float G, float H, float F, GameObject nodeObject, Node parent):
        base(location, nodeObject, parent)
    {
        this._location = location;
        this._G = G;
        this._H = H;
        this._F = F;
        this._nodeObject = nodeObject;
        this._parent = parent;
    }
    public override bool Equals(object obj)
    {
        if (obj == null || !this.GetType().Equals(obj.GetType()))
            return false;
        return _location.Equals(((AStarNode)obj)._location);
    }
    public override int GetHashCode()
    {
        return 0;
    }
}
