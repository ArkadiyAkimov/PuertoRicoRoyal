using PuertoRicoAPI.Data.DataClasses;
using PuertoRicoAPI.Model;
using PuertoRicoAPI.Model.Roles;

namespace PuertoRicoAPI.Types
{
    public enum RoleName
    {
        Settler,
        Builder,
        Mayor,
        Trader,
        Craftsman,
        Captain,
        Prospector,
        PostCaptain,
        NoRole,
        Draft
    }
    public static class RoleInit
    {
        public static Role getRoleClass(DataRole dataRole,GameState gs)
        {
            switch (dataRole.Name)
            {
                case RoleName.Mayor:
                    return new Mayor(dataRole, gs);
                case RoleName.Builder:
                    return new Builder(dataRole, gs);
                case RoleName.Settler:
                    return new Settler(dataRole, gs);
                case RoleName.Craftsman:
                    return new Craftsman(dataRole, gs);
                case RoleName.Trader:
                    return new Trader(dataRole, gs);
                case RoleName.Captain:
                    return new Captain(dataRole, gs);
                case RoleName.PostCaptain:
                    return new PostCaptain(dataRole, gs);
                case RoleName.Draft:
                    return new Draft(dataRole, gs);
                default:
                    return new Prospector(dataRole, gs);
            }
        }
    }
}
