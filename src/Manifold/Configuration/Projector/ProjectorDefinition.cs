using System;
using System.Collections.Generic;
using Manifold.Configuration.Projector.Operations;
using Manifold.DependencyInjection;

namespace Manifold.Configuration.Projector
{
    public class ProjectorDefinition<TInput, TOutput> : IProjectorDefinition<TInput, TOutput>
    {
        private readonly IList<IProjectorOperation<TInput, TOutput>> _operations = new List<IProjectorOperation<TInput,TOutput>>();
        private readonly IList<Action<IRegisterTypes>> _registrationActions;

        public IEnumerable<IProjectorOperation<TInput, TOutput>> Operations
        {
            get { return _operations; }
        }

        public ProjectorDefinition(IList<Action<IRegisterTypes>> registrationActions)
        {
            _registrationActions = registrationActions;
        }

        public void AddFunctionOperation(Func<TInput, IEnumerable<TOutput>> function)
        {
            _operations.Add(new FunctionOperation<TInput, TOutput>(function));
        }

        public void AddInjectedOperation<TType>() where TType : class, IPipelineTask<TInput, IEnumerable<TOutput>>
        {
            _operations.Add(new InjectedProjectorOperation<TType, TInput, TOutput>());
            _registrationActions.Add(ctx => ctx.RegisterType<TType>());
        }
    }
}