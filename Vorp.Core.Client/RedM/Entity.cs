namespace Vorp.Core.Client.RedM
{
    public class Entity : PoolObject
    {
        public Entity(int handle) : base(handle)
        {

        }

        /// <summary>
        /// Deletes this <see cref="Entity"/>
        /// </summary>
        public override void Delete()
        {
            _Delete();
        }

        private void _Delete()
        {
            // prevent the game from crashing if this is called on the player ped.
            if (Handle != PluginManager.Instance.LocalPlayer.Character.Handle)
            {
                API.SetEntityAsMissionEntity(Handle, false, true);
                int handle = Handle;
                API.DeleteEntity(ref handle);
                Handle = handle;
            }
        }

        public override bool Exists()
        {
            return DoesEntityExist(Handle);
        }

        /// <summary>
        /// Gets the position in world coords of an offset relative this <see cref="Entity"/>
        /// </summary>
        /// <param name="offset">The offset from this <see cref="Entity"/>.</param>
        public Vector3 GetOffsetPosition(Vector3 offset)
        {
            return API.GetOffsetFromEntityInWorldCoords(Handle, offset.X, offset.Y, offset.Z);
        }
        /// <summary>
        /// Gets the relative offset of this <see cref="Entity"/> from a world coords position
        /// </summary>
        /// <param name="worldCoords">The world coords.</param>
        public Vector3 GetPositionOffset(Vector3 worldCoords)
        {
            return API.GetOffsetFromEntityGivenWorldCoords(Handle, worldCoords.X, worldCoords.Y, worldCoords.Z);
        }

        /// <summary>
        /// Sets the position of this <see cref="Entity"/> without any offset.
        /// </summary>
        /// <value>
        /// The position in world space.
        /// </value>
        public Vector3 PositionNoOffset
        {
            set
            {
                API.SetEntityCoordsNoOffset(Handle, value.X, value.Y, value.Z, true, true, true);
            }
        }

        public Vector3 Position
        {
            get
            {
                return GetEntityCoords(Handle, false, false);
            }
            set
            {
                SetEntityCoords(Handle, value.X, value.Y, value.Z, false, false, false, false);
            }
        }

        public float Heading
        {
            get
            {
                return GetEntityHeading(Handle);
            }
            set
            {
                SetEntityHeading(Handle, value);
            }
        }

        public bool IsPositionFrozen
        {
            get => Function.Call<bool>((Hash)0x083D497D57B7400F, Handle);
            set => FreezeEntityPosition(Handle, value);
        }


    }
}
