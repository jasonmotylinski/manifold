﻿using System;
using System.Collections.Generic;
using System.Linq;
using Wormhole.PipeAndFilter;
using Wormhole.Pipeline.Configuration;
using PipelineCompiler = Wormhole.PipeAndFilter.PipelineCompiler;


namespace Wormhole.DependencyInjection
{
    public interface IPipelineAggregator
    {
        IDictionary<PipelineKey, Func<IResolveTypes, object, object>> Compile(IRegisterTypes typeRegistrar);
        PipeAndFilter.PipelineConfigurator<TInput, TOutput> CreatePipeline<TType, TInput, TOutput>(TType name);
        PipeAndFilter.PipelineConfigurator<TInput, TOutput> CreatePipeline<TInput, TOutput>();
    }

    public class PipelineAggregator<TResolver> : IPipelineAggregator where TResolver : IResolveTypes
    {
        public PipelineAggregator()
        {
            _registrationActions.Add(a => a.RegisterType<TResolver, IResolveTypes>());
        }
        public IDictionary<PipelineKey, Func<IResolveTypes, object, object>> Compile(IRegisterTypes typeRegistrar)
        {
            var dictionary = _aggregatePipelines.ToDictionary(value => value.Key, value => value.Value.Compile());
            typeRegistrar.RegisterInstance(dictionary);

            foreach (var item in _registrationActions)
                item(typeRegistrar);

            return dictionary;
        }

        private readonly IDictionary<PipelineKey, IPipeCompiler> _aggregatePipelines =
            new Dictionary<PipelineKey, IPipeCompiler>();

        private readonly IList<Action<IRegisterTypes>> _registrationActions = new List<Action<IRegisterTypes>>();



        public PipeAndFilter.PipelineConfigurator<TInput,TOutput> CreatePipeline<TType, TInput, TOutput>(TType name) 
        {
            var definition = new PipelineDefinition(_registrationActions);

            _aggregatePipelines.Add(new PipelineKey
                                        {
                                            Input = typeof (TInput),
                                            Output = typeof (TOutput),
                                            Named = name
                                        }, new PipelineCompiler(definition));


            return new PipeAndFilter.PipelineConfigurator<TInput, TOutput>(definition);
        }

        public PipeAndFilter.PipelineConfigurator<TInput, TOutput> CreatePipeline<TInput, TOutput>() 
        {
            var definition = new PipelineDefinition(_registrationActions);
            var compiler = new PipelineCompiler(definition);

            _aggregatePipelines.Add(new PipelineKey
            {
                Input = typeof(TInput),
                Output = typeof(TOutput),
                Named = new DefaultPipeline<TInput, TOutput>()
            }, compiler);

            _registrationActions.Add(a => a.Register<Pipe<TInput, TOutput>>(ctx =>
                                                                            input => (TOutput) compiler.Compile()(ctx, input)));


            

            return new PipeAndFilter.PipelineConfigurator<TInput, TOutput>(definition);
        }

    }

}
