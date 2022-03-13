using Vorp.Core.Client.Environment.Entities;

namespace Vorp.Core.Client.Managers.Admin
{
    public class NoClipManager : Manager<NoClipManager>
    {

        VorpPlayer Player = PluginManager.Instance.LocalPlayer;

        const float _minY = -89f, _maxY = 89f;
        const float _maxSpeed = 32f;

        public Camera CurrentCamera { get; set; }
        public float Speed { get; set; } = 1f;
        const float _maxFov = 180f;
        float fov = 75f;

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
            eControl.Jump,
            // set values
            eControl.SelectQuickselectSidearmsLeft, // camera ROT X (NUM 1)
            eControl.SelectQuickselectDualwield, // camera ROT Y (NUM 2)
            eControl.SelectQuickselectSidearmsRight, // camera ROT Z (NUM 3)
            // print to control
            eControl.FrontendAccept,
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
                Ped playerPed = Player.Character;

                if (!IsEnabled)
                {
                    if (CurrentCamera is not null)
                    {
                        CurrentCamera.Delete();
                        CurrentCamera = null;

                        Vector3 pos = playerPed.Position;
                        float groundZ = pos.Z;
                        Vector3 norm = Vector3.Zero;
                        if (API.GetGroundZAndNormalFor_3dCoord(pos.X, pos.Y, pos.Z, ref groundZ, ref norm))
                            playerPed.Position = new Vector3(pos.X, pos.Y, groundZ);

                        playerPed.IsPositionFrozen = false;
                        playerPed.IsCollisionEnabled = true;
                        playerPed.CanRagdoll = true;
                        playerPed.IsVisible = true;
                        playerPed.Opacity = 255;

                        // Enable controls
                        foreach (var ctrl in _disabledControls)
                        {
                            DisableControlAction(0, (uint)ctrl, false);
                        }

                        Instance.DetachTickHandler(OnNoClipCheckRotationTick);

                        DisplayHud(true);
                        DisplayRadar(true);

                        RenderScriptCams(false, false, 0, false, false, 0);

                        await BaseScript.Delay(100);

                        Instance.DetachTickHandler(OnNoClipControlTick);
                    }
                    return;
                }

                // Create camera on toggle
                if (CurrentCamera is null)
                {
                    CurrentCamera = VorpAPI.CreateCameraWithParams(playerPed.Position, GameplayCamera.Rotation, 75f);
                    CurrentCamera.AttachTo(playerPed, Vector3.Zero);
                    VorpAPI.RenderingCamera = CurrentCamera;

                    playerPed.IsPositionFrozen = true;
                    playerPed.IsCollisionEnabled = false;
                    playerPed.Opacity = 0;
                    playerPed.CanRagdoll = false;
                    playerPed.IsVisible = false;

                    DisplayHud(false);
                    DisplayRadar(false);

                    Instance.AttachTickHandler(OnNoClipCheckRotationTick);
                }

                // Speed Control
                if (IsDisabledControlPressed(0, (uint)eControl.SelectPrevWeapon) && !IsDisabledControlPressed(0, (uint)eControl.Jump))
                {
                    Speed = Math.Min(Speed + 0.1f, _maxSpeed);
                }
                else if (IsDisabledControlPressed(0, (uint)eControl.SelectNextWeapon) && !IsDisabledControlPressed(0, (uint)eControl.Jump))
                {
                    Speed = Math.Max(0.1f, Speed - 0.1f);
                }

                // FOV Control
                if (IsDisabledControlPressed(0, (uint)eControl.SelectPrevWeapon) && IsDisabledControlPressed(0, (uint)eControl.Jump))
                {
                    float change = 0.1f;
                    if (IsDisabledControlPressed(0, (uint)eControl.Sprint))
                    {
                        change = 1f;
                    }
                    fov = Math.Min(fov + change, _maxFov);
                    CurrentCamera.FieldOfView = fov;
                }
                else if (IsDisabledControlPressed(0, (uint)eControl.SelectNextWeapon) && IsDisabledControlPressed(0, (uint)eControl.Jump))
                {
                    float change = 0.1f;
                    if (IsDisabledControlPressed(0, (uint)eControl.Sprint))
                    {
                        change = 1f;
                    }
                    fov = Math.Max(change, fov - change);
                    CurrentCamera.FieldOfView = fov;
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
                    var pos = playerPed.GetOffsetPosition(new Vector3(0f, Speed * multiplier, 0f));
                    playerPed.PositionNoOffset = new Vector3(pos.X, pos.Y, playerPed.Position.Z);
                    // Player.PositionNoOffset = Player.Position + CurrentCamera.UpVector * (Speed * multiplier);
                }
                // Backward
                else if (IsDisabledControlPressed(2, (uint)eControl.MoveUpDown))
                {
                    var pos = playerPed.GetOffsetPosition(new Vector3(0f, -Speed * multiplier, 0f));
                    playerPed.PositionNoOffset = new Vector3(pos.X, pos.Y, playerPed.Position.Z);
                    // Player.PositionNoOffset = Player.Position - CurrentCamera.UpVector * (Speed * multiplier);
                }
                // Left
                if (IsDisabledControlPressed(0, (uint)eControl.MoveLeftOnly))
                {
                    var pos = playerPed.GetOffsetPosition(new Vector3(-Speed * multiplier, 0f, 0f));
                    playerPed.PositionNoOffset = new Vector3(pos.X, pos.Y, playerPed.Position.Z);
                }
                // Right
                else if (IsDisabledControlPressed(0, (uint)eControl.MoveLeftRight))
                {
                    var pos = playerPed.GetOffsetPosition(new Vector3(Speed * multiplier, 0f, 0f));
                    playerPed.PositionNoOffset = new Vector3(pos.X, pos.Y, playerPed.Position.Z);
                }

