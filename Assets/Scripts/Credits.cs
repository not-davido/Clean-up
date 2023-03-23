using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    /// <summary>
    /// The URL that credits the author.
    /// </summary>
    /// <param name="url">The webpage to redirect to.</param>
    public void URL(string url) => Application.OpenURL(url);
}
