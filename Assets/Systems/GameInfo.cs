using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameInfo {

    public const string
        GroundLayerName = "Ground",
        BulletLayerName = "Bullet";

    public static readonly int
        GroundLayer = LayerMask.NameToLayer(GroundLayerName),
        BulletLayer = LayerMask.NameToLayer(BulletLayerName);

    public static readonly int
        GroundMask = LayerMask.GetMask(GroundLayerName),
        BulletMask = LayerMask.GetMask(BulletLayerName);
}
