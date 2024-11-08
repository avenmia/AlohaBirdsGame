using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ColorChangingText : MonoBehaviour
{

    public TMP_Text textComponent;
    public Color targetColor = Color.cyan;
    public float changeDuration = 2f;

    public FloatingText floatingTextScript;

    // Start is called before the first frame update
    void Start()
    {
        if(textComponent == null)
        {
            Debug.LogError("Text component is not assigned. Please assign text component.");
            return;
        }

        if(!textComponent.isActiveAndEnabled)
        {
            Debug.LogError("Text component is disabled. Please enable it.");
            return;
        }

        StartCoroutine(ChangeTextColor());
    }

    private IEnumerator ChangeTextColor()
    {
        textComponent.ForceMeshUpdate();
        
        Color originalColor = textComponent.color;
        int characterCount = textComponent.textInfo.characterCount;

        if(characterCount == 0)
        {
            Debug.LogWarning("No characters found in the text component.");
            yield break;
        }

        for (int i = 0; i < characterCount; i++)
        {
            float t = 0f;
            Color currentColor;

            while (t < 1f)
            {
                t += Time.deltaTime / changeDuration;
                currentColor = Color.Lerp(originalColor, targetColor, t);
                UpdateCharacterColor(i, currentColor);
                yield return null;
            }

            UpdateCharacterColor(i, targetColor);
        }
        if(floatingTextScript!= null)
        {
            FloatingText.StartFloatingText(3f, 6f);
        }
        else
        {
            Debug.LogError("Floating text script reference is not assigned.");
        }
    }

    private void UpdateCharacterColor(int index, Color color)
    {

        var textInfo = textComponent.textInfo;


        if(textInfo.characterCount <= index)
        {
            Debug.LogError($"Index {index} is out of bounds for character count {textInfo.characterCount}.");
            return;
        }

        var character = textInfo.characterInfo[index];

        if (character.isVisible)
        {
            int materialIndex = character.materialReferenceIndex;
            int vertexIndex = character.vertexIndex;

            Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;


            vertexColors[vertexIndex + 0] = (Color32)color;
            vertexColors[vertexIndex + 1] = (Color32)color;
            vertexColors[vertexIndex + 2] = (Color32)color;
            vertexColors[vertexIndex + 3] = (Color32)color;

            textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }
    }
}
