using Vorp.Core.Client.Environment.Entities;
using Vorp.Core.Client.RedM.Enums;

namespace Vorp.Core.Client.Managers.Admin
{
    public class NoClipManager : Manager<NoClipManager>
    {

        VorpPlayer Player = PluginManager.Instance.LocalPlayer;

        const float _minY = -89f, _maxY = 89f;
        const float _maxSpeed = 32f;

        public Camera CurrentCamera { get; set; }
        public float Speed { get; set; } = 1f;

        readonly List<eControl> _disabledControls = new()
        {
            eControl.MoveLeftOnly,
            eControl.MoveLeftRight,
            eControl.MoveUpDown,
            eControl.MoveUpOnly,

            // mouse
            eControl.LookLeftRight,
            eControl.LookUpDown,
            // scroll wheel
            eControl.SelectNextWeapon,
            eControl.SelectPrevWeapon,
            // modifiers
            eControl.Sprint,
            eControl.PcFreeLook,
            eControl.Duck,
            // Up and Down
            eControl.Dive, // Q
            eControl.ContextY // E
        };

        public bool IsEnabled = false;

        public override void Begin() // Should make this an admin control
        {

        }

        public void Toggle()
        {
            IsEnabled = !IsEnabled;
            Player = PluginManager.Instance.LocalPlayer;

            if (IsEnabled)
                Instance.AttachTickHandler(OnNoClipControlTick);
        }

