using System;
using UnitSystem;
using UnityEngine;

public class BasicUnit : Unit
{
    
    private void OnValidate()
    {
        if (unitSO != null)
        {
            gameObject.name = unitSO.UnitName;
        }
    }
}
