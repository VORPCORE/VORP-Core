using System.Drawing;

namespace Vorp.Core.Client.Managers.Admin
{
    public class KeyCodeManager : Manager<KeyCodeManager>
    {
        // https://github.com/MoosheTV/DevTools/blob/master/Devtools.Client/Controllers/DevTools.cs

        bool KeyCodeTest = false;
        DateTime _lastCollection = DateTime.UtcNow;
        readonly List<KeyCodeEvent> _keyEvents = new List<KeyCodeEvent>();

        public override void Begin()
        {
#if DEVELOPMENT_CLIENT
            RegisterCommand("keyCode", new Action(() => KeyCodeTest = !KeyCodeTest), false);
#endif
        }

        [TickHandler]
        private async Task OnKeyCodeRender()
        {
            try
            {
                if (!KeyCodeTest)
                {
                    await BaseScript.Delay(100);
                    return;
                }

                var offsetY = 0f;
                foreach (var key in new List<KeyCodeEvent>(_keyEvents))
                {
                    var secs = (float)(DateTime.UtcNow - key.Time).TotalSeconds;
                    offsetY += 0.024f * MathUtil.Clamp(secs * 4f, 0f, 1f);

                    var alpha = Math.Pow(Math.Sin(MathUtil.Clamp(secs / 5f, 0f, 1f)), 1f / 16f) * 255f;

                    var color = Color.FromArgb((int)Math.Ceiling(alpha), 255, 255, 255);

                    VorpAPI.DrawText($@"{{{string.Join(", ", key.Controls)}}}", new Vector2(0.0f, 1f) - new Vector2(0f, offsetY), 0.3f);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"OnKeyCodeRender");
            }
        }

        [TickHandler]
        private async Task OnKeyCodeTick()
        {
            try
            {
                if (!KeyCodeTest)
                {
                    await BaseScript.Delay(100);
                    return;
                }

                var key = new KeyCodeEvent();
                var vals = Enum.GetValues(typeof(eControl));
                var count = 0;
                foreach (eControl ctrl in vals)
                {
                    if (IsControlJustPressed(2, (uint)ctrl))
                        key.Controls.Add($"{Enum.GetName(typeof(eControl), ctrl) ?? "UNK_KEY"}[{count}]");
                    count++;
                }

                if (key.Controls.Any())
                {
                    _keyEvents.Add(key);
                    Logger.Info($"[KeyCode] {{ {string.Join(", ", key.Controls)} }}");
                }

                // Clean up list every 5 seconds
                if ((DateTime.UtcNow - _lastCollection).TotalSeconds > 1f)
                {
                    _keyEvents.RemoveAll(e => (DateTime.UtcNow - e.Time).TotalSeconds > 5f);
                    _lastCollection = DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"OnKeyCodeTick");
            }
        }

        private class KeyCodeEvent
        {
            public DateTime Time { get; set; } = DateTime.UtcNow;
            public List<string> Controls { get; set; } = new List<string>();
        }
    }
}
