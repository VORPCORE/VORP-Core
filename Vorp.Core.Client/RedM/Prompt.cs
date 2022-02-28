using Vorp.Core.Client.RedM.PromptEvents;

namespace Vorp.Core.Client.RedM.PromptEvents
{
    public delegate void PromptEvent();
}

namespace Vorp.Core.Client.RedM
{
    internal class Prompt : PoolObject
    {
        public event PromptEvent OnPromptEvents;

        static bool _visible = true;
        static string _label;
        static ePromptType _promptType;

        public Prompt(int handle) : base(handle)
        {

        }

        public static Prompt Create(eControl control, string label, ePromptType ePromptType, Vector3? contextPoint = null, float contextSize = 0f)
        {
            int promptHandle = PromptRegisterBegin();

            // PromptSetControlAction(promptHandle, 1); // This is the label to display

            Function.Call((Hash)0xB5352B7494A08258, promptHandle, (uint)control);

            _label = label;
            long str = Function.Call<long>(Hash._CREATE_VAR_STRING, 10, "LITERAL_STRING", label);
            Function.Call((Hash)0x5DD02A8318420DD7, promptHandle, str);

            switch (ePromptType)
            {
                case ePromptType.StandardHold:
                    PromptSetHoldMode(promptHandle, 1);
                    break;
                case ePromptType.StandardizedHold:
                    PromptSetStandardizedHoldMode(promptHandle, 1);
                    break;
            }

            if (contextPoint is not null)
                Function.Call((Hash)0xAE84C5EE2C384FB3, promptHandle, contextPoint.Value.X, contextPoint.Value.Y, contextPoint.Value.Z);
            if (contextSize > 0f)
                Function.Call((Hash)0x0C718001B77CA468, promptHandle, contextSize);

            PromptRegisterEnd(promptHandle);

            PromptSetVisible(promptHandle, 1);
            PromptSetEnabled(promptHandle, 1);

            return new Prompt(promptHandle);
        }

        public void TriggerEvent()
        {
            Logger.Debug($"Prompt '{_label}' Event Triggered");
            OnPromptEvents?.Invoke();
        }

        public ePromptType Type => _promptType;

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
