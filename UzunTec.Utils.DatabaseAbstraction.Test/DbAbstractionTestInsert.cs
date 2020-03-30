using System;
using UzunTec.Utils.Common;
using Xunit;

namespace UzunTec.Utils.DatabaseAbstraction.Test
{
    public class DbAbstractionTestInsert
    {
        private readonly UserQueryClient client;

        public DbAbstractionTestInsert()
        {
            this.client = DbAbstractionTestContainer.INSTANCE.GetInstance<UserQueryClient>();
        }

        [Fact]
        public void InsertUserWithCodRefTest()
        {
            User userToInsert = new User
            {
                UserCode = 91,
                UserCodRef = 423423423432,
                UserName = "Test User1",
                InputDate = DateTime.Now,
                PasswordMd5 = MD5Hash.CalculateMD5Hash("anything"),
            };

            Assert.True(client.Insert(userToInsert));
            User insertedUser = client.FindByCode(91);
            Assert.NotNull(insertedUser);
            AssertExt.UsersTheSame(userToInsert, insertedUser);
            Assert.True(client.Delete(insertedUser.UserCode));
        }


        [Fact]
        public void InsertUserWithoutCodRefTest()
        {
            User userToInsert = new User
            {
                UserCode = 92,
                UserName = "Test User2",
                InputDate = DateTime.Now,
                PasswordMd5 = MD5Hash.CalculateMD5Hash("anything-else"),
            };

            Assert.True(client.Insert(userToInsert));
            User insertedUser = client.FindByCode(92);
            Assert.NotNull(insertedUser);
            AssertExt.UsersTheSame(insertedUser, userToInsert);
            Assert.True(client.Delete(insertedUser.UserCode));
        }
    }
}
