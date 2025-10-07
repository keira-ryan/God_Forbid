using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueTypewriter : MonoBehaviour
{
    [SerializeField] private float typeSpeed = 50f;

    public bool IsRunning {get; private set;}
    
    private readonly List<Punctuation> punctuations = new List<Punctuation>()
    {
        new Punctuation(new HashSet<char>() {'.', '!', '?'}, 0.6f),
        new Punctuation(new HashSet<char>() {',', ';', ':'}, 0.3f),
    };
    
    private Coroutine typingCoroutine;
    public void Run(string textToType, TextMeshProUGUI textComponent)
    {
        typingCoroutine = StartCoroutine(TypeText(textToType, textComponent));
    }

    public void Stop()
    {
        StopCoroutine(typingCoroutine);
        IsRunning = false;
    }

    private IEnumerator TypeText(string textToType, TextMeshProUGUI textComponent)
    {
        IsRunning = true;
        textComponent.text = string.Empty;
        
        float typeTime = 0;
        int charIndex = 0;

        while (charIndex < textToType.Length)
        {
            int lastCharIndex = charIndex;
            
            typeTime += Time.deltaTime * typeSpeed;
            charIndex = Mathf.FloorToInt(typeTime);
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

            for (int i = lastCharIndex; i < charIndex; i++)
            {
                bool isLast = i >= textToType.Length - 1;
                
                textComponent.text = textToType.Substring(0, i+1);

                if (IsPunctuation(textToType[i], out float waitTime) && !isLast &&
                    !IsPunctuation(textToType[i + 1], out _))
                {
                    yield return new WaitForSeconds(waitTime);
                }
            }

            yield return null;
        }
        IsRunning = false;
    }

    private bool IsPunctuation(char character, out float waitTime)
    {
        foreach (Punctuation punctuationCategory in punctuations)
        {
            if (punctuationCategory.Punctuations.Contains(character))
            {
                waitTime = punctuationCategory.WaitTime;
                return true;
            }
        }
        
        waitTime = default;
        return false;
    }

    private readonly struct Punctuation
    {
        public readonly HashSet<char> Punctuations;
        public readonly float WaitTime;

        public Punctuation(HashSet<char> punctuations, float waitTime)
        {
            Punctuations =  punctuations;
            WaitTime = waitTime;
        }
    }
}
