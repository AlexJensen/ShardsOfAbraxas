using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Abraxas.VFX
{
    public class AnimationClipPlayer : MonoBehaviour
    {
        [SerializeField]
        private GameObject _target;
        [SerializeField]
        private Image _image;

        private PlayableGraph playableGraph;

        public IEnumerator PlayAnimationAndWait(AnimationClip clip, Color color, bool flip)
        {
            if (clip == null)
            {
                Debug.LogError("Invalid animation clip.");
                yield break;
            }

            _target.SetActive(true);
            if (flip)
            {
                _image.transform.localScale = new Vector3(-1, 1, 1);
            }
            _image.color = color;

            // Create the PlayableGraph
            playableGraph = PlayableGraph.Create();

            // Create the AnimationPlayableOutput
            var output = AnimationPlayableOutput.Create(playableGraph, "Animation", _target.GetComponent<Animator>());

            // Create the AnimationClipPlayable
            var playableClip = AnimationClipPlayable.Create(playableGraph, clip);

            // Connect the playable to the output
            output.SetSourcePlayable(playableClip);

            // Play the graph
            playableGraph.Play();

            // Wait for the duration of the clip
            yield return new WaitForSeconds(clip.length);

            // Stop and destroy the graph
            playableGraph.Stop();
            playableGraph.Destroy();

            _target.SetActive(false);
        }

        private void OnDisable()
        {
            // Ensure the graph is destroyed when the object is disabled
            if (playableGraph.IsValid())
            {
                playableGraph.Destroy();
            }
        }
    }
}