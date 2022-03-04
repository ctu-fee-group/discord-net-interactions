using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Discord.Net.Interactions.Controllers;
using Discord.Net.Interactions.Controllers.Attributes;

namespace Discord.Net.Interactions.Example.Controllers.Controllers
{
    [SlashGroup("feedback", Description = "Leave positive or negative feedback")]
    [AutoDefer]
    [ThreadPool]
    public class FeedbackController : InteractionController
    {
        public enum Feedback : long
        {
            [EnumValue("Feedback 1")]
            Feedback1,
            [EnumValue("Feedback 2")]
            Feedback2,
            [EnumValue("Feedback 3")]
            Feedback3
        }
        
        [SlashCommand("positive", Description = "Leave positive feedback")]
        public Task HandlePositive([Description("What feedback to leave")] Feedback feedback)
        {
            return Context.Interaction.FollowupAsync($"Thanks for the feedback {feedback}");
        }
        
        [SlashCommand("negative", Description = "Leave negative feedback")]
        public Task HandleNegative([Description("What feedback to leave")] Feedback feedback)
        {
            return Context.Interaction.FollowupAsync($"Thanks for the feedback {feedback}");
        }
    }
}