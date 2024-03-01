using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverBeebe.UnityUtilities.Runtime.Input;

public class PlayerInput : InputManager {

    [Header("Movement")]
    public Button Jump;
    public Button Crouch;
    public Button Dash;
    public Axis Movement;
    public Axis Looking;

    [Header("Debuggin")]
    public Button Debug;
}