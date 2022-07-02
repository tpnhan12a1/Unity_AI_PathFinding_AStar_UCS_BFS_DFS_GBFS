using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FinshedGame : MonoBehaviour
{
    Recursive maze;

    private void Start()
    {
        maze = GameObject.Find("Maze").GetComponent<Recursive>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Start"))
        {
            RemoveAllWall();
            maze.height++;
            maze.width++;
            maze.difficult++;
            maze.NewMap();
            List<MapLocation> locations = new List<MapLocation>();
            for (int z = 0; z < maze.height - 1; z++)
                for (int x = 0; x < maze.width - 1; x++)
                {
                    if (maze._map[x, z] != 1)
                        locations.Add(new MapLocation(x, z));
                }
            //Vector3 goalLocation = new Vector3(locations[locations.Count - 1].x * maze.scale, 0, locations[locations.Count - 1].z * maze.scale);


            Vector3 start = GameObject.FindGameObjectWithTag("Start").transform.position;
            locations = locations.OrderByDescending(x => x.Distance(start, x.ConvertToVector3(new MapLocation(x.x, x.z)))).ToList<MapLocation>();
            GameObject.FindGameObjectWithTag("Goal").transform.position = locations.ElementAt(0).ConvertToVector3();
               
             
        }    
    }
    public  void RemoveAllWall()
    {
        GameObject[] paths = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject path in paths)
            Destroy(path);
    }

}
