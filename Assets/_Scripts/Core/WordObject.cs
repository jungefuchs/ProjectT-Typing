using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace RS.Typing.Core {
    public class WordObject : MonoBehaviour {
        public static event EventHandler<bool> WordMatched;

        [SerializeField] private TMP_Text text;
        [SerializeField] private float knockBackTime;
        
        private static WordObject _lockedWordObject;
        private Action _wordDestroyed;

        private float _speed;
        private string _word;
        private bool _attacked;

        public void Setup(string word, Vector3 position, Action onDestroy) {
            _word = word;
            _speed = Mathf.Clamp(1.5f - word.Length * 0.075f, 0.25f, float.MaxValue);
            _wordDestroyed = onDestroy;
            
            text.text = _word;
            transform.position = position;
            
            InputSystem.onAnyButtonPress.Call(ctrl => {
                if (ctrl.name.Length == 1) AttemptInput(ctrl.name);
            });

            var player = GameObject.FindGameObjectWithTag("Player");
            StartCoroutine(MoveToOtherTransform(player.transform));
        }
        private void AttemptInput(string c) {
            if (_lockedWordObject != null && _lockedWordObject != this) return;
            if (!_word.StartsWith(c)) {
                Error();
                return;
            }
            
            _lockedWordObject = this;
            
            _word = _word.Remove(0, 1);
            
            text.text = _word;
            _attacked = true;
            
            WordMatched?.Invoke(_word == "" ? null: this, true);
            CheckEmpty();

        }
        private void CheckEmpty() {
            if (_word == "") {
                _wordDestroyed?.Invoke();
                _lockedWordObject = null;
            }
        }

        private void Error() {
            WordMatched?.Invoke(this, false);
        }

        private IEnumerator MoveToOtherTransform(Transform otherTransform) {
            while (transform.position != otherTransform.position) {
                var direction = otherTransform.position - transform.position;
                transform.Translate(direction.normalized * (Time.deltaTime * _speed));

                if (_attacked) {
                    yield return new WaitForSeconds(knockBackTime);
                    _attacked = false;
                }
                yield return null;
            }
        }
    }
}