using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using static ToolHit;

public class TreeCuttable : ToolHit
{
    public override void Hit()
    {
        Debug.Log("Tree cut!");
    }
}
