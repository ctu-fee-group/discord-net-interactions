using System;
using Discord.Net.Interactions.Abstractions;
using Microsoft.Extensions.Logging;

namespace Discord.Net.Interactions.Executors
{
    public sealed class CommandExecutorBuilder
        : CommandExecutorBuilder<CommandExecutorBuilder> { }

    public abstract class CommandExecutorBuilder<TBuilder>
        where TBuilder : CommandExecutorBuilder<TBuilder>
    {
        private readonly TBuilder _builderInstance;
        private bool _defer, _threadPool;

        private ICommandExecutor? _base;
        private ICommandPermissionsResolver<SlashCommandInfo>? _commandPermissionsResolver;
        private string? _deferMessage;
        private ILogger? _logger;

        protected CommandExecutorBuilder()
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
        public TBuilder SetBaseExecutor(ICommandExecutor executor)
        {
            _base = executor;
            return _builderInstance;
        }
        
        /// <summary>
        /// Adds permission check
        /// </summary>
        /// <param name="commandPermissionsResolver">Permission resolver</param>
        /// <returns></returns>
        public TBuilder WithPermissionCheck(ICommandPermissionsResolver<SlashCommandInfo> commandPermissionsResolver)
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
        public virtual ICommandExecutor Build()
        {
            if (_base is null)
            {
                if (_logger is null)
                {
                    throw new InvalidCastException("Logger must not be null");
                }
                
                _base = new HandlerCommandExecutor(_logger);
            }

            ICommandExecutor executor = _base;

            if (_threadPool)
            {
                if (_logger is null)
                {
                    throw new InvalidOperationException("Logger must not be null");
                }
                
                executor = new ThreadPoolCommandExecutor(_logger, executor);
            }

            if (_commandPermissionsResolver != null)
            {
                if (_logger is null)
                {
                    throw new InvalidOperationException("Logger must not be null");
                }
                
                executor = new PermissionCheckCommandExecutor(_logger, _commandPermissionsResolver, executor);
            }

            if (_defer)
            {
                executor = new AutoDeferCommandExecutor(executor, _deferMessage);
            }

            return executor;
        }
    }
}