using System.Threading.Tasks;
using Discord.Net.Interactions.Controllers;
using Discord.Net.Interactions.Controllers.Attributes;

namespace Discord.Net.Interactions.Example.Controllers.Controllers
{
    public class ControlController : InteractionController
    {
        private readonly StopBot _stopBot;
        
        public ControlController(StopBot stopBot)
        {
            _stopBot = stopBot;
        }
        
        [SlashCommand("quit", Description = "Exit the bot")]
        [AutoDefer]
        public Task HandleQuit()
        {
            _stopBot.Invoke();
            return Context.Interaction.FollowupAsync("Quitting!");
        }
    }
}