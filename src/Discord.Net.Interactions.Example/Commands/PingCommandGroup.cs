using System.Threading;
using System.Threading.Tasks;
using Discord.Net.Interactions.Abstractions;
using Discord.Net.Interactions.CommandsInfo;
using Discord.Net.Interactions.Executors;
using Discord.Net.Interactions.HandlerCreator;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Discord.Net.Interactions.Example.Commands
{
    public class PingCommandGroup : ICommandGroup
    {
        public enum Feedback : long // Important to inherit from long as discord.net returns long when calling command
        {
            Feedback1,
            Feedback2,
            Feedback3
        };
        
        private readonly ILogger _logger;

        public PingCommandGroup(ILogger<PingCommandGroup> logger)
        {
            _logger = logger;
        }

        private Task HandlePing(SocketInteraction interaction, IMentionable mentionable, CancellationToken cancellationToken)
        {
            return interaction
                .RespondAsync($"Pong {mentionable.Mention}");
        }

        private async Task HandleFeedbackPositive(SocketInteraction interaction, Feedback feedback, CancellationToken cancellationToken)
        {
            await interaction.RespondAsync("Thank you for your positive feedback!", ephemeral: true);
            await interaction.FollowupAsync($"User {interaction.GetUser()} left a feedback {feedback}");
        }
        
        private async Task HandleFeedbackNegative(SocketInteraction interaction, Feedback feedback, CancellationToken cancellationToken)
        {
            await interaction.RespondAsync("Thank you for your negative feedback!", ephemeral: true);
            await interaction.FollowupAsync($"User {interaction.GetUser()} left a feedback {feedback}");
        }
        
        public Task SetupCommandsAsync(IInteractionHolder holder, CancellationToken token = new CancellationToken())
        {
            // Create handler for ping command calling correctly HandlePing method
            DiscordInteractionHandler pingHandler = new PlainCommandHandlerCreator()
                .CreateHandlerForCommand((CommandDelegate<IMentionable>)HandlePing);

            // Create handler for /feedback positive/negative sub command
            // Calling HandleFeedbackPositive for /feedback positive
            // and HandleFeedbackNegative for /feedback negative
            DiscordInteractionHandler feedbackHandler = new SubCommandHandlerCreator()
                .CreateHandlerForCommand(
                    ("positive", (CommandDelegate<Feedback>)HandleFeedbackPositive),
                    ("negative", (CommandDelegate<Feedback>)HandleFeedbackNegative));
            
            SlashCommandInfo pingInfo = new SlashCommandInfoBuilder()
                .WithHandler(pingHandler)
                .WithBuilder(new SlashCommandBuilder()
                    .WithName("ping")
                    .WithDescription("Send pong along with mentioning specified user")
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithType(ApplicationCommandOptionType.Mentionable)
                        .WithName("mentionable") // the name must match the name of the parameter
                        .WithDescription("Who to mention")
                        .WithRequired(true)))
                .SetGlobal(true) // Make ping global
                .Build();
            
            SlashCommandInfo feedbackInfo = new SlashCommandInfoBuilder()
                .WithHandler(feedbackHandler)
                .WithBuilder(new SlashCommandBuilder()
                    .WithName("feedback")
                    .WithDescription("Send feedback to the developers")
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .WithName("positive")
                        .WithDescription("Leave positive feedback")
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithType(ApplicationCommandOptionType.Integer)
                            .WithName("feedback")
                            .WithDescription("The feedback")
                            .WithRequired(true)
                            .AddChoice("Feedback1", (int)Feedback.Feedback1)
                            .AddChoice("Feedback2", (int)Feedback.Feedback2)
                            .AddChoice("Feedback3", (int)Feedback.Feedback3)
                        ))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .WithName("negative")
                        .WithDescription("Leave negative feedback")
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithType(ApplicationCommandOptionType.Integer)
                            .WithName("feedback")
                            .WithDescription("The feedback")
                            .WithRequired(true)
                            .AddChoice("Feedback1", (int)Feedback.Feedback1)
                            .AddChoice("Feedback2", (int)Feedback.Feedback2)
                            .AddChoice("Feedback3", (int)Feedback.Feedback3)
                        )))
                .SetGlobal(false)
                .Build();
            
            // Create executor along with logging in case of an error
            IInteractionExecutor executor = new InteractionExecutorBuilder()
                .WithLogger(_logger)
                .Build();

            holder.AddInteraction(pingInfo, executor);
            holder.AddInteraction(feedbackInfo, executor);
            return Task.CompletedTask;
        }
    }
}