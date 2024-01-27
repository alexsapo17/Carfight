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
        private bool isMoving = false;

        private void OnEnable()
        {
            // Controlla se il padre è attivo e, se necessario, attivalo
            Transform parent = transform.parent;
            if (parent != null && !parent.gameObject.activeSelf)
            {
                parent.gameObject.SetActive(true);
            }

            startPosition = transform.position;
            Move();
        }

        private void OnDisable()
        {
            // Interrompi l'animazione se è in corso
            if (isMoving)
            {
                StopCoroutine("MoveCoroutine");
                isMoving = false;
            }

            // Riporta l'oggetto alla sua posizione originale
            transform.position = startPosition;
        }

        private void Move()
        {
            if (!isMoving)
            {
                StartCoroutine(MoveCoroutine());
            }
        }

        private IEnumerator MoveCoroutine()
        {
            isMoving = true;
            transform.position = startPosition;
            transform.Move(pos, time).SetEase(ease);

            // Aspetta che l'animazione sia completata
            yield return new WaitForSeconds(time);

            isMoving = false;
        }
    }
}
