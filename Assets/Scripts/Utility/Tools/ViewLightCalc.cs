using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ViewLightCalc : MonoBehaviour
{
    public void DrawRandonLine()
    {
        var childs = GetComponentsInChildren<Transform>().Where(x => x != transform).ToList();
        foreach (var child in childs)
        {
            DestroyImmediate(child.gameObject);
        }
        
        Vector2 startPos = new Vector2(UnityEngine.Random.Range(0, 40), UnityEngine.Random.Range(0, 40));
        Vector2 endPos = new Vector2(UnityEngine.Random.Range(0, 40), UnityEngine.Random.Range(0, 40));
        
        Debug.Log($"StartPos: {startPos}, EndPos: {endPos}");
        
        // TraceLine(startPos, endPos);
    }

    // public void TraceLineA(Vector2 startPos, Vector2 endPos)
    // {
    //     int startX = (int)startPos.x;
    //     int startY = (int)startPos.y;
    //     int endX = (int)endPos.x;
    //     int endY = (int)endPos.y;
    //
    //     int deltaX = endX - startX;
    //     int deltaY = endY - startY;
    //     
    //     int signX = deltaX >= 0 ? 1 : -1;
    //     int signY = deltaY >= 0 ? 1 : -1;
    //     
    //     SetVisible(startPos);
    //     
    //     deltaX = Math.Abs(deltaX);
    //     deltaY = Math.Abs(deltaY);
    //     
    //     int baseX = startX;
    //     int baseY = startY;
    //     
    //     int error = (deltaY << 1) - deltaX;
    //     int errorInc1 = deltaY << 1;
    //     int errorInc2 = (deltaY - deltaX) << 1;
    //
    //     while (baseX != endX)
    //     {
    //         baseX += signX;
    //             
    //         if (error >= 0)
    //         {
    //             error += errorInc2;
    //             baseY += signY;
    //         }
    //         else
    //         {
    //             error += errorInc1;
    //         }
    //         SetVisible(new Vector2(baseX, baseY));
    //     }
    // }
    
    public void ConputeViewArea(Vector2 startPos, int range)
    {
        if(range == 0) return;
        
        int left = (int)startPos.x - range;
        int right = (int)startPos.x + range;
        int top = (int)startPos.y + range;
        int bottom = (int)startPos.y - range;
        
        for (int x = left; x <= right; x++)
        {
            TraceLine(startPos, new Vector2(x, top), range);
            TraceLine(startPos, new Vector2(x, bottom), range);
        }
        
        for (int y = bottom; y <= top; y++)
        {
            TraceLine(startPos, new Vector2(left, y), range);
            TraceLine(startPos, new Vector2(right, y), range);
        }
    }
    
    public void TraceLine(Vector2 startPos, Vector2 endPos, int range)
    {
        
        int startX = (int)startPos.x;
        int startY = (int)startPos.y;
        int endX = (int)endPos.x;
        int endY = (int)endPos.y;
    
        int deltaX = endX - startX;
        int deltaY = endY - startY;
        
        int signX = deltaX >= 0 ? 1 : -1;
        int signY = deltaY >= 0 ? 1 : -1;
        
        SetVisible(startPos);
        
        // Version 1
        // float k = (float)deltaY / deltaX;
        // float y = startY;
        //
        // float error = k;
        // float errorInc1 = k;
        // float errorInc2 = k - 1;
        //
        // for(int x = startX; x < endX; x += incrementX)
        // {
        //     if (error > 0.5f)
        //     {
        //         error += errorInc2;
        //     }
        //     else
        //     {
        //         error += errorInc1;
        //         y += 1;
        //     }
        //     
        //     SetVisible(new Vector2(x, y));
        // }
        
        
        // 现在这是比较正负
        // float error = k - 0.5f;
        // float errorInc1 = k;
        // float errorInc2 = k - 1;
        
        // 所以乘以一个deltaX也不影响
        deltaX = Math.Abs(deltaX);
        deltaY = Math.Abs(deltaY);
        
        int baseX = startX;
        int baseY = startY;
        
        if (deltaX >= deltaY)
        {
            int error = (deltaY << 1) - deltaX;
            int errorInc1 = deltaY << 1;
            int errorInc2 = (deltaY - deltaX) << 1;
    
            while (baseX != endX)
            {
                baseX += signX;
                
                if (error >= 0)
                {
                    error += errorInc2;
                    baseY += signY;
                }
                else
                {
                    error += errorInc1;
                }
                if(CalcDist(baseX, baseY, startX, startY) > range) break;
                SetVisible(new Vector2(baseX, baseY));
                if(BlockLight(baseX, baseY)) break;
            }
        }
        else
        {
            int error = (deltaX << 1) - deltaY;
            int errorInc1 = deltaX << 1;
            int errorInc2 = (deltaX - deltaY) << 1;
    
            while (baseY != endY)
            {
                baseY += signY;
                
                if (error >= 0)
                {
                    error += errorInc2;
                    baseX += signX;
                }
                else
                {
                    error += errorInc1;
                }
                if(CalcDist(baseX, baseY, startX, startY) > range) break;
                SetVisible(new Vector2(baseX, baseY));
                if(BlockLight(baseX, baseY)) break;
            }
        }
    }

    private bool BlockLight(int baseX, int baseY)
    {
        return false;
    }

    private void SetVisible(Vector2 pos)
    {
        PrimitiveType primitiveType = PrimitiveType.Cube;
        GameObject go = GameObject.CreatePrimitive(primitiveType);
        go.transform.position = pos;
        go.transform.parent = transform;
    }
    
    private int CalcDist(int AposX, int AposY, int BposX, int BposY)
    {
        return Math.Abs(AposX - BposX) + Math.Abs(AposY - BposY);
    }
    
    private int CalcDist(Vector2 posA, Vector2 posB)
    {
        return (int)(Mathf.Abs(posA.x - posB.x) + Mathf.Abs(posA.y - posB.y));
    }
}
