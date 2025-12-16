using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimationCotroller : MonoBehaviour
{
    public Animator animator;

    public void playAnimation(string animationName)
    {
        if (animator == null)
        {
            Debug.LogWarning("MenuAnimationCotroller: Animator reference is missing.");
            return;
        }

        if (string.IsNullOrEmpty(animationName))
        {
            Debug.LogWarning("MenuAnimationCotroller: animationName is null or empty.");
            return;
        }

        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogWarning("MenuAnimationCotroller: Animator has no Controller assigned.");
            return;
        }

        var stateHash = Animator.StringToHash(animationName);
        var foundLayer = -1;
        for (int layer = 0; layer < animator.layerCount; layer++)
        {
            if (animator.HasState(layer, stateHash))
            {
                foundLayer = layer;
                break;
            }
        }

        if (foundLayer == -1)
        {
            Debug.LogWarning($"MenuAnimationCotroller: State '{animationName}' not found in any layer.");
            return;
        }

        animator.Play(animationName, foundLayer, 0f);
    }
}
