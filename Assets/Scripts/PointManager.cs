using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour {

    public GameObject pointPrefab;
	public void DrawPoint(Vector3 pos)
    {
        GameObject go = Instantiate(pointPrefab,this.transform);
        go.transform.localPosition = pos;
    }
    public void Reset()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
