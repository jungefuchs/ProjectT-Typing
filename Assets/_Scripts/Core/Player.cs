using DG.Tweening;
using UnityEngine;

namespace RS.Typing.Core {
    public class Player : MonoBehaviour {
        [SerializeField] private float angleOffset = 90f;

        private float _timeRefresh = float.MaxValue;
        private void Awake() {
            WordObject.WordMatched += WordObjectOnWordMatched;
        }

        private void OnDestroy() {
            WordObject.WordMatched -= WordObjectOnWordMatched;
        }

        private void WordObjectOnWordMatched(object sender, bool isMatched) {
            if (!isMatched) return;
            var wordObject = sender as WordObject;
            if (wordObject == null) return;

            var direction = (wordObject.transform.position - transform.position).normalized;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            transform.DORotateQuaternion(Quaternion.Euler(0, 0, angle-angleOffset), .15f).OnComplete(() => {
                _timeRefresh = Time.time + .25f;
            });
        }

        private void Update() {
            if (Time.time > _timeRefresh) {
                Refresh();
            }
        }

        private void Refresh() {
            transform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), .15f);
            _timeRefresh = float.MaxValue;
        }
    }
}
