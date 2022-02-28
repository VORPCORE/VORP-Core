﻿namespace Vorp.Core.Client.Interface
{
    internal class PromptHandler
    {
        static List<Prompt> _prompts = new();

        public static async Task OnHandlePrompt()
        {
            List<Prompt> prompts = new(_prompts);

            if (prompts.Count == 0)
            {
                await BaseScript.Delay(1000);
            }

            foreach(Prompt prompt in prompts)
            {
                if (!prompt.IsActive) continue;
                if (!prompt.Visible) continue;

                if (prompt.HasHoldMode && prompt.Type == ePromptType.StandardHold)
                {
                    while (prompt.IsHoldModeRunning)
                    {
                        await BaseScript.Delay(0);
                        if (prompt.HasHoldModeCompleted)
                        {
                            prompt.TriggerEvent();
                        }
                    }
                }
                else
                {
                    switch(prompt.Type)
                    {
                        case ePromptType.JustPressed:
                            if (prompt.IsJustPressed)
                                prompt.TriggerEvent();
                            break;
                        case ePromptType.Pressed:
                            if (prompt.IsPressed)
                                prompt.TriggerEvent();
                            break;
                        case ePromptType.JustReleased:
                            if (prompt.IsJustReleased)
                                prompt.TriggerEvent();
                            break;
                        case ePromptType.Released:
                            if (prompt.IsReleased)
                                prompt.TriggerEvent();
                            break;
                    }
                }
            }
        }

        public static void Add(Prompt prompt) => _prompts.Add(prompt);
        public static void Remove(Prompt prompt) => _prompts.Remove(prompt);
    }
}