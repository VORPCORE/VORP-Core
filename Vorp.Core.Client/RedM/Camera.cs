using Vorp.Core.Client.Environment.Entities;

namespace Vorp.Core.Client.RedM
{
    /// <summary>
    /// Copy required methods from CFX
    /// https://github.com/citizenfx/fivem/blob/cbe56f78f86bebb68d7960a38c3cdc31c7d76790/code/client/clrcore/External/Camera.cs
    /// </summary>
    public class Camera : PoolObject
    {
        public Camera(int handle) : base(handle)
        {

        }

        public Vector3 Position
        {
            get
            {
                return GetCamCoord(Handle);
            }
            set
            {
                SetCamCoord(Handle, value.X, value.Y, value.Z);
            }
        }

        public Vector3 Rotation
        {
            get
            {
                return GetCamRot(Handle, 2);
            }
            set
            {
                SetCamRot(Handle, value.X, value.Y, value.Z, 2);
            }
        }

        public float FieldOfView
        {
            get
            {
                return API.GetCamFov(Handle);
            }
            set
            {
                API.SetCamFov(Handle, value);
            }
        }

		public bool IsActive
		{
			get
			{
				return API.IsCamActive(Handle);
			}
			set
			{
				API.SetCamActive(Handle, value);
			}
		}

		public void PointAt(int target, Vector3 offset = default(Vector3))
        {
            API.PointCamAtEntity(Handle, target, offset.X, offset.Y, offset.Z, true);
        }

        public override void Delete()
        {
			API.SetCamActive(Handle, false);
			API.DestroyCam(Handle, false);
        }

        public override bool Exists()
        {
            return API.DoesCamExist(Handle);
        }

		public void AttachTo(VorpPlayer vorpPlayer, Vector3 offset)
        {
			AttachCamToEntity(Handle, vorpPlayer.PlayerPedId, offset.X, offset.Y, offset.Z, true);
		}

		/// <summary>
		/// Gets the up vector of this <see cref="Camera"/>.
		/// </summary>
		public Vector3 UpVector
		{
			get
			{
				return Matrix.Up;
			}
		}

		/// <summary>
		/// Gets the forward vector of this <see cref="Camera"/>, see also <seealso cref="Direction"/>.
		/// </summary>
		public Vector3 ForwardVector
		{
			get
			{
				return Matrix.Forward;
			}
		}

		/// <summary>
		/// Gets the right vector of this <see cref="Camera"/>.
		/// </summary>
		public Vector3 RightVector
		{
			get
			{
				return Matrix.Right;
			}
		}
		/// <summary>
		/// Gets the matrix of this <see cref="Camera"/>. CURRENTLY DOESN'T WORK
		/// </summary>
		public Matrix Matrix
		{
			get
			{
				Vector3 rightVector = new Vector3();
				Vector3 forwardVector = new Vector3();
				Vector3 upVector = new Vector3();
				Vector3 position = new Vector3();

				API.GetCamMatrix(Handle, ref rightVector, ref forwardVector, ref upVector, ref position);

				return new Matrix(
					rightVector.X, rightVector.Y, rightVector.Z, 0.0f,
					forwardVector.X, forwardVector.Y, forwardVector.Z, 0.0f,
					upVector.X, upVector.Y, upVector.Z, 0.0f,
					position.X, position.Y, position.Z, 1.0f
				);
			}
		}
	}

    public static class GameplayCamera
    {
		/// <summary>
		/// Gets the position of the <see cref="GameplayCamera"/>.
		/// </summary>
		public static Vector3 Position
		{
			get
			{
				return API.GetGameplayCamCoord();
			}
		}
		/// <summary>
		/// Gets the rotation of the <see cref="GameplayCamera"/>.
		/// </summary>
		/// <value>
		/// The yaw, pitch and roll rotations measured in degrees.
		/// </value>
		public static Vector3 Rotation
		{
			get
			{
				return API.GetGameplayCamRot(2);
			}
		}

		/// <summary>
		/// Gets the relative offset of the <see cref="GameplayCamera"/> from a world coords position
		/// </summary>
		/// <param name="worldCoords">The world coords.</param>
		public static Vector3 GetPositionOffset(Vector3 worldCoords)
		{
			return default(Vector3);
			//return Matrix.InverseTransformPoint(worldCoords);
		}

		/// <summary>
		/// Gets or sets the relative pitch of the <see cref="GameplayCamera"/>.
		/// </summary>
		public static float RelativePitch
		{
			get
			{
				return API.GetGameplayCamRelativePitch();
			}
			set
			{
				API.SetGameplayCamRelativePitch(value, 1f);
			}
		}
		/// <summary>
		/// Gets or sets the relative heading of the <see cref="GameplayCamera"/>.
		/// </summary>
		public static float RelativeHeading
		{
			get
			{
				return API.GetGameplayCamRelativeHeading();
			}
			set
			{
				API.SetGameplayCamRelativeHeading((int)value, 0);
			}
		}

		/// <summary>
		/// Clamps the yaw of the <see cref="GameplayCamera"/>.
		/// </summary>
		/// <param name="min">The minimum yaw value.</param>
		/// <param name="max">The maximum yaw value.</param>
		public static void ClampYaw(float min, float max)
		{
			API.ClampGameplayCamYaw(min, max);
		}

		/// <summary>
		/// Clamps the pitch of the <see cref="GameplayCamera"/>.
		/// </summary>
		/// <param name="min">The minimum pitch value.</param>
		/// <param name="max">The maximum pitch value.</param>
		public static void ClampPitch(float min, float max)
		{
			API.ClampGameplayCamPitch(min, max);
		}

		/// <summary>
		/// Gets the field of view of the <see cref="GameplayCamera"/>.
		/// </summary>
		public static float FieldOfView
		{
			get
			{
				return API.GetGameplayCamFov();
			}
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="GameplayCamera"/> is rendering.
		/// </summary>
		/// <value>
		/// <c>true</c> if the <see cref="GameplayCamera"/> is rendering; otherwise, <c>false</c>.
		/// </value>
		public static bool IsRendering
		{
			get
			{
				return API.IsGameplayCamRendering();
			}
		}
	}
}
