using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RS.Typing.Core {
    public class WordObjectHighlighter : MonoBehaviour {
        [SerializeField] private TMP_Text text;
        [SerializeField] private Material highlightedTextMaterial;
        [SerializeField] private Material defaultTextMaterial;
        [SerializeField] private Image backgroundTextImage;

        private WordObject _wordObject;

        private void Awake() {
            _wordObject = GetComponent<WordObject>();
            WordObject.WordMatched += WordObjectOnWordMatched;
        }

        private void OnDestroy() {
            WordObject.WordMatched -= WordObjectOnWordMatched;
        }

        private void WordObjectOnWordMatched(object sender, bool isMatch) {
            if (sender is WordObject wordObject && wordObject == _wordObject) {
                text.fontMaterial = isMatch ? highlightedTextMaterial : defaultTextMaterial;
                backgroundTextImage.enabled = isMatch;
            }
            
            
        }

        private void OnDisable() {
            text.fontMaterial = defaultTextMaterial;
            backgroundTextImage.enabled = false;
        }
    }
}