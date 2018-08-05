using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

namespace Framework
{
    public class SceneLoader : MonoBehaviour
    {
        public event Action<float> LoadProgress;

        public string LoadingScene { get; set; }

        public IEnumerator Load(string[] targetUnloadScenes, string[] targetLoadScenes, Action onLoadComplete)
        {
            if (string.IsNullOrEmpty(LoadingScene))
            {
                DebugEx.LogWarning<SceneLoader>("No scene is set to load during the load transition!");
            }
            else
            {
                yield return SceneManager.LoadSceneAsync(LoadingScene, LoadSceneMode.Additive);
            }

            for (int i = 0; i < targetUnloadScenes.Length; i++)
            {
                yield return SceneManager.UnloadSceneAsync(targetUnloadScenes[i]);
            }

            int scenesLoaded = 0;
            float totalProgress = 0f;
            foreach (var scene in targetLoadScenes)
            {
                var task = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

                while (!task.isDone)
                {
                    totalProgress = (scenesLoaded + task.progress) / targetLoadScenes.Length;
                    LoadProgress.InvokeSafe(totalProgress);
                    yield return null;
                }

                scenesLoaded++;
            }

            if (!string.IsNullOrEmpty(LoadingScene))
                yield return SceneManager.UnloadSceneAsync(LoadingScene);

            // We're going to assume that the last scene is what we want to be active...
            var activeScene = SceneManager.GetSceneByName(targetLoadScenes[targetLoadScenes.Length - 1]);
            SceneManager.SetActiveScene(activeScene);

            // Wait until Unity has finished loading in lightmaps. Bit of a hack, tbh.
            yield return new WaitForSeconds(0.5f);

            onLoadComplete.InvokeSafe();
        }
    }
}
