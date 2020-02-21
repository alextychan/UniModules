﻿namespace UniGreenModules.UniUiSystem.Runtime
{
    using System.Collections;
    using Interfaces;
    using UniCore.Runtime.Interfaces;
    using UniCore.Runtime.ModelBehaviours;
    using UniCore.Runtime.Rx.Extensions;
    using UniRoutine.Runtime;
    using UnityEngine;

    public class UiView<TModel> : 
        ScheduledViewModel<TModel>, 
        IUiView<TModel>
    {
        [SerializeField]
        private RoutineType updateType = RoutineType.EndOfFrame;
        [SerializeField]
        private bool immediateUpdate = false;

        public RectTransform RectTransform  => transform as RectTransform;

        #region private methods

        //schedule single ui update at next EndOfFrame call
        protected override IDisposableItem ScheduleUpdate()
        {
            return OnScheduledUpdate().
                ExecuteRoutine(updateType,immediateUpdate);
        }

        private IEnumerator OnScheduledUpdate()
        {
            //update ui view
            yield return OnUpdateView();
            
            //cancel disposable item
            updateDisposable.Cancel();
            updateDisposable = null;
        }

        protected virtual IEnumerator OnUpdateView()
        {
            yield break;
        }

        #endregion

    }
}