﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Tools.UniRoutineTask {
    
    public class UniRoutineTask : IEnumerator<IEnumerator> {
        private Stack<IEnumerator> _awaiters = new Stack<IEnumerator>();

        public void Initialize(IEnumerator enumerator) {
            _awaiters.Clear();
            Current = enumerator;
        }

        public bool MoveNext() {
            if (Current == null) {
                Dispose();
                return false;
            }

            var moveNext = Current.MoveNext();

            //if current enumerator motion finished try get next one from stack
            if (!moveNext) {
                if (_awaiters.Count == 0)
                    return false;
                Current = _awaiters.Pop();
                return true;
            }

            var awaiter = Current.Current as IEnumerator;
            if (awaiter != null) {
                _awaiters.Push(Current);
                Current = awaiter;
            }

            return true;
        }

        public void Reset() {
        }

        public IEnumerator Current { get; protected set; }

        object IEnumerator.Current => Current;

        public void Dispose() {
            _awaiters.Clear();
        }
    }
    
}