using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : IBullet {

    [SerializeField] private float lifetime;

    private float lifetimer;

    public override void Spawn(Vector3 position) {
        base.Spawn(position);
        lifetimer = 0;
    }

    protected override void Update() {

        base.Update();

        lifetimer += Time.deltaTime;

        if (lifetimer > lifetime) Destroy();
    }
}
