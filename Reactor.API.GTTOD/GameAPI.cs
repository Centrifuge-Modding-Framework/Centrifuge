using CommandTerminal;
using System;
using System.Collections;
using UnityEngine;

namespace Reactor.API.GTTOD
{
    public class GameAPI : MonoBehaviour
    {
        public static event EventHandler TerminalInitialized;

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void Start()
        {
            StartCoroutine(WaitForTerminal());
        }

        IEnumerator WaitForTerminal()
        {
            yield return new WaitUntil(() => FindObjectOfType<Terminal>());
            TerminalInitialized?.Invoke(this, EventArgs.Empty);
        }
    }
}
