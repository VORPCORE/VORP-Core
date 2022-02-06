using Vorp.Core.Client.RedM.Enums;

namespace Vorp.Core.Client.RedM
{
    internal class VorpAPI
    {
        public static void SetMpGamerTagVisibility(int gamerTagId, eUIGamertagVisibility visibility)
        {
            Function.Call((Hash)0x93171DDDAB274EB8, gamerTagId, (int)visibility);
        }

        public static int CreateGamerTag(int playerId, string name, bool pointedTag = false, bool isRockstarClan = false, string clanTag = "", int clanFlag = 0)
        {
            return Function.Call<int>((Hash)0xD877AF112AD2B41B, playerId, name, pointedTag, isRockstarClan, clanTag, clanFlag);
        }

        public static float Distance(Vector3 start, Vector3 end)
        {
            return Vdist(start.X, start.Y, start.Z, end.X, end.Y, end.Z);
        }
    }
}
