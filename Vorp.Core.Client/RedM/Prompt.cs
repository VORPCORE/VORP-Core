namespace Vorp.Core.Client.RedM.PromptEvents
{
    public delegate void PromptEvent();
}

namespace Vorp.Core.Client.RedM
{
    internal class Prompt : PoolObject
    {
        static bool _visible = true;
        static string _label;
        static ePromptType _promptType;
        static int _group;
        public bool EventTriggered = false;

        public Prompt(int handle, ePromptType promptType, string label) : base(handle)
        {
            _promptType = promptType;
            _label = label;
            Logger.Trace($"Registered '{label}'");
        }

        // void func_2074(
        // int globalPromptIndexValue (iParam0),
        // int promptLabel (iParam1),
        // char* promptSetTag (sParam2), (BRT2MountPrompt, CTX_GRIP, CTX_HOOK, INPUT_CRK_PROMPT, CTX_REEL, MR53_UC_BRFL)
        // int contextSetting (iParam3), (4 = Set Context Point and Size, 2 = Set Context Volume (param10), 5 = Set Context Volume (param10) and Size, Point Vector3.Zero)
        // int promptPriority (iParam4), (0 - Low, 1 = Normal, 2 = High, 3 = Critical)
        // int promptTransportMode (iParam5), (0 ANY, 1 ON FOOT, 2 IN VEHICLE)
        // vector3 contextPoint (vParam6 (6,7,8)),
        // float contextSize (fParam9),
        // int contextVolume (iParam10)
        // int contextVolume2 (iParam11),
        // int iParam12,
        // int iParam13,
        // int promptSetType (iParam14),
        // int iParam15 (used on Hold),
        // int holdDuration?,
        // int iParam17 (used on Mash),
        // int iParam18 (mash resistance?),
        // int iParam19,
        // int iParam20 (Rotate Mode),
        // int iParam21 (Rotate Mode),
        // bool bParam22 (53CE46C01A089DA1),
        // int iParam23 (STANDARDIZED_HOLD_MODE HASH - SHORT_TIMED_EVENT_MP, SHORT_TIMED_EVENT, MEDIUM_TIMED_EVENT, LONG_TIMED_EVENT, RUSTLING_CALM_TIMING, PLAYER_FOCUS_TIMING, PLAYER_REACTION_TIMING),
        // bool bParam24 (STANDARDIZED_HOLD_MODE ENABLE - promptSetType 4,5),
        // bool bParam25 (_UIPROMPT_SET_ATTRIBUTE if true))
        //
        //

        public static Prompt Create(eControl control, string label, int priority = 1, int transportMode = 0,
            string tag = null,
            ePromptType promptType = ePromptType.Pressed,
            Vector3? contextPoint = null,
            float contextSize = 0f,
            uint timedEventHash = 0
            )
        {
            List<eControl> controls = new() { control };
            return Create(controls, label, priority, transportMode, tag, promptType, contextPoint, contextSize, timedEventHash);
        }

        public static Prompt Create(List<eControl> controls, string label, int priority = 1, int transportMode = 0,
            string tag = null,
            ePromptType promptType = ePromptType.Pressed,
            Vector3? contextPoint = null,
            float contextSize = 0f,
            uint timedEventHash = 0
            )
        {
            int promptHandle = PromptRegisterBegin();

            foreach (eControl control in controls)
            {
                Function.Call((Hash)0xB5352B7494A08258, promptHandle, (long)control); // UiPromptSetControlAction
            }

            long str = Function.Call<long>(Hash._CREATE_VAR_STRING, 10, "LITERAL_STRING", label);
            Function.Call((Hash)0x5DD02A8318420DD7, promptHandle, str); // UiPromptSetText

            Function.Call((Hash)0xCA24F528D0D16289, promptHandle, priority); // UiPromptSetPriority
            Function.Call((Hash)0x876E4A35C73A6655, promptHandle, transportMode); // UiPromptSetTransportMode
            Function.Call((Hash)0x560E76D5E2E1803F, promptHandle, 18, true); // UiPromptSetAttribute

            if (!string.IsNullOrEmpty(tag))
                Function.Call((Hash)0xDEC85C174751292B, promptHandle, tag); // UiPromptSetTag

            switch (promptType) // All of this is still being tested and checked
            {
                case ePromptType.JustReleased:
                case ePromptType.Released:
                    Function.Call((Hash)0xCC6656799977741B, promptHandle, true);
                    break;
                case ePromptType.JustPressed:
                case ePromptType.Pressed:
                    Function.Call((Hash)0xCC6656799977741B, promptHandle, false);
                    break;
                case ePromptType.StandardHold:
                    Function.Call((Hash)0x94073D5CA3F16B7B, promptHandle, true); // UiPromptSetHoldMode
                    break;
                case ePromptType.StandardizedHold:
                    Function.Call((Hash)0x74C7D7B72ED0D3CF, promptHandle, timedEventHash); // PromptSetStandardizedHoldMode
                    break;
            }

            if (contextPoint is not null)
                Function.Call((Hash)0xAE84C5EE2C384FB3, promptHandle, contextPoint.Value.X, contextPoint.Value.Y, contextPoint.Value.Z);
            if (contextSize > 0f)
                Function.Call((Hash)0x0C718001B77CA468, promptHandle, contextSize);

            PromptRegisterEnd(promptHandle);

            PromptSetVisible(promptHandle, 1);
            PromptSetEnabled(promptHandle, 1);

            return new Prompt(promptHandle, promptType, label);
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
                Enabled = value;
            }
        }

        public void SetStandardMode(bool mode) => PromptSetStandardMode(Handle, mode ? 1 : 0);
        public void SetStandardizedHoldMode(bool mode) => PromptSetStandardizedHoldMode(Handle, mode ? 1 : 0);
        public void SetHoldMode(int mode) => PromptSetHoldMode(Handle, mode);
        public bool HasHoldModeCompleted => PromptHasHoldModeCompleted(Handle);
        public bool HasHoldMode => Function.Call<bool>((Hash)0xB60C9F9ED47ABB76, Handle);
        public bool IsHoldModeRunning => PromptIsHoldModeRunning(Handle);
        public bool IsActive => PromptIsActive(Handle);
        public bool IsPressed => Function.Call<bool>((Hash)0x21E60E230086697F, Handle);
        public bool IsReleased => Function.Call<bool>((Hash)0xAFC887BA7A7756D6, Handle);
        public bool IsValid => PromptIsValid(Handle);
        public bool IsJustPressed => Function.Call<bool>((Hash)0x2787CC611D3FACC5, Handle);
        public bool IsJustReleased => Function.Call<bool>((Hash)0x635CC82FA297A827, Handle);
        public int Group
        {
            get => _group;
            set
            {
                _group = value;
                PromptSetGroup(Handle, value, 0);
            }

        }
    }
}