        private async Task OnNoClipControlTick()
        {
            try
            {
                if (!IsEnabled)
                {
                    if (CurrentCamera is not null)
                    {
                        CurrentCamera.Delete();
                        CurrentCamera = null;

                        Vector3 pos = Player.Position;
                        float groundZ = pos.Z;
                        Vector3 norm = Vector3.Zero;
                        if (API.GetGroundZAndNormalFor_3dCoord(pos.X, pos.Y, pos.Z, ref groundZ, ref norm))
                            PluginManager.Instance.LocalPlayer.Position = new Vector3(pos.X, pos.Y, groundZ);

                        Player.IsPositionFrozen = false;
                        Player.IsCollisionEnabled = true;
                        Player.CanRagdoll = true;
                        Player.IsVisible = true;
                        Player.Opacity = 255;

                        // Enable controls
                        foreach (var ctrl in _disabledControls)
                        {
                            DisableControlAction(0, (uint)ctrl, false);
                        }

                        Instance.DetachTickHandler(OnNoClipCheckRotationTick);

                        DisplayHud(true);
                        DisplayRadar(true);

                        await BaseScript.Delay(100);

                        Instance.DetachTickHandler(OnNoClipControlTick);
                    }
                    return;
                }

                // Create camera on toggle
                if (CurrentCamera is null)
                {
                    CurrentCamera = VorpAPI.CreateCameraWithParams(Player.Position, GameplayCamera.Rotation, 75f);
                    CurrentCamera.AttachTo(Player, Vector3.Zero);
                    VorpAPI.RenderingCamera = CurrentCamera;

                    Player.IsPositionFrozen = true;
                    Player.IsCollisionEnabled = false;
                    Player.Opacity = 0;
                    Player.CanRagdoll = false;
                    Player.IsVisible = false;

                    DisplayHud(false);
                    DisplayRadar(false);

                    Instance.AttachTickHandler(OnNoClipCheckRotationTick);
                }

                VorpAPI.DrawText($"{CurrentCamera.Position}", new Vector2(0f, 0.025f), 0.3f);
                VorpAPI.DrawText($"{CurrentCamera.Rotation}", new Vector2(0f, 0.05f), 0.3f);

                // Speed Control
                if (IsDisabledControlPressed(0, (uint)eControl.SelectPrevWeapon))
                {
                    Speed = Math.Min(Speed + 0.1f, _maxSpeed);
                }
                else if (IsDisabledControlPressed(0, (uint)eControl.SelectNextWeapon))
                {
                    Speed = Math.Max(0.1f, Speed - 0.1f);
                }

                var multiplier = 1f;
                if (IsDisabledControlPressed(0, (uint)eControl.Sprint))
                {
                    multiplier = 2f;
                }
                else if (IsDisabledControlPressed(0, (uint)eControl.PcFreeLook))
                {
                    multiplier = 4f;
                }
                else if (IsDisabledControlPressed(0, (uint)eControl.Duck))
                {
                    multiplier = 0.25f;
                }

                // Forward
                if (IsDisabledControlPressed(2, (uint)eControl.MoveUpOnly))
                {
                    var pos = Player.GetOffsetPosition(new Vector3(0f, Speed * multiplier, 0f));
                    Player.PositionNoOffset = new Vector3(pos.X, pos.Y, Player.Position.Z);
                    // Player.PositionNoOffset = Player.Position + CurrentCamera.UpVector * (Speed * multiplier);
                }
                // Backward
                else if (IsDisabledControlPressed(2, (uint)eControl.MoveUpDown))
                {
                    var pos = Player.GetOffsetPosition(new Vector3(0f, -Speed * multiplier, 0f));
                    Player.PositionNoOffset = new Vector3(pos.X, pos.Y, Player.Position.Z);
                    // Player.PositionNoOffset = Player.Position - CurrentCamera.UpVector * (Speed * multiplier);
                }
                // Left
                if (IsDisabledControlPressed(0, (uint)eControl.MoveLeftOnly))
                {
                    var pos = Player.GetOffsetPosition(new Vector3(-Speed * multiplier, 0f, 0f));
                    Player.PositionNoOffset = new Vector3(pos.X, pos.Y, Player.Position.Z);
                }
                // Right
                else if (IsDisabledControlPressed(0, (uint)eControl.MoveLeftRight))
                {
                    var pos = Player.GetOffsetPosition(new Vector3(Speed * multiplier, 0f, 0f));
                    Player.PositionNoOffset = new Vector3(pos.X, pos.Y, Player.Position.Z);
                }

                // Up (E)
                if (IsDisabledControlPressed(0, (uint)eControl.ContextY))
                {
                    Player.PositionNoOffset = Player.GetOffsetPosition(new Vector3(0f, 0f, multiplier * Speed / 2));
                }

                // Down (Q)
                if (IsDisabledControlPressed(0, (uint)eControl.Dive))
                {
                    Player.PositionNoOffset = Player.GetOffsetPosition(new Vector3(0f, 0f, multiplier * -Speed / 2));
                }

                // Disable controls
                foreach (var ctrl in _disabledControls)
                {
                    DisableControlAction(0, (uint)ctrl, true);
                }

                Player.Heading = Math.Max(0f, (360 + CurrentCamera.Rotation.Z) % 360f);
                Player.Opacity = 0;
                DisablePlayerFiring(Player.Handle, false);


                VorpAPI.DrawText($"Speed: {Speed} / Multiplier: {multiplier}", new Vector2(0, 0), 0.3f);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"OnNoClipControlTick");
            }
        }

        private async Task OnNoClipCheckRotationTick()
        {
            try
            {
                var rightAxisX = GetDisabledControlNormal(0, (uint)eControl.LookLeftRight);
                var rightAxisY = GetDisabledControlNormal(0, (uint)eControl.LookUpDown);

                if (!(Math.Abs(rightAxisX) > 0) && !(Math.Abs(rightAxisY) > 0)) return;
                var rotation = CurrentCamera.Rotation;
                rotation.Z += rightAxisX * -10f;

                var yValue = rightAxisY * -5f;
                if (rotation.X + yValue > _minY && rotation.X + yValue < _maxY)
                    rotation.X += yValue;

                CurrentCamera.Rotation = rotation;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"OnNoClipCheckRotationTick");
            }
        }
    }
}
