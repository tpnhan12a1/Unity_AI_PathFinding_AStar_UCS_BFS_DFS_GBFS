using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public MapLocation _location;
    public Node _parent;

    public GameObject _nodeObject;

    public Node(MapLocation location, GameObject nodeObject, Node parent)
    {
        this._location = location;
        this._nodeObject = nodeObject;
        this._parent = parent;
    }
    public override bool Equals(object obj)
    {
        if (obj == null || !this.GetType().Equals(obj.GetType()))
           return false;
        return _location.Equals(((Node)obj)._location);
    }
    public override int GetHashCode()
    {
        return 0;
    }
    public void SetNodeObject(GameObject nodeObject)
    {
        this._nodeObject = nodeObject;
    }
    public void SetParent(Node parent)
    {
        this._parent = parent;
    }

}
