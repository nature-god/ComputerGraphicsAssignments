using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 *Spatial Straight-Line Algorithm
 * Related Paper: http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.845.6869&rep=rep1&type=pdf
 */
public class LineManager : MonoBehaviour {

    //Calculate the dx,dy,fz
    private int dx;
    private int dy;
    private int dz;

    //Calculate the starting decision parameters
    private int ey;
    private int ez;

    //Calculate constants 2dy, 2(dy-dx)
    private int twoDy;
    private int twoDyDx;

    //Calculate constants 2dz,2(dz-dx)
    private int twoDz;
    private int twoDzDx;
    public void DrawLine(Vector3 StartPoint,Vector3 EndPoint)
    {
        int x1, y1, z1;
        int x2, y2, z2;
        x1 = (int)StartPoint.x;
        y1 = (int)StartPoint.y;
        z1 = (int)StartPoint.z;

        x2 = (int)EndPoint.x;
        y2 = (int)EndPoint.y;
        z2 = (int)EndPoint.z;
        //Assuming dx >= dy > 0 and dx >= dz > 0
       
        int _xy = (Mathf.Abs(y2 - y1) > Mathf.Abs(x2 - x1)) ? 1 : 0;
        if(_xy == 1)
        {
            //交换x1,y1交换x2,y2
            int tmp = x1;
            x1 = y1;
            y1 = tmp;
            tmp = x2;
            x2 = y2;
            y2 = tmp;
        }
        int _xz = (Mathf.Abs(z2 - z1) > Mathf.Abs(x2 - x1)) ? 1 : 0;

        if (_xz == 1)
        {
            //交换x1,z1,交换x2,z2;
            int tmp = x1;
            x1 = z1;
            z1 = tmp;
            tmp = x2;
            x2 = z2;
            z2 = tmp;
        }


        if(x1 > x2)
        {
            //EP.x < SP.x交换
            int tmpx = x1;
            int tmpy = y1;
            int tmpz = z1;

            x1 = x2;
            y1 = y2;
            z1 = z2;

            x2 = tmpx;
            y2 = tmpy;
            z2 = tmpz;
        }

        dx = Mathf.Abs(x2 - x1);
        dy = Mathf.Abs(y2 - y1);
        dz = Mathf.Abs(z2 - z1);

        ey = 2 * dy - dx;
        ez = 2 * dz - dx;

        twoDy = 2 * dy;
        twoDyDx = 2 * (dy - dx);

        twoDz = 2 * dz;
        twoDzDx = 2 * (dz - dx);

        //Initialization of the interpolation's position
        int x = x1;
        int y = y1;
        int z = z1;

        int incy = (y1 < y2) ? 1 : -1;
        int incz = (z1 < z2) ? 1 : -1;

        for (int i =0;i<=dx;i++)
        {
            if(_xy == 1)
            {
                if(_xz == 1)
                {
                    Draw3DPoint(y, z, x);
                }
                else
                {
                    Draw3DPoint(y, x, z);
                }
            }
            else
            {
                if (_xz == 1)
                {
                    Draw3DPoint(z, y, x);
                }
                else
                {
                    Draw3DPoint(x, y, z);
                }
            }
            x++;
            if(ey >= 0)
            {
                y += incy;
                ey += twoDyDx;
            }
            else
            {
                ey += twoDy;
            }

            if(ez >= 0)
            {
                z += incz;
                ez += twoDzDx;
            }
            else
            {
                ez += twoDz;
            }
        }
    }

    private void Draw3DPoint(int x,int y,int z)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.SetParent(this.transform);
        go.transform.localPosition = new Vector3(x, y, z);
    }

    public void Reset()
    {
        for(int i=0;i<transform.childCount;i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
