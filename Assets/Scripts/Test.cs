using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Action<int> act = Add;
        Type myType = act.GetType();
        Assembly ass = act.GetType().Assembly;
        myType.GetFields();
        Debug.Log(myType.Name);
        Debug.Log(myType.Namespace);
        Debug.Log(myType.Assembly);
    }

    void Add(int a)
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
