using UnityEngine;

public class AnimationCall : StateMachineBehaviour
{
    public delegate void AnimationCallback();
    AnimationCallback _callback;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _callback?.Invoke();
        _callback -= _callback;
    }
    
    public void SetCallback(AnimationCallback callback)
    {
        _callback = callback;
    }
}
