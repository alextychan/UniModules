﻿namespace UniGreenModules.UniStateMachine.Runtime.Interfaces
{
    using UniCore.Runtime.Interfaces;
    using UniCore.Runtime.ObjectPool.Runtime.Interfaces;

    public interface IContextState<out TAwaiter> : 
        IStateBehaviour<IContext, TAwaiter>,
        ILifeTimeContext,
        IPoolable
    {
        bool IsActive { get; }
    }
}