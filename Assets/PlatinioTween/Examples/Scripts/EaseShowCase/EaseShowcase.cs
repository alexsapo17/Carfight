using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platinio.TweenEngine
{
    public class EaseShowcase : MonoBehaviour
    {
        [SerializeField] private Ease ease;
        [SerializeField] private float time;
        [SerializeField] private Vector3 pos;

        private Vector3 startPosition = Vector3.zero;

 private void Start()
{
    startPosition = transform.position;
    Debug.Log("Start - Position in Editor: " + startPosition);
    Move();
}

private void Move()
{
    Debug.Log("Move - Current Position: " + transform.position + ", Target Position: " + pos);
    transform.position = startPosition;
    transform.Move(pos, time).SetEase(ease);
    Debug.Log("After Move - New Position: " + transform.position);
}

    }

}

