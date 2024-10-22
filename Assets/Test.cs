using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private Action<string> onSayHello;
    
    void Start()
    {
        Action<string> f1 = (_) => { SayHello(); };
        onSayHello += f1;
        onSayHello -= f1;
        // 输出两个匿名函数
        
        onSayHello?.Invoke("Kurumi");
    }

    private void SayHello()
    {
        Debug.Log("Hello Marsin!");
    }
}
