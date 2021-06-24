using System;
using UzunTec.Utils.Common;
using Xunit;

namespace UzunTec.Utils.DatabaseAbstraction.SQLServer.Test
{
    [Collection("BootstrapCollectionFixture")]
    public class DbAbstractionTestInsert
    {
        private readonly UserQueryClient client;

        public DbAbstractionTestInsert(BootstrapFixture bootstrap)
        {
            this.client = bootstrap.GetInstance<UserQueryClient>();
        }

        [Fact]
        public void InsertUserWithCodRefTest()
        {
            this.client.Delete(91);
            User userToInsert = new User
            {
                UserCode = 91,
                UserCodRef = 423423423432,
                UserName = "Test User1",
                InputDate = DateTime.Now,
                PasswordMd5 = MD5Hash.CalculateMD5Hash("anything"),
                Status = StatusUser.User,
            };

            this.client.Delete(userToInsert.UserCode);  // Avoid Duplicates

            Assert.True(this.client.Insert(userToInsert));
            User insertedUser = this.client.FindByCode(userToInsert.UserCode);
            Assert.NotNull(insertedUser);
            AssertExt.UsersTheSame(userToInsert, insertedUser);
            Assert.True(this.client.Delete(insertedUser.UserCode));
        }


        [Fact]
        public void InsertUserWithoutCodRefTest()
        {
            this.client.Delete(92);
            User userToInsert = new User
            {
                UserCode = 92,
                UserName = "Test User2",
                InputDate = DateTime.Now,
                PasswordMd5 = MD5Hash.CalculateMD5Hash("anything-else"),
                Status = StatusUser.Guest,
            };

            this.client.Delete(userToInsert.UserCode);  // Avoid Duplicates

            Assert.True(this.client.Insert(userToInsert));
            User insertedUser = this.client.FindByCode(userToInsert.UserCode);
            Assert.NotNull(insertedUser);
            AssertExt.UsersTheSame(insertedUser, userToInsert);
            Assert.True(this.client.Delete(insertedUser.UserCode));
        }
    }
}
