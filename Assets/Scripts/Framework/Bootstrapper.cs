using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;
using System.Collections;

namespace Framework
{
    /// <summary>
    /// Bootstraps the game. 
    /// When placed in a level, it will automatically initialize the game then return to the current level.
    /// </summary>
    class Bootstrapper : MonoBehaviour
    {
        private const string _rootSceneName = "Main";
        private string _startingSceneName;

        public void Start()
        {
            _startingSceneName = SceneManager.GetActiveScene().name;

            //StartCoroutine(Initialize());

            var game = FindObjectOfType<Game>();
            if (game == null)
            {
                // We're not in the root scene that contains the Game, so we'll need to initialize.
                StartCoroutine(Initialize());
            }
            else if (!game.Initialized)
            {
                DebugEx.Log<Bootstrapper>("Bootstrap complete.");

                // We must be in the root scene. So let's initialize the game.
                game.Initialize(_startingSceneName);
            }
        }

        IEnumerator Initialize()
        {
            // NB: There is no good way to know if the root scene is in the build settings.
            // So...if it's not in the build, the runtime will error on the next line.

            yield return SceneManager.LoadSceneAsync(_rootSceneName, LoadSceneMode.Additive);

            // Look for an existing bootstrapper in the freshly loaded scene. Destroy it, if one is found.
            var existingBootstrappers = FindObjectsOfType<Bootstrapper>();
            foreach (var bootstrapper in existingBootstrappers)
            {
                if (bootstrapper != this)
                {
                    Debug.Log("Found an existing bootstrapper in root scene during initialization. Removing it...");
                    Destroy(bootstrapper);
                }
            }

            // Move Bootstrapper to root.

            var rootScene = SceneManager.GetSceneByName(_rootSceneName);

            SceneManager.MoveGameObjectToScene(gameObject, rootScene);

            // Unload the scene the bootstrapper is in.
            yield return SceneManager.UnloadSceneAsync(_startingSceneName);

            // Initialize game passing in the level name as an argument.
            var game = FindObjectOfType<Game>();
            if (game == null)
            {
                Debug.LogError("Failed to find a Game component. The scene 'Main' should contain a Game component.");
            }
            else
            {
                DebugEx.Log<Bootstrapper>("Bootstrap complete.");

                game.Initialize(_startingSceneName);
            }
        }
    }
}
