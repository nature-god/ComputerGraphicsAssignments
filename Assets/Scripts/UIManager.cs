using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Text output;
    public LineManager line;
    public PointManager point;
    public PointManager resPoint;
    public PointManager zPoint;
    public PointManager planePoint;
    private float Px, Py, Pz;
    private float SPx, SPy, SPz;
    private float EPx, EPy, EPz;
    private float Angle;


    private Vector3 rotatePos = new Vector3();
    private Vector3 zAxisPos = new Vector3();
    private Vector3 planePos = new Vector3();

	public void OnPxChange(string str)
    {
        Px = System.Convert.ToSingle(str);
    }
    public void OnPyChange(string str)
    {
        Py = System.Convert.ToSingle(str);
    }
    public void OnPzChange(string str)
    {
        Pz = System.Convert.ToSingle(str);
    }
    public void OnSPxChange(string str)
    {
        SPx = System.Convert.ToSingle(str);
    }
    public void OnSPyChange(string str)
    {
        SPy = System.Convert.ToSingle(str);
    }
    public void OnSPzChange(string str)
    {
        SPz = System.Convert.ToSingle(str);
    }
    public void OnEPxChange(string str)
    {
        EPx = System.Convert.ToSingle(str);
    }
    public void OnEPyChange(string str)
    {
        EPy = System.Convert.ToSingle(str);
    }
    public void OnEPzChange(string str)
    {
        EPz = System.Convert.ToSingle(str);
    }
    public void OnAngleChange(string str)
    {
        Angle = System.Convert.ToSingle(str);
    }

    public void OnClickRotate()
    {
        line.Reset();
        point.Reset();
        resPoint.Reset();
        zPoint.Reset();


        line.DrawLine(new Vector3(SPx, SPy, SPz), new Vector3(EPx, EPy, EPz));
        point.DrawPoint(new Vector3(Px, Py, Pz));
        RotateAroundLine(new Vector3(Px, Py, Pz), new Vector3(SPx, SPy, SPz), new Vector3(EPx, EPy, EPz), Angle);
        resPoint.DrawPoint(rotatePos);
        zPoint.DrawPoint(zAxisPos);
    }

    private void RotateAroundLine(Vector3 point,Vector3 StartPos,Vector3 EndPos,float RAngle)
    {
        //1.将直线平移回原点位置
        //2.将旋转轴旋转至XOZ平面（绕Z轴旋转）
        //3.将旋转轴旋转至与Z轴重合（绕y轴旋转）
        //4.绕z轴旋转指定度数
        //5.步骤3的逆过程
        //6.步骤2的逆过程
        //7.步骤1的逆过程

        /* T1矩阵
         * |1 0 0 x0-x1|
         * |0 1 0 y0-y1|
         * |0 0 1 z0-z1|
         * |0 0 0 1    |
         */ 
        Matrix4x4 T1 = Matrix4x4.identity;
        T1.m03 = StartPos.x - EndPos.x;
        T1.m13 = StartPos.y - EndPos.y;
        T1.m23 = StartPos.z - EndPos.z;

        /*
         * 计算投影与x轴的夹角,绕z轴旋转
         * 余玄定理
         * |cos  -sin  0   0|
         * |sin  cos   0   0|
         * | 0    0    1   0|
         * | 0    0    0   1|
         */
        float theta = GetCosTheta(new Vector2(1.0f, 0.0f),new Vector2(EndPos.x - StartPos.x,EndPos.y-StartPos.y));
        Debug.Log(theta);
        Matrix4x4 T2 = Matrix4x4.identity;
        T2.m00 = Mathf.Cos(theta);
        T2.m01 = -1.0f * Mathf.Sin(theta);
        T2.m10 = Mathf.Sin(theta);
        T2.m11 = Mathf.Cos(theta);

        /*
         * 计算与z轴的夹角，绕y轴旋转
         * 余玄定理
         * |cos   0   sin  0|
         * | 0    1    0   0|
         * |-sin  0   cos  0|
         * | 0    0    0   1|
         */
        float theta2 = GetCosTheta(new Vector3(0.0f, 0.0f, 1.0f), EndPos - StartPos);
        Matrix4x4 T3 = Matrix4x4.identity;
        T3.m00 = Mathf.Cos(theta2);
        T3.m02 = Mathf.Sin(theta2);
        T3.m20 = -1.0f * Mathf.Sin(theta2);
        T3.m22 = Mathf.Cos(theta2);

        /*
         * 绕z轴旋转指定角度
         * |cos  -sin  0   0|
         * |sin  cos   0   0|
         * | 0    0    1   0|
         * | 0    0    0   1|
         */
        Matrix4x4 T4 = Matrix4x4.identity;
        RAngle = RAngle * Mathf.PI/180.0f;
        T4.m00 = Mathf.Cos(RAngle);
        T4.m01 = -1.0f * Mathf.Sin(RAngle);
        T4.m10 = Mathf.Sin(RAngle);
        T4.m11 = Mathf.Cos(RAngle);

        //绕y轴回转
        Matrix4x4 T5 = Matrix4x4.identity;
        T5.m00 = Mathf.Cos(-1.0f*theta2);
        T5.m02 = Mathf.Sin(-1.0f * theta2);
        T5.m20 = -1.0f * Mathf.Sin(-1.0f * theta2);
        T5.m22 = Mathf.Cos(-1.0f * theta2);

        //绕z轴回转
        Matrix4x4 T6 = Matrix4x4.identity;
        T6.m00 = Mathf.Cos(-1.0f*theta);
        T6.m01 = -1.0f * Mathf.Sin(-1.0f*theta);
        T6.m10 = Mathf.Sin(-1.0f*theta);
        T6.m11 = Mathf.Cos(-1.0f*theta);

        //平移回去
        Matrix4x4 T7 = Matrix4x4.identity;
        T7.m03 = EndPos.x - StartPos.x;
        T7.m13 = EndPos.y - StartPos.y;
        T7.m23 = EndPos.z - StartPos.z;
        
        /*
         *按Z轴对称的变换矩阵
         * |-1  0  0  0|
         * |0  -1  0  0|
         * |0   0  1  0|
         * |0   0  0  1|
         */
        Matrix4x4 TzAxis = Matrix4x4.identity;
        TzAxis.m00 = -1.0f;
        TzAxis.m11 = -1.0f;


        Vector3 tmp = point;
        tmp = LeftMul(point, T1);
        tmp = LeftMul(tmp, T2);
        tmp = LeftMul(tmp, T3);
        tmp = LeftMul(tmp, T4);
        tmp = LeftMul(tmp, T5);
        tmp = LeftMul(tmp, T6);
        tmp = LeftMul(tmp, T7);
        rotatePos = tmp;

        Vector3 tmp2 = point;
        tmp2 = LeftMul(point, T1);
        tmp2 = LeftMul(tmp2, T2);
        tmp2 = LeftMul(tmp2, T3);
        tmp2 = LeftMul(tmp2, TzAxis);
        tmp2 = LeftMul(tmp2, T5);
        tmp2 = LeftMul(tmp2, T6);
        tmp2 = LeftMul(tmp2, T7);
        zAxisPos = tmp2;

        output.text = "关于直线旋转的点:" + "\n" + rotatePos.ToString()+"\n"
                     +"关于直线对称的点:" + "\n" + zAxisPos.ToString() + "\n"
                     +"关于平面对称的点:" + "\n" + planePos.ToString();
    }

    private float GetCosTheta(Vector3 v1,Vector3 v2)
    {
        return Mathf.Acos((v1.x * v2.x + v1.y * v2.y + v1.z * v2.z) / (v1.magnitude * v2.magnitude));
    }

    private float GetCosTheta(Vector2 v1,Vector2 v2)
    {
        if (v2.magnitude == 0.0f)
            return 0.0f;
        else
            return Mathf.Acos((v1.x * v2.x + v1.y * v2.y) / (v1.magnitude * v2.magnitude));
    }

    private Vector3 LeftMul(Vector3 v,Matrix4x4 M)
    {
        Vector3 tmp = new Vector3(M.m00 * v.x + M.m01 * v.y + M.m02 * v.z + M.m03 * 1.0f,
                                  M.m10 * v.x + M.m11 * v.y + M.m12 * v.z + M.m13 * 1.0f,
                                  M.m20 * v.x + M.m21 * v.y + M.m22 * v.z + M.m23 * 1.0f);
        return tmp;
    }
}
