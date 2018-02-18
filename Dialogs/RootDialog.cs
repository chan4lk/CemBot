namespace BasicMultiDialogBot.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    #pragma warning disable 1998

    [Serializable]
    public class RootDialog : IDialog<object>
    {

        private string name;
        

        public async Task StartAsync(IDialogContext context)
        {
            /* Wait until the first message is received from the conversation and call MessageReceviedAsync 
             *  to process that message. */
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            /* When MessageReceivedAsync is called, it's passed an IAwaitable<IMessageActivity>. To get the message,
             *  await the result. */
            var message = await result;

            await this.SendWelcomeMessageAsync(context);
        }

        private async Task SendWelcomeMessageAsync(IDialogContext context)
        {
            await context.SayAsync("Hi, I'm the CEM bot. Let's get started.", speak: "Hi, I'm the CEM bot. Let's get started.");

            context.Call(new NameDialog(), this.NameDialogResumeAfter);
        }

        private async Task NameDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                this.name = await result;

                context.Call(new QuestionDialog(this.name), this.QuestionDialogResumeAfter);
            }
            catch (TooManyAttemptsException)
            {
                await context.SayAsync("I'm sorry, I'm having issues understanding you. Let's try again.", speak:"I'm sorry, I'm having issues understanding you.Let's try again.");

                await this.SendWelcomeMessageAsync(context);
            }
        }

        private async Task QuestionDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var message = await result;
                
                await context.SayAsync($"Your name is { name } and { message }.", speak: $"Your name is { name } and { message }.");

            }
            catch (TooManyAttemptsException)
            {
                await context.SayAsync("I'm sorry, I'm having issues understanding you. Let's try again.", speak: "I'm sorry, I'm having issues understanding you. Let's try again.");
            }
            finally
            {
                await this.SendWelcomeMessageAsync(context);
            }
        }
    }
}