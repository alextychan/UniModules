﻿namespace UniGreenModules.UniNodeSystem.Runtime
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Extensions;
    using Interfaces;
    using Runtime;
    using UniCore.Runtime.DataFlow;
    using UniCore.Runtime.Extension;
    using UniCore.Runtime.Interfaces;
    using UniCore.Runtime.ObjectPool;
    using UniCore.Runtime.ObjectPool.Extensions;
    using UniStateMachine.Runtime;
    using UniStateMachine.Runtime.Interfaces;
    using UniTools.UniRoutine.Runtime;
    using UnityEngine;

    [Serializable]
    public abstract class UniFlowNode : UniNode
    {
        /// <summary>
        /// output port name
        /// </summary>
        public const string OutputPortName = "Output";
        
        /// <summary>
        /// input port name
        /// </summary>
        public const string InputPortName = "Input";
        
        #region serialized data

        [SerializeField] private RoutineType routineType = RoutineType.UpdateStep;

        #endregion

        #region private fields

        [NonSerialized] private Dictionary<string,UniPortValue> portValuesMap;

        [NonSerialized] private IContextState<IEnumerator> behaviourState;

        [NonSerialized] private List<UniPortValue> portValues;

        [NonSerialized] private bool isInitialized;
        
        [NonSerialized] protected IContext nodeContext;

        #endregion

        #region public properties
        
        public IPortValue Input => GetPortValue(InputPortName);

        public IPortValue Output => GetPortValue(OutputPortName);
        
        public RoutineType RoutineType => routineType;

        #endregion
        
        #region public methods
        
        public void Exit() => behaviourState?.Exit();

        public IEnumerator Execute(IContext context)
        {
            StateLogger.LogState(string.Format("STATE EXECUTE {0} TYPE {1} CONTEXT {2}", 
                name, GetType().Name, context), this);
            
            Initialize();
            
            behaviourState = CreateState();
            
            yield return behaviourState.Execute(context);
            
        }
        
        public void Release() => Exit();

        #region Node Ports operations
 
        public override object GetValue(NodePort port) => GetPortValue(port);
        
        public UniPortValue GetPortValue(NodePort port) => GetPortValue(port.fieldName);

        public UniPortValue GetPortValue(string portName)
        {
            portValuesMap.TryGetValue(portName, out var value);
            return value;
        }

        public bool AddPortValue(UniPortValue portValue)
        {
            if (portValue == null)
            {
                Debug.LogErrorFormat("Try add NULL port value to {0}",this);
                return false;
            }
            
            if (portValuesMap.ContainsKey(portValue.name))
            {
                return false;
            }

            portValuesMap[portValue.name] = portValue;
            portValues.Add(portValue);
            
            return true;
        }

        #endregion

        #endregion

        private bool IsExistsPort(NodePort port)
        {
            if (port.IsStatic) return false;
            var value = GetPortValue(port.fieldName);
            return value == null;
        }
        
        protected virtual void OnUpdatePortsCache()
        {
            CreateBasePorts();
        }

        protected virtual void OnNodeInitialize(){}
        
        #region state behaviour methods

        protected void CreateBasePorts()
        {
            this.UpdatePortValue(OutputPortName, PortIO.Output);
            this.UpdatePortValue(InputPortName, PortIO.Input);
        }
        
        private void Initialize(IContext stateContext)
        {
            nodeContext = stateContext;
            
            LifeTime.AddCleanUpAction(CleanUpAction);
            
            OnInitialize(stateContext);
        }

        /// <summary>
        /// base logic realization
        /// transfer context data to output port value
        /// </summary>
        protected virtual IEnumerator OnExecuteState(IContext context)
        {
            var output = Output;
            output.Add(context);
            yield break;
        }

        protected virtual void OnExit(IContext context){}

        protected virtual void OnInitialize(IContext context){}
        
        protected virtual void OnPostExecute(IContext context){}

        protected IContextState<IEnumerator> CreateState()
        {
            if (behaviourState != null)
                return behaviourState;
            
            var behaviour = ClassPool.Spawn<ProxyState>();
            behaviour.Initialize(OnExecuteState, Initialize, OnExit, OnPostExecute);
            
            behaviour.LifeTime.AddCleanUpAction(() => behaviour.Despawn());
            behaviour.LifeTime.AddCleanUpAction(CleanUpAction);
            
            return behaviour;
        }

        private void CleanUpAction()
        {
            for (var i = 0; i < PortValues.Count; i++)
            {
                var portValue = PortValues[i];
                portValue.CleanUp();
            }

            nodeContext = null;
            behaviourState = null;
        }

#endregion

    }
}