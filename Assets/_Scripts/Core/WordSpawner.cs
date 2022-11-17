using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace RS.Typing.Core {
    public class WordSpawner : MonoBehaviour {
        [SerializeField] private TextAsset wordsFile;
        [SerializeField] private WordObject wordObjectPrefab;
        [SerializeField] private int wordsPerWave = 4;
        
        private List<string> _wordBank;
        private ObjectPool<WordObject> _pool;

        private int _currentWords;

        private void Awake() {
            _wordBank = wordsFile.text.Split("\n").Where(word => word.Length> 3).ToList();
            WordObject.WordMatched += WordObjectOnWordMatched;
        }

        private void Start() {
            _pool = new ObjectPool<WordObject>(
                () => Instantiate(wordObjectPrefab, GetRandomPosition(), Quaternion.identity),
                word => {
                    word.gameObject.SetActive(true);
                }, 
                word => word.gameObject.SetActive(false), 
                word => Destroy(word.gameObject), true, 50, 100);
            SpawnWords(wordsPerWave);
        }

        private void OnDestroy() {
            WordObject.WordMatched -= WordObjectOnWordMatched;
        }

        private void WordObjectOnWordMatched(object wordObject, bool isMatched) {
            if (wordObject != null || !isMatched) return; // null : word object is already deleted/destroyed
            _currentWords--;
            if (_currentWords <= 0) {
                SpawnWords(wordsPerWave);
            }
        }

        private void SpawnWords(int amount) {
            var words = GetRandomWords(amount).ToArray();
            for (var i = 0; i < amount; i++) {
                var word = _pool.Get();
                word.Setup(words[i], GetRandomPosition(), ()=> _pool.Release(word));
            }
            _currentWords = amount;
        }

        private Vector3 GetRandomPosition() {
            var randInt = Random.Range(0, transform.childCount);
            var position = transform.GetChild(randInt).position;
            
            while (Physics2D.OverlapCircle(position, 1f)) {
                randInt = Random.Range(0, transform.childCount);
                position = transform.GetChild(randInt).position;
            }
            return position;
        }

        private IEnumerable<string> GetRandomWords(int amount) {
            var words = new List<string>();
            var random = new System.Random();

            while (words.Count < amount) {
                var startingChars = words.Select(s => s[0]);
                var word = _wordBank.ElementAt(random.Next(0, _wordBank.Count));
                
                if (words.Contains(word)) continue;
                if (startingChars.Any(c => c == word[0])) continue;
                
                words.Add(word);
            }
            return words;
        }
    }
}