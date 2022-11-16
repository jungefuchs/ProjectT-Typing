using System;
using System.Collections;

using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace RS.Typing.Core {
    public class WordObject : MonoBehaviour {
        public string testWord;
        public static event EventHandler WordMatched;
        private static WordObject _lockedWordObject = null;
        [SerializeField] private TMP_Text text;
        private string _word;
        private bool _attacked;
        
        private void Start() {
            if (testWord != "") Setup(testWord);
        }

        private void Setup(string word) {
            _word = word;
            text.text = _word;
            
            InputSystem.onAnyButtonPress.Call(ctrl => {
                if (ctrl.name.Length == 1) AttemptInput(ctrl.name);
            });

            var player = GameObject.FindGameObjectWithTag("Player");

            StartCoroutine(MoveToOtherTransform(player.transform));
        }
        private void AttemptInput(string c) {
            if (!_word.StartsWith(c)) return;
            if (_lockedWordObject != null && _lockedWordObject != this) return;
            _lockedWordObject = this;
            _word = _word.Remove(0, 1);
            text.text = _word;
            _attacked = true;
            
            WordMatched?.Invoke(_word == "" ? null: this, null);
            CheckEmpty();

        }
        private void CheckEmpty() {
            if (_word == "") {
                _lockedWordObject = null;
                Destroy(gameObject);    
            }
        }

        private IEnumerator MoveToOtherTransform(Transform otherTransform) {
            while (transform.position != otherTransform.position) {
                var direction = otherTransform.position - transform.position;
                transform.Translate(direction.normalized * Time.deltaTime);

                if (_attacked) {
                    yield return new WaitForSeconds(.1f);
                    _attacked = false;
                }
                yield return null;
            }
        }
    }
}