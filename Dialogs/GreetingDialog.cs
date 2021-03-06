﻿namespace BasicMultiDialogBot.Dialogs
{
    using Microsoft.Bot.Builder.Dialogs;
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class GreetingDialog : IDialog<string>
    {
        private int attempts = 3;
        private string name = string.Empty;
        private string message = string.Empty;
        private bool isCheifGuest = false;

        public GreetingDialog(string name, bool isCheifGuest)
        {
            this.name = name;
            this.isCheifGuest = isCheifGuest;
        }

        public async Task StartAsync(IDialogContext context)
        {
            string message = string.Empty;
            if (isCheifGuest)
                message = $"Hello {name} welcome to Virtusa Tech day 2018. How do you do?"; ;

            message = $"Hello {name} How do you do?";

            await context.SayAsync(message);

            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            /* If the message returned is a valid name, return it to the calling dialog. */
            if ((message.Text != null) && (message.Text.Trim().Length > 0))
            {
                /* Completes the dialog, removes it from the dialog stack, and returns the result to the parent/calling
                    dialog. */
                context.Done(message.Text);
            }
            /* Else, try again by re-prompting the user. */
            else
            {
                --attempts;
                if (attempts > 0)
                {
                    await context.SayAsync($"I'm sorry, I cannot understand you. {message}");

                    context.Wait(this.MessageReceivedAsync);
                }
                else
                {
                    /* Fails the current dialog, removes it from the dialog stack, and returns the exception to the 
                        parent/calling dialog. */
                    context.Fail(new TooManyAttemptsException("Message was not a string or was an empty string."));
                }
            }
        }
    }
}