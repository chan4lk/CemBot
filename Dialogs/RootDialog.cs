namespace BasicMultiDialogBot.Dialogs
{
    using System;
    using System.Configuration;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

#pragma warning disable 1998

    [Serializable]
    public class RootDialog : IDialog<object>
    {

        private string name;
        private bool isChiefGuest = false;
        private string[] questions =
        {
            "What do you think about CEM stall?",
            "What is the rating you would give for our stall out of 10?",
            "Will you recommend our stall to your friends as well?"
        };

        private string[] chiefGuestQuestion =
        {
        "What do you think about CEM stall?",
            "What is the rating you would give for our stall out of 10?",
            "Will you recommend our stall to your friends as well?"
        };

        public string[] Questions
        {
            get
            {
                if (isChiefGuest == true)
                    return chiefGuestQuestion;
                return questions;
            }
        }

        private int questionCount = 0;
        private static string botName = ConfigurationManager.AppSettings["BotName"];
        private static string cheifGuestName = ConfigurationManager.AppSettings["CheifGuestName"];
        private string retryMessage = "I'm sorry, I'm having issues understanding you.Let's try again.";

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
            await context.SayAsync($"Hi, I'm {botName}. Let's get started.");

            context.Call(new NameDialog(), this.NameDialogResumeAfter);
        }

        private async Task NameDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                this.name = await result;
                isChiefGuest = (name == cheifGuestName);
                context.Call(new GreetingDialog(this.name), this.GreetingDialogResumeAfter);
            }
            catch (TooManyAttemptsException)
            {
                await context.SayAsync(this.retryMessage);

                await this.SendWelcomeMessageAsync(context);
            }
        }

        private async Task GreetingDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                questionCount = 0;
                context.Call(new QuestionDialog2(this.name, this.Questions[questionCount++]), this.QuestionDialogResumeAfter);
            }
            catch (TooManyAttemptsException)
            {
                await context.SayAsync(this.retryMessage);

                await this.SendWelcomeMessageAsync(context);
            }
        }

        private async Task QuestionDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                if (questionCount == this.Questions.Length - 2)
                {
                    context.Call(new QuestionDialog2(this.name, this.Questions[questionCount++]), this.FinallyGreet);
                }
                else
                {
                    context.Call(new QuestionDialog2(this.name, this.Questions[questionCount++]), this.QuestionDialogResumeAfter);
                }
            }
            catch (TooManyAttemptsException)
            {
                await context.SayAsync(retryMessage);

                await this.SendWelcomeMessageAsync(context);
            }
        }

        private async Task FinallyGreet(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var message = await result;

                await context.SayAsync($"Thank you for visiting CEM stall {name} Please enjoy.");

            }
            catch (TooManyAttemptsException)
            {
                await context.SayAsync(retryMessage);
            }
            finally
            {
                context.EndConversation("end");
            }
        }
    }
}