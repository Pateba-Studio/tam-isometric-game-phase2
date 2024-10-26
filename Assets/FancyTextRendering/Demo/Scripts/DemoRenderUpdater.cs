using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LogicUI.FancyTextRendering;
using NaughtyAttributes;

namespace FancyTextRendering.Demo
{
    public class DemoRenderUpdater : MonoBehaviour
    {
        [SerializeField] TMP_InputField MarkdownSourceInputField;
        [SerializeField] MarkdownRenderer MarkdownRenderer;

        [Button]
        private void UpdateRender()
        {
            MarkdownRenderer.Source = MarkdownSourceInputField.text;
        }
    }
}