                // Up (E)
                if (IsDisabledControlPressed(0, (uint)eControl.ContextY))
                {
                    playerPed.PositionNoOffset = playerPed.GetOffsetPosition(new Vector3(0f, 0f, multiplier * Speed / 2));
                }

                // Down (Q)
                if (IsDisabledControlPressed(0, (uint)eControl.Dive))
                {
                    playerPed.PositionNoOffset = playerPed.GetOffsetPosition(new Vector3(0f, 0f, multiplier * -Speed / 2));
                }

                // NUM 1
                if (IsDisabledControlPressed(0, (uint)eControl.SelectQuickselectSidearmsLeft))
                {
                    if (IsDisabledControlPressed(0, (uint)eControl.Sprint))
                    {
                        float Y = CurrentCamera.Rotation.Y;
                        float Z = CurrentCamera.Rotation.Z;
                        CurrentCamera.Rotation = new Vector3(0f, Y, Z);
                        await BaseScript.Delay(100);
                    }
                    else
                    {
                        float mod = 90f;
                        if (IsDisabledControlPressed(0, (uint)eControl.Duck))
                            mod = 45f;

                        float X = CurrentCamera.Rotation.X + mod;
                        if (X < 0f)
                            X = CurrentCamera.Rotation.X - mod;

                        CurrentCamera.Rotation += new Vector3(X, 0f, 0f);
                    }
                }

                // NUM 2
                if (IsDisabledControlPressed(0, (uint)eControl.SelectQuickselectDualwield))
                {
                    if (IsDisabledControlPressed(0, (uint)eControl.Sprint))
                    {
                        float X = CurrentCamera.Rotation.X;
                        float Z = CurrentCamera.Rotation.Z;
                        CurrentCamera.Rotation = new Vector3(X, 0f, Z);
                        await BaseScript.Delay(100);
                    }
                    else
                    {
                        float mod = 90f;
                        if (IsDisabledControlPressed(0, (uint)eControl.Duck))
                            mod = 45f;

                        float Y = CurrentCamera.Rotation.Y + mod;
                        if (Y < 0f)
                            Y = CurrentCamera.Rotation.Y - mod;

                        CurrentCamera.Rotation += new Vector3(0f, Y, 0f);
                    }
                }

                // NUM 3
                if (IsDisabledControlPressed(0, (uint)eControl.SelectQuickselectSidearmsRight))
                {
                    if (IsDisabledControlPressed(0, (uint)eControl.Sprint))
                    {
                        float X = CurrentCamera.Rotation.X;
                        float Y = CurrentCamera.Rotation.Y;
                        CurrentCamera.Rotation = new Vector3(X, Y, 0f);
                        await BaseScript.Delay(100);
                    }
                    else
                    {
                        float mod = 90f;
                        if (IsDisabledControlPressed(0, (uint)eControl.Duck))
                            mod = 45f;

                        float Z = CurrentCamera.Rotation.Z + mod;
                        if (Z < 0f)
                            Z = CurrentCamera.Rotation.Z - mod;

                        CurrentCamera.Rotation += new Vector3(0f, 0f, Z);
                    }
                }

                // Disable controls
                foreach (var ctrl in _disabledControls)
                {
                    DisableControlAction(0, (uint)ctrl, true);
                }

                if (IsDisabledControlPressed(0, (uint)eControl.FrontendAccept))
                {
                    Logger.Trace($"Camera Position: {CurrentCamera.Position}");
                    Logger.Trace($"Camera Rotation: {CurrentCamera.Rotation}");
                    Logger.Trace($"Camera FOV: {CurrentCamera.FieldOfView}");
                    await BaseScript.Delay(100);
                }

                playerPed.Heading = Math.Max(0f, (360 + CurrentCamera.Rotation.Z) % 360f);
                playerPed.Opacity = 0;
                DisablePlayerFiring(playerPed.Handle, false);

                VorpAPI.DrawText($"Speed: {Speed} / Multiplier: {multiplier} / FOV: {fov} / POS: {CurrentCamera.Position} / ROT: {CurrentCamera.Rotation}", new Vector2(0, 0), 0.3f);
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
