using System;
using Xunit;

namespace UzunTec.Utils.DatabaseAbstraction.SQLServer.Test
{
    public static class AssertExt
    {
        public static void UsersTheSame(params User[] users)
        {
            Assert.True(users.Length > 1);
            User pivot = users[0];

            for (int i = 1; i < users.Length; i++)
            {
                User comparedUser = users[i];
                Assert.Equal(pivot.UserCode, comparedUser.UserCode);
                Assert.Equal(pivot.UserName, comparedUser.UserName);
                Assert.Equal(pivot.UserCodRef, comparedUser.UserCodRef);
                Assert.Equal(pivot.PasswordMd5, comparedUser.PasswordMd5);
                Assert.Equal(pivot.InputDate, comparedUser.InputDate, new TimeSpan(0, 0, 0, 1));
            }
        }
    }
}
