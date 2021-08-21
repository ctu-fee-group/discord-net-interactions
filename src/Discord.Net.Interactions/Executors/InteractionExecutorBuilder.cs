using System;
using Discord.Net.Interactions.Abstractions;
using Microsoft.Extensions.Logging;

namespace Discord.Net.Interactions.Executors
{
    public sealed class InteractionExecutorBuilder
        : InteractionExecutorBuilder<InteractionExecutorBuilder, SlashCommandInfo>
    { }
    
    public sealed class InteractionExecutorBuilder<TInteractionInfo>
        : InteractionExecutorBuilder<InteractionExecutorBuilder<TInteractionInfo>, TInteractionInfo>
        where TInteractionInfo : InteractionInfo
    { }

    public abstract class InteractionExecutorBuilder<TBuilder, TInteractionInfo>
        where TBuilder : InteractionExecutorBuilder<TBuilder, TInteractionInfo>
        where TInteractionInfo : InteractionInfo
    {
        private readonly TBuilder _builderInstance;
        private bool _defer, _threadPool;

        private IInteractionExecutor? _base;
        private ICommandPermissionsResolver<TInteractionInfo>? _commandPermissionsResolver;
        private string? _deferMessage;
        private ILogger? _logger;

        protected InteractionExecutorBuilder()
        {
            _builderInstance = (TBuilder)this;
        }

        /// <summary>
        /// Add AutoDeferCommandExecutor decorator
        /// </summary>
        /// <param name="message">Message to respond with, if null, defer will be calle</param>
        /// <returns>this</returns>
        public TBuilder WithDeferMessage(string? message = null)
        {
            _defer = true;
            _deferMessage = message;
            return _builderInstance;
        }
        
        /// <summary>
        /// Logger to initialize ThreadPoolCommandExecutor or base executor
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public TBuilder WithLogger(ILogger logger)
        {
            _logger = logger;
            return _builderInstance;
        }

        /// <summary>
        /// Sets the underlying executor that will be decorated
        /// </summary>
        /// <param name="executor"></param>
        /// <returns></returns>
        public TBuilder SetBaseExecutor(IInteractionExecutor executor)
        {
            _base = executor;
            return _builderInstance;
        }
        
        /// <summary>
        /// Adds permission check
        /// </summary>
        /// <param name="commandPermissionsResolver">Permission resolver</param>
        /// <returns></returns>
        public TBuilder WithPermissionCheck(ICommandPermissionsResolver<TInteractionInfo> commandPermissionsResolver)
        {
            _commandPermissionsResolver = commandPermissionsResolver;
            return _builderInstance;
        }

        /// <summary>
        /// Add ThreadPoolCommandExecutor decorator
        /// </summary>
        /// <returns></returns>
        public TBuilder WithThreadPool()
        {
            _threadPool = true;
            return _builderInstance;
        }

        /// <summary>
        /// Creates ICommandExecutor based on configuration of builder
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual IInteractionExecutor Build()
        {
            if (_base is null)
            {
                if (_logger is null)
                {
                    throw new InvalidCastException("Logger must not be null");
                }
                
                _base = new HandlerInteractionExecutor(_logger);
            }

            IInteractionExecutor executor = _base;

            if (_threadPool)
            {
                if (_logger is null)
                {
                    throw new InvalidOperationException("Logger must not be null");
                }
                
                executor = new ThreadPoolInteractionExecutor(_logger, executor);
            }

            if (_commandPermissionsResolver != null)
            {
                if (_logger is null)
                {
                    throw new InvalidOperationException("Logger must not be null");
                }
                
                executor = new PermissionCheckInteractionExecutor<TInteractionInfo>(_logger, _commandPermissionsResolver, executor);
            }

            if (_defer)
            {
                executor = new AutoDeferInteractionExecutor(executor, _deferMessage);
            }

            return executor;
        }
    }
}