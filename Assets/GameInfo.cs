using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameInfo {

    public const string GroundLayerName = "Ground";
    public static readonly int GroundLayer = LayerMask.NameToLayer(GroundLayerName);
    public static readonly int GroundMask = LayerMask.GetMask(GroundLayerName);
}
