// Animancer // https://kybernetik.com.au/animancer // Copyright 2022 Kybernetik //

using UnityEngine;

namespace Animancer
{
    /// <summary>Plays a single <see cref="AnimationClip"/>.</summary>
    /// <remarks>
    /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/playing/component-types">Component Types</see>
    /// </remarks>
    /// <example>
    /// <see href="https://kybernetik.com.au/animancer/docs/examples/fine-control/solo-animation">Solo Animation</see>
    /// </example>
    /// https://kybernetik.com.au/animancer/api/Animancer/SoloAnimation
    /// 
    [AddComponentMenu(Strings.MenuPrefix + "Solo Animation")]
    [DefaultExecutionOrder(DefaultExecutionOrder)]
    [HelpURL(Strings.DocsURLs.APIDocumentation + "/" + nameof(SoloAnimation))]
    public class SoloAnimation : SoloAnimationInternal { }
}
