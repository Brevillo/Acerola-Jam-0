using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerseScroll : MonoBehaviour {

    [SerializeField] private RectTransform position, size;
    [SerializeField] private float scrollSpeed;

    private void Update() {

        Vector2 pos = position.localPosition;

        pos.x -= scrollSpeed * Time.deltaTime;
        pos.x %= size.sizeDelta.x;

        position.localPosition = pos;
    }
}
