using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node[] neighbors;
    public Vector2[] directions;

    void Start()
    {
        directions = new Vector2[neighbors.Length];

        for(int i = 0; i < neighbors.Length; i++)
        {
            Node neighbor = neighbors[i];
            Vector2 temp = neighbor.transform.localPosition - transform.localPosition;

            directions[i] = temp.normalized;
        }
    }

}
