using System;
using DG.Tweening;
using UnityEngine;

namespace RS.Typing.Core {
    public class Player : MonoBehaviour {
        [SerializeField] private float angleOffset = 90f;
        private void Awake() {
            WordObject.WordMatched += WordObjectOnWordMatched;
        }

        private void OnDestroy() {
            WordObject.WordMatched -= WordObjectOnWordMatched;
        }

        private void WordObjectOnWordMatched(object sender, EventArgs e) {
            var wordObject = sender as WordObject;
            if (wordObject == null) return;

            var direction = (wordObject.transform.position - transform.position).normalized;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            transform.DORotateQuaternion(Quaternion.Euler(0, 0, angle-angleOffset), .15f);
        }

        private void Start() {
        
        }
        private void Update() {
        
        }
    }
}
