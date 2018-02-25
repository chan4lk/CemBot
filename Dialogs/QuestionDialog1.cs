namespace BasicMultiDialogBot.Dialogs
{
    using Microsoft.Bot.Builder.Dialogs;
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class QuestionDialog1 : IDialog<string>
    {
        private string name;
        private int attempts = 3;

        public QuestionDialog1(string name)
        {
            this.name = name;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.SayAsync($"{ this.name }, Would you like to play a game?", speak: $"{ this.name }, Would you like to play a game ? ");


            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            var answer = message.Text.ToUpper();

            if (answer == "YES")
            {
                context.Done("You are ready to Play a Game");
            }
            else if (answer == "NO")
            {

                context.Done("You are not ready to Play a Game");
            }
            else
            {
                --attempts;
                if (attempts > 0)
                {
                    await context.SayAsync("I'm sorry, I don't understand your reply. Would you like to play a game(yes or no)?", speak: "I'm sorry, I don't understand your reply. Would you like to play a game(yes or no)?");

                    context.Wait(this.MessageReceivedAsync);
                }
                else
                {
                    context.Fail(new TooManyAttemptsException("Message was not a valid age."));
                }
            }
        }
    }
}