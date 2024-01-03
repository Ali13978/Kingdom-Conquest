using System;
using System.Collections.Generic;
using System.Collections;

namespace TinyWar
{
    /// <summary>
    /// UITweener顺序类
    /// </summary>
    public class TweenSequence
    {
        private IList<UITweener> _list;
        System.Action _callback;

        public TweenSequence(IList<UITweener> tweeners) {
            this._list = tweeners;
        }

        public void Run() {
            this._list[0].Play(true);
        }

        private void _finish(UITweener tween) {
            if (this._callback != null) {
                this._callback.Invoke();
            }
        }
    }
}
