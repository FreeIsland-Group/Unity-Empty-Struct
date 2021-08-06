using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Prefab
{
    public class Remind : MonoBehaviour
    {
        public Text info;

        public void InitText(string infoText)
        {
            info.text = infoText;
            StartCoroutine(HideSelf(transform.GetComponent<CanvasGroup>()));
        }

        private IEnumerator HideSelf(CanvasGroup self)
        {
            for (float f = 0; f < 1; f -= 0.0016f)
            {
                self.alpha = -Mathf.Pow(f, 2) + 1;
                if (self.alpha < .001f)
                {
                    Destroy(gameObject);
                    yield break;
                }
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
