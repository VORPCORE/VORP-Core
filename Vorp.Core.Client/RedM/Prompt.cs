using Vorp.Core.Client.RedM.Enums;

namespace Vorp.Core.Client.RedM
{
    internal class Prompt : PoolObject
    {
        bool _visible = true;

        public Prompt(int handle) : base(handle)
        {

        }

        public static Prompt Create(eControl control, string label)
        {
            int promptHandle = PromptRegisterBegin();

            PromptSetControlAction(promptHandle, (int)control);
            long str = Function.Call<long>(Hash._CREATE_VAR_STRING, 10, "LITERAL_STRING", label);
            PromptSetText(promptHandle, (int)str);

            API.PromptRegisterEnd(promptHandle);

            return new Prompt(promptHandle);
        }

        public override bool Exists() => PromptIsActive(Handle);
        public override void Delete() => PromptDelete(Handle);

        public bool Enabled
        {
            get => PromptIsEnabled(Handle) == 1;
            set => PromptSetEnabled(Handle, value ? 1 : 0);
        }

        public bool Visible
        {
            get => _visible;
            set
            { 
                _visible = value;
                PromptSetVisible(Handle, value ? 1 : 0);
            }
        }

        public void SetStandardMode(bool mode) => PromptSetStandardMode(Handle, mode ? 1 : 0);
        public void SetStandardizedHoldMode(bool mode) => PromptSetStandardizedHoldMode(Handle, mode ? 1 : 0);
        public void SetHoldMode(int mode) => PromptSetHoldMode(Handle, mode);
        public bool HasHoldModeCompleted => PromptHasHoldModeCompleted(Handle);
        public bool HasHoldMode => Function.Call<bool>((Hash)0xB60C9F9ED47ABB76, Handle);
        public bool IsHoldModeRunning => PromptIsHoldModeRunning(Handle);
        public bool IsActive => PromptIsActive(Handle);
        public bool IsPressed => PromptIsPressed(Handle);
        public bool IsReleased => PromptIsReleased(Handle);
        public bool IsValid => PromptIsValid(Handle);
        public bool IsJustPressed => PromptIsJustPressed(Handle);
        public bool IsJustReleased => PromptIsJustReleased(Handle);

    }
}
