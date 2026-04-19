using Library_Data;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Library_Business
{
    public static class clsUsersToken
    {
        public static bool Login(int UserID, string RefreshTokenHash, DateTime? RefreshTokenExpiresAt) =>
            clsUsersTokensData.Login(UserID, RefreshTokenHash, RefreshTokenExpiresAt) != -1;

        public static bool Logout(int UserID, DateTime? RefreshTokenRevokedAt) =>
            clsUsersTokensData.Logout(UserID, RefreshTokenRevokedAt);

        public static string GetRefreshTokenHashForUser(int UserID) =>
            clsUsersTokensData.GetRefreshTokenHashForUser(UserID);

        public static DateTime? GetRefreshTokenRevokedAt(int UserID) =>
            clsUsersTokensData.GetRefreshTokenRevokedAt(UserID);

        public static DateTime? GetRefreshTokenExpiresAt(int UserID) =>
            clsUsersTokensData.GetRefreshTokenExpiresAt(UserID);

        public static bool Refresh(int UserID, string RefreshTokenHash, DateTime? RefreshTokenExpiresAt) =>
            clsUsersTokensData.Refresh(UserID, RefreshTokenHash, RefreshTokenExpiresAt);
    
        public static (DateTime? expiresAt, DateTime? revokedAt, string hash) GetTokenDataForUser(int UserID) =>
            clsUsersTokensData.GetTokenDataForUser(UserID);
    }
}
